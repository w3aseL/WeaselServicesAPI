using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace WeaselServicesAPI
{
    public static class PasswordHasher
    {
        public static byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100000, 32);
        }

        public static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(8);
        }

        public static bool ComparePasswords(byte[] storedPassword, byte[] enteredPassword)
        {
            return storedPassword.SequenceEqual(enteredPassword);
        }
    }
}
