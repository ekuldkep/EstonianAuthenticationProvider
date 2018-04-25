using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TestAuthPackage.Constants;
using TestAuthPackage.Dtos;
using DigiDocService;

namespace TestAuthPackage.Helpers
{
    public class CertificationVerificationHelper
    {
        public static string ServiceName = "DigiDocService";
        public static string EndpointUrl = "https://tsp.demo.sk.ee/dds.wsdl";

        public virtual DigiDocServicePortTypeClient ReturnDigiDocService()
        {
            // https://stackoverflow.com/a/3704312
            var binding = new BasicHttpBinding();
            binding.Name = ServiceName;
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            var endpoint = new EndpointAddress(EndpointUrl);
            return new DigiDocServicePortTypeClient(binding, endpoint);
        }

        public virtual CertificationInfoDto MakeResponse(CheckCertificateResponse checkCertificateResponse)
        {
            var certificationInfoDto = new CertificationInfoDto();
            if (checkCertificateResponse.Status == CertificateStatusConstants.CertificateStatusGood)
            {
                var authenticationDto = new AuthenticationDto
                {
                    FirstName = checkCertificateResponse.UserGivenname,
                    LastName = checkCertificateResponse.UserSurname,
                    IdCode = checkCertificateResponse.UserIDCode
                };
                certificationInfoDto.IsCertificateValid = true;
                certificationInfoDto.AuthenticationDto = authenticationDto;
                certificationInfoDto.IsCertificationFromValidRoot = true;
                return certificationInfoDto;
            }

            certificationInfoDto.IsCertificateValid = false;
            certificationInfoDto.AuthenticationDto = null;
            certificationInfoDto.IsCertificationFromValidRoot = true;

            return certificationInfoDto;
        }

        public async Task<CertificationInfoDto> CertificationInfoAndChain(string clientCertificate, List<string> middleCertifications, string baseCert)
        {
            var isCertFromValidChain = IsClientCertFromValidRoot(clientCertificate, middleCertifications, baseCert);
            if (!isCertFromValidChain)
            {
                return new CertificationInfoDto { IsCertificationFromValidRoot = false };
            }

            return await CertificationInfo(clientCertificate);
        }

        public async Task<CertificationInfoDto> CertificationInfo(string clientCertificate)
        {
            var cerClient = ReturnDigiDocService();
            var certInfo = await cerClient.CheckCertificateAsync(new CheckCertificateRequest(clientCertificate, true));
            return MakeResponse(certInfo);
        }

        public async Task<string> FredFunctionThatReplacesPreviousTHing(string clientCertificate)
        {
            var cerClient = ReturnDigiDocService();
            var certInfo = await cerClient.CheckCertificateAsync(new CheckCertificateRequest(clientCertificate, true));
            return certInfo.UserIDCode;
        }

        private static bool IsClientCertFromValidRoot(string clientCertificate, List<string> additionalCertificates, string baseCert)
        {
            var convertedCertificates = ConvertAllCertificates(clientCertificate, additionalCertificates, baseCert);
            var clietCert = convertedCertificates.ClientCertificate;
            foreach (var additionalCertificate in convertedCertificates.AdditionalCertificates)
            {
                List<byte[]> middleCertsBytes = new List<byte[]>();
                middleCertsBytes.Add(additionalCertificate);
                middleCertsBytes.Add(convertedCertificates.BaseCertificate);
                var isCertfromValidRoot = VerifyCertificate(clietCert, middleCertsBytes);
                if (isCertfromValidRoot)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool VerifyCertificate(byte[] primaryCertificate, List<byte[]> additionalCertificates)
        {
            var chain = new X509Chain();

            foreach (var cert in additionalCertificates.Select(x => new X509Certificate2(x)))
            {
                chain.ChainPolicy.ExtraStore.Add(cert);
            }

            // You can alter how the chain is built/validated.
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreWrongUsage;

            // Do the preliminary validation.
            var primaryCert = new X509Certificate2(primaryCertificate);
            if (!chain.Build(primaryCert))
                return false;

            // Make sure we have the same number of elements.
            if (chain.ChainElements.Count != chain.ChainPolicy.ExtraStore.Count + 1)
                return false;

            // Make sure all the thumbprints of the CAs match up.
            // The first one should be 'primaryCert', leading up to the root CA.
            for (var i = 1; i < chain.ChainElements.Count; i++)
            {
                if (chain.ChainElements[i].Certificate.Thumbprint != chain.ChainPolicy.ExtraStore[i - 1].Thumbprint)
                    return false;
            }

            return true;
        }

        private static byte[] ConvertCertToByteArray(string certificate)
        {
            var convertedCert = Encoding.ASCII.GetBytes(certificate);
            return convertedCert;
        }

        public static AllCertificates ConvertAllCertificates(string clientCert, List<string> middleCerts, string baseCert)
        {
            List<byte[]> middleCertsBytes = new List<byte[]>();

            foreach (var middleCert in middleCerts)
            {
                var byteCertContainer = ConvertCertToByteArray(middleCert);
                middleCertsBytes.Add(byteCertContainer);
            }
            var certificates = new AllCertificates()
            {
                ClientCertificate = ConvertCertToByteArray(clientCert),
                BaseCertificate = ConvertCertToByteArray(baseCert),
                AdditionalCertificates = middleCertsBytes
            };

            return certificates;
        }

    }

    public class AllCertificates
    {
        public List<byte[]> AdditionalCertificates { get; set; }
        public byte[] ClientCertificate { get; set; }
        public byte[] BaseCertificate { get; set; }


    }
}
