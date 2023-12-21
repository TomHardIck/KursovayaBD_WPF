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

        public string EncryptPassword(string value_to_encrypt, string salt_value)
        {
            SHA256Managed sHA256Managed = new SHA256Managed();
            return Convert.ToBase64String(sHA256Managed.ComputeHash(Encoding.UTF8.GetBytes(value_to_encrypt + salt_value)));
        }

        public bool AreEqual(string text, string hash, string salt)
        {
            string newPin = EncryptPassword(text, salt);
            return newPin.Equals(hash);
        }
    }
}
