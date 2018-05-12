using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAuthPackage.Constants
{
    public class EncryptionKey
    {
        private static string _encryptionKey;
        private static Random random = new Random();

        public static string Key()
        {
            if (_encryptionKey == null)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                _encryptionKey = new string(Enumerable.Repeat(chars, 32)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            return _encryptionKey;
        }
    }
}
