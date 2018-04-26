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
        public static string RedirectUrl;

        public virtual DigiDocService GetDigiDocService()
        {
            return new DigiDocService();
        }
        public virtual CertificationVerificationHelper GetClient(HttpContext httpContext)
        {
            return new CertificationVerificationHelper(GetDigiDocService());
        }

        public virtual async Task<CertificationInfoDto> CheckCertificationInfo(HttpContext httpContext, string clientCertificate)
        {
            var client = GetClient(httpContext);
            return await client.CertificationInfoAndChain(clientCertificate, MiddleCertifications, BaseCert);
        }

        public virtual async Task<string> GetCertifikateSettings(HttpContext httpContext)
        {
            return Thumbprint;
        }
        public virtual async Task<string> GetCertificateHttpContext(HttpContext httpContext)
        {
            X509Certificate2 clientCertificate = await httpContext.Connection.GetClientCertificateAsync();
            return Convert.ToBase64String(clientCertificate.Export(X509ContentType.Cert));
        }

        public virtual async Task<string> GetCertificateHttpRequestHeader(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;
            var headers = request.Headers.TryGetValue("SSL_CLIENT_CERT", out var headerCertString);  
            var str = headerCertString.ToString();
            str = str.Replace("-----BEGIN CERTIFICATE-----", "").Replace("-----END CERTIFICATE-----", "");
            return str;
        }

        public virtual async Task<string> GetCertificate(HttpContext httpContext)
        {
            return await GetCertificateHttpContext(httpContext);
        }

        public virtual string EncryptResponse(AuthenticationDto authentication)
        {
            return SecurityHelper.EncryptString(authentication.PropertyValuesCommaSeparated(),
                EncryptionKey.Key());
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var certificate = await GetCertificate(httpContext);

            if (!String.IsNullOrEmpty(certificate))
            {
                var certificationInfo = CheckCertificationInfo(httpContext, certificate);

                if (certificationInfo.Result.CanRedirect)
                {
                    var encryptedCertInfo = EncryptResponse(certificationInfo.Result.AuthenticationDto);

                    var token = System.Net.WebUtility.UrlEncode(encryptedCertInfo);

                    if (!httpContext.Request.QueryString.ToString().Contains("?token="))
                    {
                        var newPath = RedirectUrl + $"?token={token}"; 
                        httpContext.Response.Redirect(newPath, false);
                    }
                }
            }
            await _next(httpContext);
        }
    }
}
