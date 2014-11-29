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
using System.IO;

using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;
using TiS.Core.PlatformRuntime.Customizations;
using TiS.TisCommon;
using TiS.Core.Workflow.WFCommon;

#endregion "using"

#region "namespace eFlow.CollectionManagement"

namespace eFlow.CollectionManagement
{
    #region "class Batch"

    /// <summary>
    /// "Batch" Class --> Is a wrapper class that performs atomic operations around a ITisCollectionData object. 
    /// </summary>
    public class Batch : BatchInternals, IDisposable
    {
        #region "Constructor-Finalizer-Dispose"

        #region "Constructor"
        /// <summary>
        /// [Constructor] Batch() --> Initializes a Batch object instance with the eFLOW application and station names.
        /// </summary>
        /// <param name="applicationName">Indicates the name of the eFLOW application to login to.</param>
        /// <param name="stationName">Indicates the name of the eFLOW station to login to.</param>
        /// <example><code>Batch b = new Batch("SimpleDemo", "Completion");</code></example>
        public Batch (string applicationName, string stationName) : base(applicationName, stationName)
        {
        }
        
        /// <summary>
        /// [Constructor] Batch() --> Initializes a Batch object instance.
        /// </summary>
        /// <example><code>Batch b = new Batch();</code></example>
        public Batch() : base()
        {
        }

        #endregion "Constructor"

        #region "Destructor"
        /// <summary>
        /// [Destructor] Batch() --> Releases unmanaged resources and performs other cleanup operations before the is reclaimed by garbage collection.
        /// </summary>
        /// <remarks>We must implement a finalizer to guarantee that the native CSM object is cleaned up</remarks>
        ~Batch()
        {
            // Our finalizer should call our Dispose(bool) method with false
            base.Dispose(false);
        }
        #endregion "Destructor"

        #endregion region "Constructor-Finalizer-Dispose"

        #region "Exposed/Public methods"

        #region "Batch->History()"
        /// <summary>
        /// Batch->History() --> Returns a list of the stations a collection has gone through (when processed normally through the workflow).
        /// </summary>
        /// <param name="Collection">Is a ITisCollectionData object.</param>
        /// <returns>Returns a string array if the collection has any history, otherwise returns an empty array.</returns>
        /// <example><code>string[] history = b.History(collData);</code></example>
        public string[] History(ITisCollectionData Collection)
        {
            return InternalHistory(Collection);
        }
        #endregion "Batch->History()"

        #region "Batch.History()"
        /// <summary>
        /// [Static] Batch.History() --> Returns a list of the stations a collection has gone through (when processed normally through the workflow).
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM. It must be the same station where you intend to get the collection's history from.</param>
        /// <param name="CollectionName">Indicates the collection name to be checked.</param>
        /// <returns>Returns a string array if the collection has any history, otherwise returns an empty array.</returns>
        /// <example><code>string[] history = Batch.History("SimpleDemo", "Completion", "000002349_0001");</code></example>
        public static string[] History(string applicationName, string stationName, string CollectionName)
        {
            return InternalHistory(applicationName, stationName, CollectionName);
        }
        #endregion "Batch.History()"

        // Many
        #region "Batch.HistoryMany()"
        /// <summary>
        /// [Static] Batch.HistoryMany() --> Returns a list of the stations, the collections have gone through (when processed normally through the workflow).
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM. It must be the same station where you intend to get the collections history from.</param>
        /// <param name="CollectionNames">Indicates the collection names to be checked.</param>
        /// <returns>Returns a multidimensional string array if the collections have any history, otherwise returns an empty sub array per collection.</returns>
        /// <example><code>string[][] history = Batch.HistoryMany("SimpleDemo", "Completion", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public static string[][] HistoryMany(string applicationName, string stationName, string[] CollectionNames)
        {
            return InternalHistoryMany(applicationName, stationName, CollectionNames);
        }
        #endregion "Batch.HistoryMany()"

        #region "Batch->Release()"
        /// <summary>
        /// Batch->Release() --> Releases a collection that is on Hold.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionName">Indicate the collection name to be released.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.Release("000002349_0001");</code></example>
        public bool Release(string CollectionName)
        {
            return InternalPrioritizeHold(CollectionName, Batch.Priority.Normal, Constants.cStrHoldMethod, false, "");
        }
        #endregion "Batch->Release()"

