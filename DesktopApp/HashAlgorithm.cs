using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp
{
    public class HashAlgorithm
    {
        public string CreateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public string GenerateHash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed sHA256Managed = new SHA256Managed();
            byte[] hash = sHA256Managed.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool AreEqual(string text, string hash, string salt)
        {
            string newPin = GenerateHash(text, salt);
            return newPin.Equals(hash);
        }
    }
}
