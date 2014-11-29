#region "about"

//
// eFLOW Collection Management SDK
// 2012 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to make eFLOW programming safe, a lot easier, faster and fun, basically removing complexity.
// Developed by: Eduardo Freitas
//

#endregion "about"

#region "using"

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using TiS.Core.TisCommon;
using TiS.Core.Domain;
using TiS.Core.Common;
using TiS.Core.Application;
using TiS.Core.Application.Interfaces;
using TiS.Core.Application.DataModel.Dynamic;
using TiS.Core.Application.Workflow;
using System.Data.SqlClient;

#endregion "using"

#region "namespace eFlow.CollectionManagement"

namespace eFlow.CollectionManagement
{
    /// <summary>
    /// "BatchInternals" Class --> Is an internal wrapper class that performs atomic operations around a ITisCollectionData object. 
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class BatchInternals : IDisposable
    {
        #region "class Batch private declarations"
        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// Name of the eFLOW application logged on to the CSM
        /// </summary>
        protected string applicationName = String.Empty;

        /// <summary>
        /// Name of the eFLOW station logged on to the CSM
        /// </summary>
        protected string stationName = String.Empty;

        /// <summary>
        /// CSM object instance
        /// </summary>
        protected ITisClientServicesModule oCSM = null;

        /// <summary>
        /// List of Collections recently loaded through a Batch->Get() or Batch->FromDynamic()
        /// </summary>
        protected static List<string> collections = new List<string>();
        #endregion "class Batch private declarations"

        #region "Properties"

        /// <summary>
        /// Enumeration of Collection priorities
        /// </summary>
        public enum Priority
        {
            /// <summary>
            ///  Indicates a Low Priority
            /// </summary>
            Low,
            /// <summary>
            ///  Indicates a Normal Priority
            /// </summary>
            Normal,
            /// <summary>
            ///  Indicates an AboveNormal Priority
            /// </summary>
            AboveNormal,
            /// <summary>
            ///  Indicates a High Priority
            /// </summary>
            High
        };

        /// <summary>
        /// Read Only - List of Collections recently loaded through a Batch->Get() or Batch->FromDynamic()
        /// </summary>
        /// <example><code>string[] cols = b.CollectionsLoaded;</code></example>
        public string[] CollectionsLoaded
        {
            get
            {
                return (collections != null) ? collections.ToArray() : null;
            }
        }

        /// <summary>
        /// Name of the eFLOW application logged on to the CSM
        /// </summary>
        /// <example><code>b.ApplicationName = "SimpleDemo";</code></example>
        public string ApplicationName
        {
            get
            {
                return this.applicationName;
            }
            set
            {
                this.applicationName = value;
            }
        }

        /// <summary>
        /// Name of the eFLOW station logged on to the CSM
        /// </summary>
        /// <example><code>b.StationName = "Completion";</code></example>
        public string StationName
        {
            get
            {
                return this.stationName;
            }
            set
            {
                this.stationName = value;
            }
        }

        #endregion "Properties"

        #region "Constructor-Finalizer-Dispose"

        #region "Constructor"
        /// <summary>
        /// [Constructor] BatchInternals() --> Initializes a Batch object instance with the eFLOW application and station names.
        /// </summary>
        /// <param name="applicationName">Indicates the name of the eFLOW application to login to.</param>
        /// <param name="stationName">Indicates the name of the eFLOW station to login to.</param>
        /// <example><code>BatchInternals b = new BatchInternals("SimpleDemo", "Completion");</code></example>
        protected BatchInternals (string applicationName, string stationName)
        {
            try
            {
                //oCSM = TisClientServicesModule.GetNewInstance(applicationName, stationName); 4.5
                
                // 5
                oCSM = SingletoneTisClientServicesModule.GetSingletoneInstance();
                
                oCSM.Initialize(null, null, null, 0, null, null, null, null, null, null, null);
                oCSM.LoginApplication(applicationName, stationName);
            }
            catch (Exception e)
            {
                throw new Exception(Constants.cStrLiteSdkBatch + " -> " + Constants.cStrFailedWithException, e);
            }
            finally
            {
                this.applicationName = applicationName;
                this.stationName = stationName;
            }
        }
        
        /// <summary>
        /// [Constructor] BatchInternals() --> Initializes a Batch object instance.
        /// </summary>
        /// <example><code>BatchInternals b = new BatchInternals();</code></example>
        protected BatchInternals()
        {
            this.applicationName = String.Empty;
            this.stationName = String.Empty;
        }

        #endregion "Constructor"

        #region "Destructor"
        /// <summary>
        /// [Destructor] BatchInternals() --> Releases unmanaged resources and performs other cleanup operations before the is reclaimed by garbage collection.
        /// </summary>
        /// <remarks>We must implement a finalizer to guarantee that the native CSM object is cleaned up</remarks>
        ~BatchInternals()
        {
            // Our finalizer should call our Dispose(bool) method with false
            this.Dispose(false);
        }
        #endregion "Destructor"

        #region "protected Dispose"
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// If the main class was marked as sealed, we could just make this a private void Dispose(bool).  Alternatively, we could (in this case) put
        /// all of our logic directly in Dispose().
        /// </remarks>
        public virtual void Dispose(bool disposing)
        {
            // Use our disposed flag to allow us to call this method multiple times safely.
            // This is a requirement when implementing IDisposable
            if (!this.disposed)
            {
                if (disposing)
                {
                    // If we have any managed, IDisposable resources, Dispose of them here.
                    // In this case, we don't, so this was unneeded.
                    // Later, we will subclass this class, and use this section.

                    if (collections != null) collections.Clear();

                    if (oCSM != null)
                        Logout();
                }

                // Always dispose of undisposed unmanaged resources in Dispose(bool)

            }
            // Mark us as disposed, to prevent multiple calls to dispose from having an effect, 
            // and to allow us to handle ObjectDisposedException
            this.disposed = true;
        }
        #endregion "protected Dispose"

        #region "Dispose"
        /// <summary>
        /// Batch.Dispose() --> Performs Batch defined tasks associated with freeing, releasing, or resetting managed and unmanaged resources.
        /// </summary>
        /// <example><code>b.Dispose();</code></example>
        public void Dispose()
        {
            // We start by calling Dispose(bool) with true
            this.Dispose(true);

            // Now suppress finalization for this object, since we've already handled our resource cleanup tasks
            GC.SuppressFinalize(this);
        }
        #endregion "Dispose"
        #endregion region "Constructor-Finalizer-Dispose"

        #region "Internal Private/Protected methods"

        #region "Batch->InternalHistory()"
        /// <summary>
        /// Batch->InternalHistory() --> Returns a list of the stations a collection has gone through (when processed normally through the workflow).
        /// </summary>
        /// <param name="Collection">Is a ITisCollectionData object.</param>
        /// <returns>Returns a string array if the collection has any history, otherwise returns an empty array.</returns>
        /// <example><code>string[] history = b.InternalHistory(collData);</code></example>
        protected string[] InternalHistory(ITisCollectionData Collection)
        {
            List<string> result = new List<string>();

            if (Collection != null)
            {
                if (CheckLogon())
                {
                    try
                    {
                        if (Collection.NumberOfHistoryTags > 0)
                            for (short j = 0; j < Collection.NumberOfHistoryTags; j++)
                                //result.Add(Collection.get_HistoryDataTag(j).Name); 4.5
                                result.Add(Collection.GetHistoryDataTag(j).Name); 
                    }
                    catch (Exception e)
                    {
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrFailedWithException +
                            ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()), Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    }
                }
                else
                    if (oCSM != null)
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrCouldNotLogin,
                            Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrCouldNotLogin);
            }
            else
                if (oCSM != null)
                    oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrCollectionIsNull, Constants.cStrLiteSdk,
                        Log.Severity.ERROR, 0, 0);

            return result.ToArray();
        }
        #endregion "Batch->InternalHistory()"

        #region "Batch.CoreHistory()"
        /// <summary>
        /// [Static] Batch.CoreHistory() --> Used by the InternalHistory functions.
        /// </summary>
        private static void CoreHistory(ref List<string> result, ref ITisClientServicesModule csm, string CollectionName)
        {
            ITisCollectionData Collection = csm.Dynamic.GetSpecificCollection(CollectionName, false);

            if (csm != null)
            {
                if (Collection.NumberOfHistoryTags > 0)
                    for (short j = 0; j < Collection.NumberOfHistoryTags; j++)
                        result.Add(Collection.GetHistoryDataTag(j).Name);
            }

            csm.Dynamic.FreeSpecificCollection(Collection, true);
        }
        #endregion "Batch.CoreHistory()"

        #region "Batch.InternalHistory()"
        /// <summary>
        /// [Static] Batch.InternalHistory() --> Returns a list of the stations a collection has gone through (when processed normally through the workflow).
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name.</param>
        /// <param name="CollectionName">Indicates the collection name to be checked.</param>
        /// <example><code>string[] history = Batch.InternalHistory("SimpleDemo", "Completion", "000002349_0001");</code></example>
        protected static string[] InternalHistory(string applicationName, string stationName, string CollectionName)
        {
            List<string> result = new List<string>();
            
            ITisClientServicesModule csm = null;

            if (applicationName != String.Empty && stationName != String.Empty)
            {
                try
                {
                    csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                    if (CollectionName != String.Empty)
                        CoreHistory(ref result, ref csm, CollectionName);
                }
                catch (Exception e)
                {
                    if (csm != null)
                        csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrFailedWithException + e,
                            Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrFailedWithException + e);
                }
                finally
                {
                    if (csm != null) csm.Dispose();
                }
            }

            return result.ToArray();
        }
        #endregion "Batch.InternalHistory()"

        #region "Batch.InternalHistoryMany()"
        /// <summary>
        /// [Static] Batch.InternalHistoryMany() --> Returns a list of the stations, the collections have gone through (when processed normally through the workflow).
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to log on to the CSM.</param>
        /// <param name="CollectionNames">Indicates the collection names to be checked.</param>
        /// <returns>Returns a multidimensional string array if the collections have any history, otherwise returns an empty sub array per collection.</returns>
        /// <example><code>string[][] history = Batch.HistoryMany("SimpleDemo", "Completion", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        protected static string[][] InternalHistoryMany(string applicationName, string stationName, string[] CollectionNames)
        {
            List<string[]> result = new List<string[]>();

            ITisClientServicesModule csm = null;

            if (applicationName != String.Empty && stationName != String.Empty)
            {
                if (CollectionNames != null && CollectionNames.Length > 0)
                {
                    try
                    {
                        csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                        foreach (string CollectionName in CollectionNames)
                        {
                            if (CollectionName != String.Empty)
                            {
                                List<string> subresult = new List<string>();

                                CoreHistory(ref subresult, ref csm, CollectionName);

                                result.Add(subresult.ToArray());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (csm != null)
                            csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrFailedWithException + e,
                                Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                        else
                            throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrHistoryMethod + Constants.cStrFailedWithException + e);
                    }
                    finally
                    {
                        if (csm != null) csm.Dispose();
                    }
                }
            }

            return result.ToArray();
        }
        #endregion "Batch.InternalHistoryMany()"

        #region "Batch.ReturnPriorityLevels()"
        /// <summary>
        /// [Static] Batch.ReturnPriorityLevels() --> Returns the WorkflowPriorityLevel and AbsolutePriority values given the Priority value.
        /// </summary>
        /// <param name="priority">The Priority value.</param>
        /// <param name="wfp">The equivalent WorkflowPriorityLevel for the given Priority value.</param>
        /// <param name="abs">The equivalent AbsolutePriority for the given Priority value.</param>
        protected static void ReturnPriorityLevels(Priority priority, ref WorkflowPriorityLevel wfp, ref double abs)
        {
            wfp = WorkflowPriorityLevel.Normal;
            abs = 100.0;

            switch (priority)
            {
                case Priority.Low:
                    wfp = WorkflowPriorityLevel.Low;
                    abs = 0.0;
                    break;

                case Priority.High:
                    wfp = WorkflowPriorityLevel.High;
                    abs = 1000000.0;
                    break;

                case Priority.AboveNormal:
                    wfp = WorkflowPriorityLevel.AboveNormal;
                    abs = 10000.0;
                    break;
            }
        }
        #endregion "Batch.ReturnPriorityLevels()"

        #region "Batch->CheckLogon()"
        /// <summary>
        /// Batch->CheckLogon() --> Returns true if the oCSM, ApplicationName and StationName parameters are set correctly.
        /// </summary>
        protected bool CheckLogon()
        {
            return (oCSM != null && this.applicationName != String.Empty && this.stationName != String.Empty) ? true : false;
        }
        #endregion "Batch->CheckLogon()"

        #region "Batch->InternalPut()"
        /// <summary>
        /// Batch->InternalPut() --> Used by Put, Reject or Free collection.
        /// </summary>
        /// <param name="Collection">Is a ITisCollectionData object.</param>
        /// <param name="put">A boolean - if true, indicates a Put collection operation, if false indicates a free collection operation.</param>
        /// <param name="method">The name of the method, either a Put, Reject or Free collection.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        protected bool InternalPut(ITisCollectionData Collection, bool put, string method)
        {
            bool result = false;

            if (Collection != null)
            {
                if (CheckLogon())
                {
                    try
                    {
                        if (method == Constants.cStrPutMethod)
                            //oCSM.Dynamic.set_StampAsFinalByObject(Collection, put); 4.5
                            oCSM.Dynamic.SetStampAsFinalByObject(Collection, put);
                        else
                        {
                            if (method == Constants.cStrDeleteMethod)
                                oCSM.Dynamic.RemoveSpecificCollection(Collection);

                            else if (method == Constants.cStrRejectMethod)
                                oCSM.Dynamic.SetStampAsRejectByObject(Collection, true);
                        }

                        oCSM.Dynamic.FreeSpecificCollection(Collection, put);
                    }
                    catch (Exception e)
                    {
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + method + Constants.cStrFailedWithException +
                            ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()), Constants.cStrLiteSdk,
                            Log.Severity.ERROR, 0, 0);
                    }
                    finally
                    {
                        result = true;

                        if (collections != null)
                            if (collections.IndexOf(Collection.Name) >= 0)
                                collections.Remove(Collection.Name);
                    }
                }
                else
                    if (oCSM != null)
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + method + Constants.cStrCouldNotLogin,
                            Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + method + Constants.cStrCouldNotLogin);
            }
            else
                if (oCSM != null)
                    oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + method + Constants.cStrCollectionIsNull, Constants.cStrLiteSdk,
                        Log.Severity.ERROR, 0, 0);

            return result;
        }
        #endregion "Batch->InternalPut()"

        #region "Batch.InternalUnlockSpecificCollection()"
        /// <summary>
        /// [Static] Batch.InternalUnlockSpecificCollection() --> Unlocks a specific collection given the collection's name.
        /// </summary>
        /// <param name="CollectionName">Indicates the collection name to unlock.</param>
        /// <param name="connectionString">The connection string to the workflow DB.</param>
        /// <returns>Returns 1 if successful, otherwise 0 or -1.</returns>
        /// <example><code>Batch.InternalUnlockSpecificCollection("000002349_0001", "Data Source=RADKIDDO-PC\EFLOW;Initial Catalog=SwitchDemo_Workflow;Integrated Security=True");</code></example>
        protected static int InternalUnlockSpecificCollection(string CollectionName, string connectionString)
        {
            int result = -1;

            try
            {
                if (CollectionName != String.Empty)
                {
                    result = CoreUnlockSpecificCollection(CollectionName, connectionString, result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrUnlockSpecificCollection + Constants.cStrFailedWithException + ex);
            }

            return result;
        }

        protected static int CoreUnlockSpecificCollection(string CollectionName, string connectionString, int result)
        {
            SqlConnection mySqlConnection = new SqlConnection(connectionString);

            string updateString = "UPDATE E_Unit SET QueueStatus = 1 WHERE (QueueStatus <> 1) AND (Name = '" + CollectionName + "');";

            SqlCommand cmdUpdate = new SqlCommand(updateString);

            mySqlConnection.Open();
            cmdUpdate.Connection = mySqlConnection;

            result = cmdUpdate.ExecuteNonQuery();
            return result;
        }
        #endregion "Batch.InternalUnlockSpecificCollection()"

        #region "Batch.CorePut()"
        /// <summary>
        /// [Static] Batch.CorePut() --> Core / heart of Put, Reject or Free collection.
        /// </summary>
        private static void CorePut(ref ITisClientServicesModule csm, string CollectionName, string method, bool put)
        {
            ITisCollectionData Collection = csm.Dynamic.GetSpecificCollection(CollectionName, false);

            if (csm != null)
            {
                if (method == Constants.cStrPutMethod)
                    csm.Dynamic.SetStampAsFinalByObject(Collection, put);
                else
                {
                    if (method == Constants.cStrDeleteMethod)
                        csm.Dynamic.RemoveSpecificCollection(Collection);

                    else if (method == Constants.cStrRejectMethod)
                        csm.Dynamic.SetStampAsRejectByObject(Collection, true);
                }

                try
                {
                    if (method != Constants.cStrDeleteMethod)
                        csm.Dynamic.FreeSpecificCollection(Collection, put);
                }
                catch { }
            }
        }
        #endregion "Batch.CorePut()"

        #region "Batch.InternalPut()"
        /// <summary>
        /// [Static] Batch.InternalPut() --> Used by Put, Reject or Free collection.
        /// </summary>
        /// <param name="CollectionName">Indicates the collection name.</param>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name.</param>
        /// <param name="put">A boolean - if true, indicates a Put collection operation, if false indicates a free collection operation.</param>
        /// <param name="method">The name of the method, either a Put, Reject or Free collection.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        protected static bool InternalPut(string applicationName, string stationName, string CollectionName, bool put, string method)
        {
            bool result = false;

            ITisClientServicesModule csm = null;

            if (applicationName != String.Empty && stationName != String.Empty)
            {
                try
                {
                    csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                    if (CollectionName != String.Empty)
                        CorePut(ref csm, CollectionName, method, put);
                }
                catch (Exception e)
                {
                    if (csm != null)
                        csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + method + Constants.cStrFailedWithException + e,
                            Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + method + Constants.cStrFailedWithException + e);
                }
                finally
                {
                    if (csm != null) csm.Dispose();
                    result = true;
                }
            }

            return result;
        }
        #endregion "Batch.InternalPut()"

        #region "Batch.InternalPutMany()"
        /// <summary>
        /// [Static] Batch.InternalPutMany() --> Used by Put, Reject or Free collection.
        /// </summary>
        /// <param name="CollectionNames">Indicates the collection names to process.</param>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to log on to the CSM.</param>
        /// <param name="put">A boolean - if true, indicates a Put collection operation, if false indicates a free collection operation.</param>
        /// <param name="method">The name of the method, either a Put, Reject or Free collection.</param>
        /// <returns>Returns true is successful (no exceptions occur), otherwise false.</returns>
        protected static bool InternalPutMany(string applicationName, string stationName, string[] CollectionNames, bool put, string method)
        {
            bool result = false;

            ITisClientServicesModule csm = null;

            if (applicationName != String.Empty && stationName != String.Empty)
            {
                if (CollectionNames != null && CollectionNames.Length > 0)
                {
                    try
                    {
                        csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                        foreach (string CollectionName in CollectionNames)
                            if (CollectionName != String.Empty)
                                CorePut(ref csm, CollectionName, method, put);
                    }
                    catch (Exception e)
                    {
                        if (csm != null)
                            csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + method + Constants.cStrFailedWithException + e,
                                Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                        else
                            throw new Exception(Constants.cStrLiteSdkBatch + method + Constants.cStrFailedWithException + e);
                    }
                    finally
                    {
                        if (csm != null) csm.Dispose();
                        result = true;
                    }
                }
            }

            return result;
        }
        #endregion "Batch.InternalPutMany()"

        #region "Batch.CoreInternalPrioritizeHold"
        /// <summary>
        /// [Static] Batch.CoreInternalPrioritizeHold() --> Core: Places a collection on Hold.
        /// </summary>
        private static void CoreInternalPrioritizeHold(ref ITisClientServicesModule csm, string stationName, string toStationName, string CollectionName,
            Priority priority, string operation, bool hold)
        {
            if (operation == Constants.cStrPrioritizeMethod)
            {
                WorkflowPriorityLevel wfp = WorkflowPriorityLevel.Normal;
                double abs = 100.0;

                ReturnPriorityLevels(priority, ref wfp, ref abs);

                csm.DynamicManage.SetCollectionPriorityLevelByName(stationName, CollectionName, wfp);
            }
            else if (operation == Constants.cStrHoldMethod)
                csm.DynamicManage.SetCollectionHoldStateByName(CollectionName, hold);

            else if (operation == Constants.cStrMoveMethod)
                csm.DynamicManage.MoveCollectionToQueueByName(CollectionName, toStationName);
        }
        #endregion "Batch.CoreInternalPrioritizeHold"

        #region "Batch.InternalPrioritizeHold()"
        /// <summary>
        /// [Static] Batch.InternalPrioritizeHold() --> Places a collection on Hold.
        /// </summary>
        /// <param name="CollectionName">Indicates the collection name.</param>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name.</param>
        /// <param name="toStationName">Indicates the station name where to move the collection.</param>
        /// <param name="priority">Is a Priority value, which indicates the collection's priority.</param>
        /// <param name="operation">Type of operation to execute: Hold / Prioritize.</param>
        /// <param name="hold">True indicates on Hold.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.InternalPrioritizeHold("SimpleDemo", "Completion", "", "000002349_0001", Batch.Priority.High, Constants.cStrHoldMethod, true);</code></example>
        protected static bool InternalPrioritizeHold(string applicationName, string stationName, string toStationName, string CollectionName,
            Priority priority, string operation, bool hold)
        {
            bool result = false;

            ITisClientServicesModule csm = null;

            if (applicationName != String.Empty && stationName != String.Empty)
            {
                try
                {
                    csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                    if (csm != null)
                        if (CollectionName != String.Empty)
                            CoreInternalPrioritizeHold(ref csm, stationName, toStationName, CollectionName, priority, operation, hold); 
                }
                catch (Exception e)
                {
                    if (csm != null)
                        csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrFailedWithException + e,
                            Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + operation + Constants.cStrFailedWithException + e);
                }
                finally
                {
                    if (csm != null) csm.Dispose();
                    result = true;
                }
            }

            return result;
        }
        #endregion "Batch.InternalPrioritizeHold()"

        #region "Batch.InternalPrioritizeHoldMany()"
        /// <summary>
        /// [Static] Batch.InternalPrioritizeHoldMany() --> Places collections on Hold.
        /// </summary>
        /// <param name="CollectionNames">Indicates the collection names.</param>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name.</param>
        /// <param name="toStationName">Indicates the station name where to move the collection.</param>
        /// <param name="priority">Is a Priority value, which indicates the collection's priority.</param>
        /// <param name="operation">Type of operation to execute: Hold / Prioritize.</param>
        /// <param name="hold">True indicates on Hold.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.InternalPrioritizeHoldMany("SimpleDemo", "Completion", "", new string[] { "000002349_0001", "000002349_0002" }, Batch.Priority.High, Constants.cStrHoldMethod, true);</code></example>
        protected static bool InternalPrioritizeHoldMany(string applicationName, string stationName, string toStationName, string[] CollectionNames,
            Priority priority, string operation, bool hold)
        {
            bool result = false;

            ITisClientServicesModule csm = null;

            if (applicationName != String.Empty && stationName != String.Empty)
            {
                if (CollectionNames != null && CollectionNames.Length > 0)
                {
                    try
                    {
                        csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                        if (csm != null)
                        {
                            foreach (string CollectionName in CollectionNames)
                            {
                                if (CollectionName != String.Empty)
                                    CoreInternalPrioritizeHold(ref csm, stationName, toStationName, CollectionName, priority, operation, hold);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (csm != null)
                            csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrFailedWithException + e,
                                Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                        else
                            throw new Exception(Constants.cStrLiteSdkBatch + operation + Constants.cStrFailedWithException + e);
                    }
                    finally
                    {
                        if (csm != null) csm.Dispose();
                        result = true;
                    }
                }
            }

            return result;
        }
        #endregion "Batch.InternalPrioritizeHoldMany()"

        #region "Batch->CoreInternalPrioritizeHold()"
        /// <summary>
        /// Batch->CoreInternalPrioritizeHold() --> Core: Places a collection on Hold.
        /// </summary>
        private void CoreInternalPrioritizeHold(string CollectionName, Priority priority, string operation, bool hold, string toStationName)
        {
            if (operation == Constants.cStrPrioritizeMethod)
            {
                WorkflowPriorityLevel wfp = WorkflowPriorityLevel.Normal;
                double abs = 100.0;

                Batch.ReturnPriorityLevels(priority, ref wfp, ref abs);

                oCSM.DynamicManage.SetCollectionPriorityLevelByName(stationName, CollectionName, wfp);
            }
            else if (operation == Constants.cStrHoldMethod)
                oCSM.DynamicManage.SetCollectionHoldStateByName(CollectionName, hold);

            else if (operation == Constants.cStrMoveMethod)
                oCSM.DynamicManage.MoveCollectionToQueueByName(CollectionName, toStationName);
        }
        #endregion "Batch->CoreInternalPrioritizeHold()"

        #region "Batch->InternalPrioritizeHold()"
        /// <summary>
        /// Batch->InternalPrioritizeHold() --> Places a collection on Hold.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionName">Indicate the collection name to prioritize.</param>
        /// <param name="priority">Is a Priority value, which indicates the collection's priority.</param>
        /// <param name="operation">Type of operation to execute: Hold / Prioritize.</param>
        /// <param name="hold">True indicates on Hold.</param>
        /// <param name="toStationName">Indicates the station name where to move the collection.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.InternalPrioritizeHold("000002349_0001", Batch.Priority.High, Constants.cStrHoldMethod, true, "");</code></example>
        protected bool InternalPrioritizeHold(string CollectionName, Priority priority, string operation, bool hold, string toStationName)
        {
            bool result = false;

            if (CollectionName != String.Empty)
            {
                if (CheckLogon())
                {
                    try
                    {
                        CoreInternalPrioritizeHold(CollectionName, priority, operation, hold, toStationName);
                    }
                    catch (Exception e)
                    {
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrFailedWithException +
                            ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()), Constants.cStrLiteSdk,
                            Log.Severity.ERROR, 0, 0);
                    }
                    finally
                    {
                        result = true;
                    }
                }
                else
                    if (oCSM != null)
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrCouldNotLogin,
                            Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + operation + Constants.cStrCouldNotLogin);
            }
            else
                if (oCSM != null)
                    oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrCollectionNameCannotBeEmpty, Constants.cStrLiteSdk,
                        Log.Severity.ERROR, 0, 0);

            return result;
        }
        #endregion "Batch->InternalPrioritizeHold()"

        #region "Batch->InternalPrioritizeHoldMany()"
        /// <summary>
        /// Batch->InternalPrioritizeHoldMany() --> Places collections on Hold.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionNames">Indicate the collection names to prioritize.</param>
        /// <param name="priority">Is a Priority value, which indicates the collection's priority.</param>
        /// <param name="operation">Type of operation to execute: Hold / Prioritize.</param>
        /// <param name="hold">True indicates on Hold.</param>
        /// <param name="toStationName">Indicates the station name where to move the collection.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.InternalPrioritizeHoldMany(new string[] { "000002349_0001", "000002349_0002" }, Batch.Priority.High, Constants.cStrHoldMethod, true, "");</code></example>
        protected bool InternalPrioritizeHoldMany(string[] CollectionNames, Priority priority, string operation, bool hold, string toStationName)
        {
            bool result = false;

            if (CollectionNames != null && CollectionNames.Length > 0)
            {
                if (CheckLogon())
                {
                    try
                    {
                        foreach (string CollectionName in CollectionNames)
                            if (CollectionName != String.Empty)
                                CoreInternalPrioritizeHold(CollectionName, priority, operation, hold, toStationName);
                    }
                    catch (Exception e)
                    {
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrFailedWithException +
                            ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()), Constants.cStrLiteSdk,
                            Log.Severity.ERROR, 0, 0);
                    }
                    finally
                    {
                        result = true;
                    }
                }
                else
                    if (oCSM != null)
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrCouldNotLogin,
                            Constants.cStrLiteSdk, Log.Severity.ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + operation + Constants.cStrCouldNotLogin);
            }
            else
                if (oCSM != null)
                    oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + operation + Constants.cStrCollectionNameCannotBeEmpty, Constants.cStrLiteSdk,
                        Log.Severity.ERROR, 0, 0);

            return result;
        }
        #endregion "Batch->InternalPrioritizeHoldMany()"

        #region "Batch.CoreFromDynamic"
        /// <summary>
        /// Batch->CoreFromDynamic() --> Core: Imports collection(s) given by a file name.
        /// </summary>
        protected static void CoreFromDynamic(ref List<ITisCollectionData> result, ref ITisClientServicesModule csm, string filename)
        {
            List<ITisCollectionData> temp = new List<ITisCollectionData>();

            try
            {
                UnitId[] CollectionInfo = csm.DynamicImportExport.ImportCollectionsArchive(filename);

                if (CollectionInfo != null && CollectionInfo.Length > 0)
                    foreach (UnitId cInfo in CollectionInfo) 
                    { 
                        WFUnitInfo wf = new WFUnitInfo(); // 5
                        wf.UnitId = cInfo.Id; // 5
                        //temp.Add(csm.DynamicDirectRead.AccessByWFID(cInfo.Id)); // 4.5
                        temp.Add(csm.DynamicDirectRead.AccessByWFID(wf)); // 5
                    }
            }
            catch (Exception e)
            {
                csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrFromDynamicMethod + Constants.cStrFailedWithException +
                    ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()), Constants.cStrLiteSdk,
                    Log.Severity.ERROR, 0, 0);
            }
            finally
            {
                result = temp ?? null;

                if (result != null)
                    if (collections == null || collections.Count == 0)
                    {
                        if (collections == null) collections = new List<string>();

                        foreach (ITisCollectionData d in result)
                            if (collections.IndexOf(d.Name) < 0)
                                collections.Add(d.Name);
                    }
            }
        }
        #endregion "Batch.CoreFromDynamic"

        #endregion "Internal Private/Protected methods"

        #region "External methods"

        #region "Batch->Logout()"
        /// <summary>
        /// Batch->Logout() --> Logs out from the CSM.
        /// </summary>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Logout();</code></example>
        public virtual bool Logout()
        {
            bool result = false;

            if (oCSM != null)
            {
                try
                {
                    if (oCSM.SetupTransaction.InTransaction) oCSM.SetupTransaction.RollbackTransaction();

                    oCSM.Dispose();
                    oCSM = null;
                }
                catch (Exception e)
                {
                    oCSM = null;

                    throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrLoginMethod + Constants.cStrFailedWithException +
                        ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()));
                }
                finally
                {
                    result = true;

                    if (collections != null) collections.Clear();
                }
            }

            return result;
        }
        #endregion "Batch->Logout()"

        #endregion "External methods"
    }
}

#endregion "namespace eFlow.CollectionManagement"
