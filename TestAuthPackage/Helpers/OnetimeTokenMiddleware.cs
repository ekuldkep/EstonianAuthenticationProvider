using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TestAuthPackage.Constants;
using TestAuthPackage.Dtos;

namespace TestAuthPackage.Helpers
{
    public class OnetimeTokenMiddleware
    {
        private readonly RequestDelegate _next;
        public OnetimeTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string tokenValue = EncryptionKey.GenerateKey();

            if (httpContext.Session.GetString(SecurityConstants.ClientSecret) == null)
            {
                httpContext.Session.SetString(SecurityConstants.ClientSecret, tokenValue);
            }
            await _next(httpContext);
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class IdAuthMiddlewareExtensions
    {

        public static IApplicationBuilder UseIdAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OnetimeTokenMiddleware>();
        }
    }
}