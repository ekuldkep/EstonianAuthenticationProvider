using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TestAuthPackage.Constants;
using TestAuthPackage.Dtos;

namespace TestAuthPackage.Helpers
{
    public static class UnpackHelper
    {
        public static AuthenticationDto UnpackDto(HttpContext httpContext)
        {
            var secretLocal = httpContext.Session.GetString(SecurityConstants.ClientSecret);

            var token = httpContext.Request.Query["token"].ToString();

            var decryptedToken = SecurityHelper.DecryptString(token, EncryptionKey.Key());

            var authenticationDto =  JsonConvert.DeserializeObject<AuthenticationDto>(decryptedToken);

            if (authenticationDto.Secret == secretLocal)
            {
                return null;
            }

            return authenticationDto;
        }
    }
}
