using System;
using System.Collections.Generic;
using System.Text;

namespace TestAuthPackage.Constants
{
    public class CertificateValidalidationConfig
    {
        public string BaseCert { get; set;}
        public List<string> MiddleCertifications { get; set; }
        public string Thumbprint { get; set; }
        public string RedirectUrl { get; set; }
       
    }
}
