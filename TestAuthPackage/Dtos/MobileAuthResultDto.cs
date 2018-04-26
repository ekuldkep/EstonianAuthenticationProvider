using System;
using System.Collections.Generic;
using System.Text;
using TestAuthPackage.Constants;

namespace TestAuthPackage.Dtos
{
    public class MobileAuthResultDto
    {
        public SkMobileAuthenticateStatus SkMobileIdAuthenticateStatus { get; set; }
        public string ChallengeID { get; set; }
        public int SessionCode { get; set; }
        public string IdentificationCode { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public string UserCountry { get; set; }
        public string UserCN { get; set; }
        public string CertificateData { get; set; }
        public string Challenge { get; set; }
        public string RevokationData { get; set; }
        public AuthenticationResultType AuthenticationResultType { get; set; }
        public string Message { get; set; }
        public bool IsMobileIdValid { get; set; }
    }
}
