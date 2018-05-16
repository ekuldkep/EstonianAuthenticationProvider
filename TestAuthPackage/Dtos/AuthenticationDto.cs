using System;
using System.Collections.Generic;
using System.Text;
using TestAuthPackage.Constants;

namespace TestAuthPackage.Dtos
{
    public class AuthenticationDto
    {
        public string IdCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserCountry { get; set; }
        public string UserOrganisation { get; set; }
        public string UserCN { get; set; }
        public string IssuerCN { get; set; }
        public string KeyUsage { get; set; }
        public string Status { get; set; }
        public string RevocationData { get; set; }
        public string EnhancedKeyUsage { get; set; }
        public string Secret { get; set; }
        public string SkMobileIdAuthenticateStatus { get; set; }
        public string ChallengeID { get; set; }
        public int SessionCode { get; set; }
        public string CertificateData { get; set; }
        public string Challenge { get; set; }
        public AuthenticationResultType AuthenticationResultType { get; set; }
        public string Message { get; set; }
        public bool IsMobileIdValid { get; set; }
        public string ErrorCode { get; set; }
    }
}
