using System;
using System.Collections.Generic;
using System.Text;
using TestAuthPackage.Constants;

namespace TestAuthPackage.Dtos
{
    public class MobileAuthResultDto
    {
        public string SkMobileIdAuthenticateStatus { get; set; }
        public string ChallengeID { get; set; }
        public int SessionCode { get; set; }
        public string IdCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserCountry { get; set; }
        public string UserCN { get; set; }
        public string CertificateData { get; set; }
        public string Challenge { get; set; }
        public string RevocationData { get; set; }
        public AuthenticationResultType AuthenticationResultType { get; set; }
        public string Message { get; set; }
        public bool IsMobileIdValid { get; set; }
        public string ErrorCode { get; set; }

        public string PropertyValuesCommaSeparated(string secret)
        {
            return
                $"{secret},{IdCode},{FirstName},{LastName},{UserCountry},{UserCN},{RevocationData}";
        }
    }
}