        // Many
        #region "Batch->ReleaseMany()"
        /// <summary>
        /// Batch->ReleaseMany() --> Releases collections that are on Hold.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionNames">Indicate the collection names to be released.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.ReleaseMany(new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public bool ReleaseMany(string[] CollectionNames)
        {
            return InternalPrioritizeHoldMany(CollectionNames, Batch.Priority.Normal, Constants.cStrHoldMethod, false, "");
        }
        #endregion "Batch->ReleaseMany()"

        #region "Batch.Release()"
        /// <summary>
        /// [Static] Batch.Release() --> Releases a collection that is on Hold.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name.</param>
        /// <param name="CollectionName">Indicates the collection name to be released.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.Release("SimpleDemo", "Completion", "000002349_0001");</code></example>
        public static bool Release(string applicationName, string stationName, string CollectionName)
        {
            return InternalPrioritizeHold(applicationName, stationName, "", CollectionName, Batch.Priority.Normal, Constants.cStrHoldMethod, false);
        }
        #endregion "Batch.Release()"

        // Many
        #region "Batch.ReleaseMany()"
        /// <summary>
        /// [Static] Batch.ReleaseMany() --> Releases collections that are on Hold.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name.</param>
        /// <param name="CollectionNames">Indicates the collection names to be released.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.ReleaseMany("SimpleDemo", "Completion", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public static bool ReleaseMany(string applicationName, string stationName, string[] CollectionNames)
        {
            return InternalPrioritizeHoldMany(applicationName, stationName, "", CollectionNames, Batch.Priority.Normal, Constants.cStrHoldMethod, false);
        }
        #endregion "Batch.ReleaseMany()"

        #region "Batch->Move()"
        /// <summary>
        /// Batch->Move() --> Moves a collection to another station.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionName">Indicate the collection name to be moved.</param>
        /// <param name="toStationName">Indicates the station name where they collection will be moved to.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.Move("000002349_0001", "Pending");</code></example>
        public bool Move(string CollectionName, string toStationName)
        {
            return InternalPrioritizeHold(CollectionName, Batch.Priority.Normal, Constants.cStrMoveMethod, false, toStationName);
        }
        #endregion "Batch->Move()"

        // Many
        #region "Batch->MoveMany()"
        /// <summary>
        /// Batch->MoveMany() --> Moves collections to another station.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionNames">Indicate the collection names to be moved.</param>
        /// <param name="toStationName">Indicates the station name where they collections will be moved to.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.MoveMany(new string[] { "000002349_0001", "000002349_0002" }, "Pending");</code></example>
        public bool MoveMany(string[] CollectionNames, string toStationName)
        {
            return InternalPrioritizeHoldMany(CollectionNames, Batch.Priority.Normal, Constants.cStrMoveMethod, false, toStationName);
        }
        #endregion "Batch->MoveMany()"

        #region "Batch.Move()"
        /// <summary>
        /// [Static] Batch.Move() --> Moves a collection to another station.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name used to log on to the CSM.</param>
        /// <param name="toStationName">Indicates the station name where they collection will be moved to.</param>
        /// <param name="CollectionName">Indicates the collection name to be moved.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.Move("SimpleDemo", "Completion", "Pending", "000002349_0001");</code></example>
        public static bool Move(string applicationName, string stationName, string toStationName, string CollectionName)
        {
            return InternalPrioritizeHold(applicationName, stationName, toStationName, CollectionName, Batch.Priority.Normal,
                Constants.cStrMoveMethod, false); 
        }
        #endregion "Batch.Move()"

        // Many
        #region "Batch.MoveMany()"
        /// <summary>
        /// [Static] Batch.MoveMany() --> Moves collections to another station.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name used to log on to the CSM.</param>
        /// <param name="toStationName">Indicates the station name where they collections will be moved to.</param>
        /// <param name="CollectionNames">Indicates the collection names to be moved.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.MoveMany("SimpleDemo", "Completion", "Pending", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public static bool MoveMany(string applicationName, string stationName, string toStationName, string[] CollectionNames)
        {
            return InternalPrioritizeHoldMany(applicationName, stationName, toStationName, CollectionNames, Batch.Priority.Normal,
                Constants.cStrMoveMethod, false);
        }
        #endregion "Batch.MoveMany()"

