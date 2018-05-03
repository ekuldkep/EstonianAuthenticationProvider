using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using DigiDocService;

namespace TestAuthPackage.Helpers
{
    public class DigiDocService
    {
        public string ServiceName = "DigiDocService";
        public string EndpointUrl = "https://tsp.demo.sk.ee/dds.wsdl";
        public BasicHttpSecurityMode SecurityMode = BasicHttpSecurityMode.Transport;
        public TimeSpan CloseTimeout = TimeSpan.FromMinutes(1);
        public TimeSpan OpenTimeout = TimeSpan.FromMinutes(1);
        public TimeSpan SendTimeout = TimeSpan.FromMinutes(1);
        public TimeSpan RecieveTimeout = TimeSpan.FromMinutes(10);
        public bool AllowCookies = false;
        public bool BypassProxyOnLocal = false;
        public int MaxBufferSize = 65536;
        public int MaxBufferPoolSize = 524288;
        public TransferMode TransferMode = TransferMode.Buffered;
        public Encoding TextEncoding = System.Text.Encoding.UTF8;
        public bool UseDefaultWebProxy = true;
        public int MaxDepth = 32;
        public int MaxStringContentLength = 8192;
        public int MaxArrayLength = 16384;
        public int MaxBytesPerRead = 4096;
        public int MaxNameTableCharCount = 8192;
        public HttpClientCredentialType ClientCredentialType = HttpClientCredentialType.None;
        public HttpProxyCredentialType ProxyCredentialType = HttpProxyCredentialType.None;
        public string Realm = "";
        public BasicHttpMessageCredentialType BasicClientCredentialType = BasicHttpMessageCredentialType.UserName;

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
