using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Util
{
    public static class Hashes
    {
        public static byte[] GetNewSalt() => RandomNumberGenerator.GetBytes(32);

        public static byte[] GetPbkdf2Hash(string password, byte[] salt)
        {
            var bytes = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 600000, 32);
            return bytes;
        }
    }
}