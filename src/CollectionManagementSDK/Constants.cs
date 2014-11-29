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

#endregion "using"

#region "namespace eFlow.CollectionManagement"

namespace eFlow.CollectionManagement
{
    #region "class Constants"

    /// <summary>
    /// "Constants" Class --> Constants used by the Batch class. 
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class Constants
    {
        #region "constants"

        #region "sdk specific"
        /// <summary>
        /// SDK specific
        /// </summary>
        public const string cStrLiteSdk = "eFlow.SDK.CM.CollectionManagement";
        public const string cStrLiteSdkBatch = "eFlow.SDK.CM.CollectionManagement.Batch()";
        #endregion "sdk specific"

        #region "Message / exception specific"
        /// <summary>
        /// Message / exception specific
        /// </summary>
        public const string cStrCouldNotLogin = "Could not connect to CSM. Did you execute Batch->Login()?";
        public const string cStrFailedToConnectToCsm = "Could not connect to CSM.";
        public const string cStrStationNameIsEmpty = "[stationName] is empty.";
        public const string cStrFileNameNotFound = "filename is empty or path does not exist [filename='";
        public const string cStrFailedWithException = "failed with exception: ";
        public const string cStrCollectionNameCannotBeEmpty = "CollectionName[s] parameter cannot be empty or NULL";
        public const string cStrFailedToCreateCSMInstanceForApplicationName = "Failed to create oCSM instance for [ApplicationName]: ";
        public const string cStrAndStationName = " and [StationName]: ";
        public const string cStrWithException = " with exception: ";
        public const string cStrCollectionIsNull = "Collection[s] parameter is NULL";
        #endregion "Message / exception specific"

        #region "Methods specific"
        /// <summary>
        /// Methods specific
        /// </summary>
        public const string cStrPrioritizeMethod = ".Prioritize() -> ";
        public const string cStrHistoryMethod = ".History() -> ";
        public const string cStrHoldMethod = ".Hold() -> ";
        public const string cStrMoveMethod = ".Move() -> ";
        public const string cStrGetMethod = ".Get() -> ";
        public const string cStrPutMethod = ".Put() -> ";
        public const string cStrFreeMethod = ".Free() -> ";
        public const string cStrDeleteMethod = ".Delete() -> ";
        public const string cStrRejectMethod = ".Reject() -> ";
        public const string cStrRenameMethod = ".Rename() -> ";
        public const string cStrLoginMethod = ".Login() -> ";
        public const string cStrLogoutMethod = ".Logout() -> ";
        public const string cStrToDynamicMethod = ".ToDynamic() -> ";
        public const string cStrFromDynamicMethod = ".FromDynamic() -> ";
        public const string cStrSplitPagePerFormMethod = ".SplitPagePerForm() -> ";
        public const string cStrUnlockSpecificCollection = ".UnlockSpecificCollection() -> ";
        #endregion "Methods specific"

        #endregion "constants"
    }

    #endregion "class Constants"
}

#endregion "namespace eFlow.CollectionManagement"