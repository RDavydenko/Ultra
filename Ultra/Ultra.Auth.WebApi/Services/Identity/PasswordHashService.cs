using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Ultra.Auth.WebApi.Services.Identity
{
    internal interface IPasswordHashService
    {
        string GetHash(string password, byte[] salt);
        byte[] GetSalt();
    }

    internal class PasswordHashService : IPasswordHashService
    {
        private const string Pepper = "j$#dl3_s]";

        public string GetHash(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password + Pepper,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }

        public byte[] GetSalt()
        {
            byte[] salt = new byte[128 / 8];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }
    }
}
