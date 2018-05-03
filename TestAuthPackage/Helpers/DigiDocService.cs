using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using DigiDocService;

namespace TestAuthPackage.Helpers
{
    public class DigiDocService
    {
        public readonly string ServiceName = "DigiDocService";
        public readonly string EndpointUrl = "https://tsp.demo.sk.ee/dds.wsdl";
        public readonly BasicHttpSecurityMode SecurityMode = BasicHttpSecurityMode.Transport;
        public readonly TimeSpan CloseTimeout = TimeSpan.FromMinutes(1);
        public readonly TimeSpan OpenTimeout = TimeSpan.FromMinutes(1);
        public readonly TimeSpan SendTimeout = TimeSpan.FromMinutes(1);
        public readonly TimeSpan RecieveTimeout = TimeSpan.FromMinutes(10);
        public readonly bool AllowCookies = false;
        public readonly bool BypassProxyOnLocal = false;
        public readonly int MaxBufferSize = 65536;
        public readonly int MaxBufferPoolSize = 524288;
        public readonly TransferMode TransferMode = TransferMode.Buffered;
        public readonly Encoding TextEncoding = System.Text.Encoding.UTF8;
        public readonly bool UseDefaultWebProxy = true;
        public readonly int MaxDepth = 32;
        public readonly int MaxStringContentLength = 8192;
        public readonly int MaxArrayLength = 16384;
        public readonly int MaxBytesPerRead = 4096;
        public readonly int MaxNameTableCharCount = 8192;
        public readonly HttpClientCredentialType ClientCredentialType = HttpClientCredentialType.None;
        public readonly HttpProxyCredentialType ProxyCredentialType = HttpProxyCredentialType.None;
        public readonly string Realm = "";
        public readonly BasicHttpMessageCredentialType BasicClientCredentialType = BasicHttpMessageCredentialType.UserName;

        public DigiDocService(string serviceName, string endpointUrl, BasicHttpSecurityMode securityMode)
        {
            ServiceName = serviceName;
            EndpointUrl = endpointUrl;
            SecurityMode = securityMode;
        }

        public DigiDocService()
        {

        }
        public virtual DigiDocServicePortTypeClient ReturnDigiDocService()
        {
            // https://stackoverflow.com/a/3704312
            var binding = new BasicHttpBinding();
            binding.Name = ServiceName;
            binding.Security.Mode = SecurityMode;
            var endpoint = new EndpointAddress(EndpointUrl);
            return new DigiDocServicePortTypeClient(binding, endpoint);
        }
    }
}