        #region "Batch->Hold()"
        /// <summary>
        /// Batch->Hold() --> Places a collection on Hold.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionName">Indicate the collection name to set on hold.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.Hold("000002349_0001");</code></example>
        public bool Hold(string CollectionName)
        {
            return InternalPrioritizeHold(CollectionName, Batch.Priority.Normal, Constants.cStrHoldMethod, true, "");
        }
        #endregion "Batch->Hold()"

        // Many
        #region "Batch->HoldMany()"
        /// <summary>
        /// Batch->HoldMany() --> Places collections on Hold.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionNames">Indicate the collection names to be set on hold.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.HoldMany(new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public bool HoldMany(string[] CollectionNames)
        {
            return InternalPrioritizeHoldMany(CollectionNames, Batch.Priority.Normal, Constants.cStrHoldMethod, true, "");
        }
        #endregion "Batch->HoldMany()"

        #region "Batch.Hold()"
        /// <summary>
        /// [Static] Batch.Hold() --> Places a collection on Hold.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to log on to the CSM.</param>
        /// <param name="CollectionName">Indicates the collection name to set on hold.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.Hold("SimpleDemo", "Completion", "000002349_0001");</code></example>
        public static bool Hold(string applicationName, string stationName, string CollectionName)
        {
            return InternalPrioritizeHold(applicationName, stationName, "", CollectionName, Batch.Priority.Normal, Constants.cStrHoldMethod, true); 
        }
        #endregion "Batch.Hold()"

        // Many
        #region "Batch.HoldMany()"
        /// <summary>
        /// [Static] Batch.HoldMany() --> Places collections on Hold.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to log on to the CSM.</param>
        /// <param name="CollectionNames">Indicates the collection names to be set on hold.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.HoldMany("SimpleDemo", "Completion", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public static bool HoldMany(string applicationName, string stationName, string[] CollectionNames)
        {
            return InternalPrioritizeHoldMany(applicationName, stationName, "", CollectionNames, Batch.Priority.Normal, Constants.cStrHoldMethod, true);
        }
        #endregion "Batch.HoldMany()"

        #region "Batch->Prioritize()"
        /// <summary>
        /// Batch->Prioritize() --> Prioritizes a collection.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionName">Indicate the collection name to prioritize - a collection on the logged on station.</param>
        /// <param name="priority">Is a Priority value, which indicates the collection's priority.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.Prioritize("000002349_0001", Batch.Priority.AboveNormal);</code></example>
        public bool Prioritize(string CollectionName, Priority priority)
        {
            return InternalPrioritizeHold(CollectionName, priority, Constants.cStrPrioritizeMethod, false, "");
        }
        #endregion "Batch->Prioritize()"

        // Many
        #region "Batch->PrioritizeMany()"
        /// <summary>
        /// Batch->PrioritizeMany() --> Prioritizes collections.
        /// </summary>
        /// <remarks>Cannot be executed on a batch that has a Get() on it (a locked batch).</remarks>
        /// <param name="CollectionNames">Indicate the collection names to prioritize - collections on the logged on station.</param>
        /// <param name="priority">Is a Priority value, which indicates the collection's priority.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>b.PrioritizeMany(new string[] { "000002349_0001", "000002349_0002" }, Batch.Priority.AboveNormal);</code></example>
        public bool PrioritizeMany(string[] CollectionNames, Priority priority)
        {
            return InternalPrioritizeHoldMany(CollectionNames, priority, Constants.cStrPrioritizeMethod, false, "");
        }
        #endregion "Batch->PrioritizeMany()"

