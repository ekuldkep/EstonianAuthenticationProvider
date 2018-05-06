using System;
using System.Collections.Generic;
using System.Text;

namespace TestAuthPackage.Dtos
{
    public class AllCertificates
    {
        public List<byte[]> AdditionalCertificates { get; set; }
        public byte[] ClientCertificate { get; set; }
        public byte[] BaseCertificate { get; set; }
    }
}
