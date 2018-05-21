using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EstonianAuthenticationProvider.Constants
{
    public class KeyGenerator
    {
        private static Random random = new Random();

        public static string GenerateKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 32)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