        #region "Batch.Prioritize()"
        /// <summary>
        /// [Static] Batch.Prioritize() --> Prioritizes a collection.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name where the collection will be prioritized.</param>
        /// <param name="CollectionName">Indicates the collection name.</param>
        /// <param name="priority">Is a Priority value, which indicates the collection's priority.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.Prioritize("SimpleDemo", "Completion", "000002349_0001", Batch.Priority.High);</code></example>
        public static bool Prioritize(string applicationName, string stationName, string CollectionName, Priority priority)
        {
            return InternalPrioritizeHold(applicationName, stationName, "", CollectionName, priority, Constants.cStrPrioritizeMethod, false);
        }
        #endregion "Batch.Prioritize()"

        // Many 
        #region "Batch.PrioritizeMany()"
        /// <summary>
        /// [Static] Batch.PrioritizeMany() --> Prioritizes collections.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name where the collections will be prioritized.</param>
        /// <param name="CollectionNames">Indicates the collection names.</param>
        /// <param name="priority">Is a Priority value, which indicates the collections priority.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.PrioritizeMany("SimpleDemo", "Completion", new string[] { "000002349_0001", "000002349_0002" }, Batch.Priority.High);</code></example>
        public static bool PrioritizeMany(string applicationName, string stationName, string[] CollectionNames, Priority priority)
        {
            return InternalPrioritizeHoldMany(applicationName, stationName, "", CollectionNames, priority, Constants.cStrPrioritizeMethod, false);
        }
        #endregion "Batch.PrioritizeMany()"

        // Many
        #region "Batch->FromDynamic()"
        /// <summary>
        /// Batch->FromDynamic() --> Imports collection(s) given by a file name.
        /// </summary>
        /// <param name="filename">Represents the file name from where the collection(s) will be imported.</param>
        /// <returns>Returns a ITisCollectionData array if successful, otherwise null.</returns>
        /// <example><code>ITisCollectionData[] collDatas = b.FromDynamic(@"c:\temp\test.dynamic");</code></example>
        public ITisCollectionData[] FromDynamic(string filename)
        {
            List<ITisCollectionData> result = new List<ITisCollectionData>();

            if (filename != String.Empty && File.Exists(filename))
            {
                if (CheckLogon())
                {
                    Batch.CoreFromDynamic(ref result, ref oCSM, filename); 
                }
                else
                    if (oCSM != null)
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrFromDynamicMethod + Constants.cStrCouldNotLogin, Constants.cStrLiteSdk,
                            TIS_SEVERITY.TIS_ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrFromDynamicMethod + Constants.cStrCouldNotLogin);
            }
            else
                if (oCSM != null)
                    oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrFromDynamicMethod + Constants.cStrFileNameNotFound + filename + "']",
                        Constants.cStrLiteSdk, TIS_SEVERITY.TIS_ERROR, 0, 0);

