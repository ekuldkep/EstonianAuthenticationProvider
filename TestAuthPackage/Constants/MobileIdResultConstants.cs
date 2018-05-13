using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace TestAuthPackage.Constants
{
    public static class MobileIdResultConstants
    {
        public const string OutstandingTransaction = "OUTSTANDING_TRANSACTION";
        public const string UserAuthenticated = "USER_AUTHENTICATED";
        public const string NotValid = "NOT_VALID";
        public const string ExpiredTransaction = "EXPIRED_TRANSACTION";
        public const string UserCancel = "USER_CANCEL";
        public const string MidNotReady = "MID_NOT_READY";
        public const string PhoneAbsent = "PHONE_ABSENT";
        public const string SendingError = "SENDING_ERROR";
        public const string SimError = "SIM_ERROR";
        public const string InternalError = "INTERNAL_ERROR";
    }
}
