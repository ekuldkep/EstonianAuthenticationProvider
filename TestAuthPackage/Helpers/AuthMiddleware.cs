using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TestAuthPackage.Constants;
using BLL.Helpers;
using Microsoft.AspNetCore.Http;
using TestAuthPackage.Dtos;
using Microsoft.Extensions.Options;

namespace TestAuthPackage.Helpers
{ 
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthMiddleware
    {
        public CertificateValidalidationConfig OptionsWithDelegateConfig;

        public AuthMiddleware(IOptions<CertificateValidalidationConfig> options)
        {
            OptionsWithDelegateConfig = options.Value;
        }

        public AuthMiddleware()
        {
            
        }
        //By default returns DigiDocService with test wsdl
        public virtual DigiDocService GetDigiDocService()
        {
            return new DigiDocService();
        }

        //Returns ertificationVerificationHelper
        public virtual CertificationVerificationHelper GetClient()
        {
            return new CertificationVerificationHelper(GetDigiDocService());
        }

        //By default checks if certificate is from valid root, can be overwritten with method that does not check chain
        public virtual async Task<CertificationInfoDto> CheckCertificationInfo(HttpContext httpContext, string clientCertificate)
        {
            var client = GetClient();
            return await client.CertificationInfoAndChain(clientCertificate, OptionsWithDelegateConfig.MiddleCertifications, OptionsWithDelegateConfig.BaseCert);
        }

        //Is useful for testing, does not have to use certificate
        public virtual async Task<string> GetCertifikateSettings(HttpContext httpContext)
        {
            return await Task.FromResult(OptionsWithDelegateConfig.Thumbprint);
        }

        //Default value, can be used if server is IIS
        public virtual async Task<string> GetCertificateHttpContext(HttpContext httpContext)
        {
            X509Certificate2 clientCertificate = await httpContext.Connection.GetClientCertificateAsync();
            return Convert.ToBase64String(clientCertificate.Export(X509ContentType.Cert));
        }

        //Can be used if other server besides IIS is used
        public virtual async Task<string> GetCertificateHttpRequestHeader(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;
            var headers = request.Headers.TryGetValue("SSL_CLIENT_CERT", out var headerCertString);  
            var str = headerCertString.ToString();
            str = str.Replace("-----BEGIN CERTIFICATE-----", "").Replace("-----END CERTIFICATE-----", "");
            return await Task.FromResult(str);
        }

        //ByDefault uses method for iis server but can be overwritten for using other server or with custom method
        public virtual async Task<string> GetCertificate(HttpContext httpContext)
        {
            return await GetCertificateHttpContext(httpContext);
        }

        //Default encryption can be overwritten with custom encryption
        public virtual string EncryptResponse(AuthenticationDto authentication)
        {
            return SecurityHelper.EncryptString(authentication.PropertyValuesCommaSeparated(),
                EncryptionKey.Key());
        }

        //Starts ID Card authentication
        public async Task InvokeCert(HttpContext httpContext)
        {
            var certificate = await GetCertificate(httpContext);

            if (!string.IsNullOrEmpty(certificate))
            {
                var certificationInfo = CheckCertificationInfo(httpContext, certificate);

                if (certificationInfo.Result.CanRedirect)
                {
                    var encryptedCertInfo = EncryptResponse(certificationInfo.Result.AuthenticationDto);

                    var token = System.Net.WebUtility.UrlEncode(encryptedCertInfo);

                    if (!httpContext.Request.QueryString.ToString().Contains("?token="))
                    {
                        var newPath = OptionsWithDelegateConfig.RedirectUrl + $"?token={token}"; 
                        httpContext.Response.Redirect(newPath, false);
                    }
                }
            }
        }
    }
}