            return result.ToArray();
        }
        #endregion "Batch->FromDynamic()"

        // Many
        #region "Batch.FromDynamic"
        /// <summary>
        /// [Static] Batch.FromDynamic() --> Imports collection(s) given by a file name.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM.</param>
        /// <param name="filename">Represents the file name from where the collection(s) will be imported.</param>
        /// <returns>Returns a string array if successful with the names of the imported collections, otherwise an empty array.</returns>
        /// <example><code>string[] collDatas = Batch.FromDynamic("SimpleDemo", "efInternal", @"c:\temp\test.dynamic");</code></example>
        public static string[] FromDynamic(string applicationName, string stationName, string filename)
        {
            List<ITisCollectionData> result = new List<ITisCollectionData>();
            List<string> returnResult = new List<string>();
            
            ITisClientServicesModule csm = null;

            if (filename != String.Empty && File.Exists(filename))
            {
                if (applicationName != String.Empty && stationName != String.Empty)
                {
                    try
                    {
                        csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                        CoreFromDynamic(ref result, ref csm, filename);
                    }
                    catch (Exception e)
                    {
                        if (csm != null)
                            csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrFromDynamicMethod + Constants.cStrFailedWithException + e,
                                Constants.cStrLiteSdk, TIS_SEVERITY.TIS_ERROR, 0, 0);
                        else
                            throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrFromDynamicMethod + Constants.cStrFailedWithException + e);
                    }
                    finally
                    {
                        foreach (ITisCollectionData res in result)
                            returnResult.Add(res.Name);

                        if (csm != null) csm.Dispose();
                    }
                }
            }

            return returnResult.ToArray();
        }
        #endregion "Batch.FromDynamic"

        // Many
        #region "Batch->ToDynamic()"
        /// <summary>
        /// Batch->ToDynamic() --> Exports the collections given by CollectionNames to a .dynamic file.
        /// </summary>
        /// <param name="filename">Represents the file name where the .dynamic will be exported to.</param>
        /// <param name="CollectionNames">Represents the names of the collections to export as a .dynamic file.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.ToDynamic(@"c:\temp\test.dynamic", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public bool ToDynamic(string filename, string[] CollectionNames)
        {
            bool result = false;

            if (CollectionNames != null && CollectionNames.Length > 0)
            {
                if (filename != String.Empty && Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    if (CheckLogon())
                    {
                        try
                        {
                            oCSM.DynamicImportExport.ExportCollectionsByName(filename, CollectionNames);
                        }
                        catch (Exception e)
                        {
                            oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrToDynamicMethod + Constants.cStrFailedWithException +
                                ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()), Constants.cStrLiteSdk, 
                                TIS_SEVERITY.TIS_ERROR, 0, 0);
                        }
                        finally
                        {
                            result = true;
                        }
                    }
                    else
                        if (oCSM != null)
                            oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrToDynamicMethod + Constants.cStrCouldNotLogin, Constants.cStrLiteSdk, 
                                TIS_SEVERITY.TIS_ERROR, 0, 0);
                        else
                            throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrToDynamicMethod + Constants.cStrCouldNotLogin);
                }
                else
                    if (oCSM != null)
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrToDynamicMethod + Constants.cStrFileNameNotFound + filename + "']",
                            Constants.cStrLiteSdk, TIS_SEVERITY.TIS_ERROR, 0, 0);
            }
            else
                if (oCSM != null)
                    oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrToDynamicMethod + Constants.cStrCollectionNameCannotBeEmpty,
                        Constants.cStrLiteSdk, TIS_SEVERITY.TIS_ERROR, 0, 0);

            return result;
        }
        #endregion "Batch->ToDynamic()"

        // Many 
        #region "Batch.ToDynamic"
        /// <summary>
        /// [Static] Batch.ToDynamic() --> Exports the collections given by CollectionNames to a .dynamic file.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM.</param>
        /// <param name="filename">Represents the file name where the .dynamic will be exported to.</param>
        /// <param name="CollectionNames">Represents the names of the collections to export as a .dynamic file.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = Batch.ToDynamic("SimpleDemo", "efInternal", @"c:\temp\test.dynamic", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public static bool ToDynamic(string applicationName, string stationName, string filename, string[] CollectionNames)
        {
            bool result = false;
            
            ITisClientServicesModule csm = null;

            if (CollectionNames != null && CollectionNames.Length > 0)
            {
                if (filename != String.Empty && Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    if (applicationName != String.Empty && stationName != String.Empty)
                    {
                        try
                        {
                            csm = TisClientServicesModule.GetNewInstance(applicationName, stationName);

                            csm.DynamicImportExport.ExportCollectionsByName(filename, CollectionNames);
                        }
                        catch (Exception e)
                        {
                            if (csm != null)
                                csm.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrToDynamicMethod + Constants.cStrFailedWithException + e,
                                    Constants.cStrLiteSdk, TIS_SEVERITY.TIS_ERROR, 0, 0);
                            else
                                throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrToDynamicMethod + Constants.cStrFailedWithException + e);
                        }
                        finally
                        {
                            if (csm != null) csm.Dispose();

                            result = true;
                        }
                    }
                }
            }

            return result;
        }
        #endregion "Batch.ToDynamic"

        #region "Batch->Logout()"
        /// <summary>
        /// Batch->Logout() --> Logs out from the CSM.
        /// </summary>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Logout();</code></example>
        public override bool Logout()
        {
            return base.Logout();
        }
        #endregion "Batch->Logout()"

        #region "Batch->Login()"

        /// <summary>
        /// Batch->Login() --> Login to the CSM using applicationName and stationName.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name to login to.</param>
        /// <param name="stationName">Indicates the eFLOW station name to login to.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Login("SimpleDemo", "Completion");</code></example>
        public bool Login(string applicationName, string stationName)
        {
            bool result = false;
            
            if (applicationName != String.Empty && stationName != String.Empty)
            {
                this.applicationName = applicationName;
                this.stationName = stationName;

                Login();

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Batch->Login() --> Login to the CSM (if the Batch.ApplicationName and Batch.StationName properties have been previously set).
        /// </summary>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Login();</code></example>
        public bool Login()
        {
            bool result = false;
            
            if (this.applicationName != String.Empty && this.stationName != String.Empty)
            {
                try
                {
                    oCSM = TisClientServicesModule.GetNewInstance(this.applicationName, this.stationName);
                }
                catch (Exception e)
                {
                    throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrLoginMethod + Constants.cStrFailedToCreateCSMInstanceForApplicationName +
                        this.applicationName + Constants.cStrAndStationName + this.stationName + Constants.cStrWithException +
                        ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()));
                }
                finally
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion "Batch->Login()"

        #region "Batch->Put()"
        /// <summary>
        /// Batch->Put() --> Puts a ITisCollectionData object (saves it and moves it to the next queue).
        /// </summary>
        /// <param name="Collection">Is a ITisCollectionData object.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Put(collData);</code></example>
        public bool Put(ITisCollectionData Collection)
        {
            return InternalPut(Collection, true, Constants.cStrPutMethod);
        }
        #endregion "Batch->Put()"

        #region "Batch->Free()"
        /// <summary>
        /// Batch->Free() --> Releases a ITisCollectionData object (saves it and keeps it in the same queue).
        /// </summary>
        /// <param name="Collection">Is a ITisCollectionData object.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Free(collData);</code></example>
        public bool Free(ITisCollectionData Collection)
        {
            return InternalPut(Collection, false, Constants.cStrFreeMethod);
        }
        #endregion "Batch->Free()"

        #region "Batch->Delete()"
        /// <summary>
        /// Batch->Delete() --> Deletes a ITisCollectionData object from the system (even if the collection is locked).
        /// </summary>
        /// <remarks>
        /// Attention: Even if the collection is locked, it will be deleted!
        /// </remarks>
        /// <param name="Collection">Is a ITisCollectionData object.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Delete(collData);</code></example>
        public bool Delete(ITisCollectionData Collection)
        {
            return InternalPut(Collection, false, Constants.cStrDeleteMethod);
        }
        #endregion "Batch->Delete()"

        #region "Batch.Delete()"
        /// <summary>
        /// [Static] Batch.Delete() --> Deletes a ITisCollectionData object from the system (even if the collection is locked), given the collection name.
        /// </summary>
        /// <remarks>
        /// Attention: Even if the collection is locked, it will be deleted!
        /// </remarks>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM. It must be the same station where you intend to delete the collection from.</param>
        /// <param name="CollectionName">The collection name to delete.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = Batch.Delete("SimpleDemo", "Completion", "000002349_0001");</code></example>
        public static bool Delete(string applicationName, string stationName, string CollectionName)
        {
            return InternalPut(applicationName, stationName, CollectionName, false, Constants.cStrDeleteMethod);
        }
        #endregion "Batch.Delete()"

        // Many
        #region "Batch.DeleteMany()"
        /// <summary>
        /// [Static] Batch.DeleteMany() --> Deletes ITisCollectionData objects from the system (even if the collections are locked), given the collections names.
        /// </summary>
        /// <remarks>
        /// Attention: Even if the collections are locked, they will be deleted!
        /// </remarks>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM. It must be the same station where you intend to delete the collections from.</param>
        /// <param name="CollectionNames">The collection names to delete.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = Batch.DeleteMany("SimpleDemo", "Completion", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public static bool DeleteMany(string applicationName, string stationName, string[] CollectionNames)
        {
            return InternalPutMany(applicationName, stationName, CollectionNames, false, Constants.cStrDeleteMethod);
        }
        #endregion "Batch.DeleteMany()"

        #region "Batch->Reject()"
        /// <summary>
        /// Batch->Reject() --> Rejects a ITisCollectionData object.
        /// </summary>
        /// <param name="Collection">Is a ITisCollectionData object.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>bool result = b.Reject(collData);</code></example>
        public bool Reject(ITisCollectionData Collection)
        {
            return InternalPut(Collection, true, Constants.cStrRejectMethod);
        }
        #endregion "Batch->Reject()"

        #region "Batch.Reject()"
        /// <summary>
        /// [Static] Batch.Reject() --> Rejects a collection given the collection's name.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM. It must be the same station where you intend to reject the collection from.</param>
        /// <param name="CollectionName">Indicates the collection name to reject.</param>
        /// <returns>Returns true is successful, otherwise false.</returns>
        /// <example><code>Batch.Reject("SimpleDemo", "Completion", "000002349_0001");</code></example>
        public static bool Reject(string applicationName, string stationName, string CollectionName)
        {
            return InternalPut(applicationName, stationName, CollectionName, true, Constants.cStrRejectMethod);
        }
        #endregion "Batch.Reject()"

        // Many
        #region "Batch.RejectMany()"
        /// <summary>
        /// [Static] Batch.RejectMany() --> Rejects several collection given the collection names.
        /// </summary>
        /// <param name="applicationName">Indicates the eFLOW application name.</param>
        /// <param name="stationName">Indicates the station name to logon to the CSM. It must be the same station where you intend to reject the collections from.</param>
        /// <param name="CollectionNames">Indicates the collection names to reject.</param>
        /// <returns>Returns true is successful (if all collections were processed fine - with no exceptions), otherwise false.</returns>
        /// <example><code>Batch.RejectMany("SimpleDemo", "Completion", new string[] { "000002349_0001", "000002349_0002" });</code></example>
        public static bool RejectMany(string applicationName, string stationName, string[] CollectionNames)
        {
            return InternalPutMany(applicationName, stationName, CollectionNames, true, Constants.cStrRejectMethod);        
        }
        #endregion "Batch.RejectMany()"

        #region "Batch->Get()"
        /// <summary>
        /// Batch.Get() --> Retrieves a ITisCollectionData object given the collection name (if it exists), otherwise returns null.
        /// </summary>
        /// <param name="CollectionName">Is a string which indicates the name of the collection to retrieve.</param>
        /// <returns>Returns an ITisCollectionData object if the collection name exists, otherwise returns null.</returns>
        /// <example><code>ITisCollectionData collData = b.Get("000002349_0001");</code></example>
        public ITisCollectionData Get(string CollectionName)
        {
            ITisCollectionData result = null;

            if (CollectionName != String.Empty)
            {
                if (CheckLogon())
                {
                    try
                    {
                        result = oCSM.Dynamic.GetSpecificCollection(CollectionName, false);
                    }
                    catch (Exception e)
                    {
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrGetMethod + Constants.cStrFailedWithException +
                            ((e.InnerException != null) ? e.InnerException.ToString() : e.ToString()), Constants.cStrLiteSdk,
                            TIS_SEVERITY.TIS_ERROR, 0, 0);
                    }
                    finally
                    {
                        if (result != null)
                            if (collections == null || collections.IndexOf(CollectionName) < 0)
                            {
                                if (collections == null) collections = new List<string>();
                                collections.Add(CollectionName);
                            }
                    }
                }
                else
                    if (oCSM != null)
                        oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrGetMethod + Constants.cStrCouldNotLogin, Constants.cStrLiteSdk, 
                            TIS_SEVERITY.TIS_ERROR, 0, 0);
                    else
                        throw new Exception(Constants.cStrLiteSdkBatch + Constants.cStrGetMethod + Constants.cStrCouldNotLogin);
            }
            else
                if (oCSM != null)
                    oCSM.Logger.RequestMessageLog(Constants.cStrLiteSdkBatch + Constants.cStrGetMethod + Constants.cStrCollectionNameCannotBeEmpty, Constants.cStrLiteSdk, 
                        TIS_SEVERITY.TIS_ERROR, 0, 0);

            return result;
        }
        #endregion "Batch->Get()"

        #endregion "Exposed/Public methods"
    }

    #endregion "class Batch"
}

#endregion "namespace Lite.CollectionManagement"
