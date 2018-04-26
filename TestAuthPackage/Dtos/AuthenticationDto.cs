using System;
using System.Collections.Generic;
using System.Text;

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

        public string PropertyValuesCommaSeparated()
        {
            return
                $"{IdCode},{FirstName},{LastName},{Status},{UserCountry},{UserOrganisation},{UserCN},{IssuerCN},{KeyUsage},{RevocationData},{EnhancedKeyUsage}";
        }
    }
}
