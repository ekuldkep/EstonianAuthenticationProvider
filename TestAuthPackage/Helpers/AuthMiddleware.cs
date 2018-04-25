using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TestAuthPackage.Constants;
using BLL.Helpers;
using Microsoft.AspNetCore.Http;
using TestAuthPackage.Dtos;

namespace TestAuthPackage.Helpers
{ 
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthMiddleware
    {
        public static string BaseCert;
        public static List<string> MiddleCertifications;

        public static string Thumbprint;

        public RequestDelegate _next;

        public virtual CertificationVerificationHelper GetClient(HttpContext httpContext)
        {
            return new CertificationVerificationHelper();
        }

        public virtual async Task<CertificationInfoDto> CheckCertificationInfo(HttpContext httpContext, string clientCertificate, List<string> middleCertifications, string baseCert)
        {
            var client = GetClient(httpContext);
            return await client.CertificationInfoAndChain(clientCertificate, middleCertifications, baseCert);
        }

        public virtual async Task<string> GetThumbprintSettings(HttpContext httpContext)
        {
            return Thumbprint;
        }
        public virtual async Task<string> GetThumbprintHttpContext(HttpContext httpContext)
        {
            X509Certificate2 clientCertificate = await httpContext.Connection.GetClientCertificateAsync();
            return clientCertificate.Thumbprint;
        }

        public virtual async Task<string> GetThumbprintHttpRequestHeader(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;
            var headers = request.Headers.TryGetValue("SSL_CLIENT_CERT", out var headerCertString);  // TODO: can not use?
            var str = headerCertString.ToString();
            str = str.Replace("-----BEGIN CERTIFICATE-----", "").Replace("-----END CERTIFICATE-----", "");
            return str;
        }


        public virtual async Task<string> GetThumbprint(HttpContext httpContext)
        {
            return await GetThumbprintHttpContext(httpContext);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            X509Certificate2 clientCertificate;
            var thumbprint = await GetThumbprint(httpContext);

            try
            {
                var certBytes = Convert.FromBase64String(thumbprint);
                clientCertificate = new X509Certificate2(certBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ":" + thumbprint);
                throw;
            }
            
            string convertedCert = Convert.ToBase64String(clientCertificate.Export(X509ContentType.Cert));

            if (!String.IsNullOrEmpty(convertedCert))
            {
                var certificationInfo = CheckCertificationInfo(httpContext, convertedCert, MiddleCertifications, BaseCert);

                if (certificationInfo.Result.IsCertificateValid && certificationInfo.Result.IsCertificationFromValidRoot)
                {
                    var encryptedCertInfo = SecurityHelper.EncryptString(
                        certificationInfo.Result.AuthenticationDto.IdCode,
                        EncryptionKey.Key());

                    var token = System.Net.WebUtility.UrlEncode(encryptedCertInfo);

                    if (!httpContext.Request.QueryString.ToString().Contains("?token="))
                    {
                        var newPath = httpContext.Request.Path + $"?token={token}"; // Rewrite the paths
                        httpContext.Response.Redirect(newPath, false);
                    }
                }
            }
            await _next(httpContext);
        }
    }
}
