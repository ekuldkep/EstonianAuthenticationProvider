using System;
using System.Collections.Generic;
using System.Text;

namespace TestAuthPackage.Constants
{
    public enum SkMobileAuthenticateStatus
    {
        /// <summary>
        /// Authenticate progress started successfully
        /// </summary>
        Ok,

        /// <summary>
        /// User is authenticated
        /// </summary>
        UserAuthenticated,

        /// <summary>
        /// Authentication process failed (with unexpected error)
        /// </summary>
        Failed,

        /// <summary>
        /// Authentication in progress
        /// </summary>
        OutstandingTransaction,

        /// <summary>
        /// Authentication ended, but signature is not valid
        /// </summary>
        NotValid,

        /// <summary>
        /// Session expired
        /// </summary>
        ExpiredTransaction,

        /// <summary>
        /// User has canceled the authentication process
        /// </summary>
        UserCancel,

        /// <summary>
        /// Mobile-ID functionality is not yet available
        /// </summary>
        MidNotReady,

        /// <summary>
        /// Phone is switched off or out of coverage
        /// </summary>
        PhoneAbsent,

        /// <summary>
        /// Error has encountered while sending message to mobile phone
        /// </summary>
        SendingError,

        /// <summary>
        /// SIM application error
        /// </summary>
        SimError,

        /// <summary>
        /// Techincal error with service
        /// </summary>
        InternalError
    }
}
