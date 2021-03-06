﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DigiDocService;
using EstonianAuthenticationProvider.Constants;
using EstonianAuthenticationProvider.Dtos;

namespace EstonianAuthenticationProvider.Helpers
{
    public class CertificationVerificationHelper
    {
        public DigiDocServiceHelper DigiDocService;
        public CertificationVerificationHelper(DigiDocServiceHelper digiDocService)
        {
            DigiDocService = digiDocService;
        }

        //tundub vb imelik, et miks seda klassis välja ei kutsta
        //Makes response
        public virtual CertificationInfoDto MakeResponse(CheckCertificateResponse checkCertificateResponse)
        {
            var certificationInfoDto = new CertificationInfoDto();
            if (checkCertificateResponse.Status == CertificateStatusConstants.CertificateStatusGood)
            {
                var authenticationDto = new AuthenticationDto
                {
                    FirstName = checkCertificateResponse.UserGivenname,
                    LastName = checkCertificateResponse.UserSurname,
                    IdCode = checkCertificateResponse.UserIDCode,
                    EnhancedKeyUsage = checkCertificateResponse.EnhancedKeyUsage,
                    IssuerCN = checkCertificateResponse.IssuerCN,
                    KeyUsage = checkCertificateResponse.KeyUsage,
                    RevocationData = checkCertificateResponse.RevocationData,
                    Status = checkCertificateResponse.Status,
                    UserCN = checkCertificateResponse.UserCN,
                    UserCountry = checkCertificateResponse.UserCountry,
                    UserOrganisation = checkCertificateResponse.UserOrganisation,
                };
                certificationInfoDto.IsCertificateValid = true;
                certificationInfoDto.AuthenticationDto = authenticationDto;
                certificationInfoDto.CertifikateChainStatus = CertificateChainStatusEnum.NotChecked; 
                certificationInfoDto.CanRedirect = true;
                certificationInfoDto.CertificateStatus = checkCertificateResponse.Status;
                return certificationInfoDto;
            }

            certificationInfoDto.CanRedirect = false;
            certificationInfoDto.IsCertificateValid = false;
            certificationInfoDto.AuthenticationDto = null;
            certificationInfoDto.CertifikateChainStatus = CertificateChainStatusEnum.NotChecked;
            certificationInfoDto.CertificateStatus = checkCertificateResponse.Status;

            return certificationInfoDto;
        }

        //By default does chain check 
        public virtual async Task<CertificationInfoDto> CertificationInfoAndChain(string clientCertificate, List<string> middleCertifications, string baseCert)
        {
            var isCertFromValidChain = IsClientCertFromValidRoot(clientCertificate, middleCertifications, baseCert);
            if (!isCertFromValidChain)
            {
                return new CertificationInfoDto
                {
                    CertifikateChainStatus = CertificateChainStatusEnum.InvalidRoot,
                    CanRedirect = false
                };
            }

            var response = await CertificationInfoWoChain(clientCertificate);
            response.CertifikateChainStatus = CertificateChainStatusEnum.ValidRoot;
            return response;
        }

        //can be used if client does not want to check if certificate is from valid root
        public virtual async Task<CertificationInfoDto> CertificationInfoWoChain(string clientCertificate)
        {
            return await CertificationInfo(clientCertificate);
        }

        //Does request to DigiDocService
        public virtual async Task<CertificationInfoDto> CertificationInfo(string clientCertificate)
        {
            var cerClient = DigiDocService.ReturnDigiDocService();
            var certInfo = await cerClient.CheckCertificateAsync(new CheckCertificateRequest(clientCertificate, true));
            return MakeResponse(certInfo);
        }

        //Makes Chains
        public virtual bool IsClientCertFromValidRoot(string clientCertificate, List<string> additionalCertificates, string baseCert)
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
        public virtual bool VerifyCertificate(byte[] primaryCertificate, List<byte[]> additionalCertificates)
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

        public virtual byte[] ConvertCertToByteArray(string certificate)
        {
            var convertedCert = Encoding.ASCII.GetBytes(certificate);
            return convertedCert;
        }

        public virtual AllCertificates ConvertAllCertificates(string clientCert, List<string> middleCerts, string baseCert)
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
}
   

