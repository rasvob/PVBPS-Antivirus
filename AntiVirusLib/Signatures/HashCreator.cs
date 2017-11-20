using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AntiVirusLib.Signatures
{
    public class HashCreator
    {
        public string CreateMd5Hash(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            using (MD5CryptoServiceProvider cryptoService = new MD5CryptoServiceProvider())
            {
                return cryptoService.ComputeHash(bytes).ToHex();
            }
        }

        public string CreateSha1Hash(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            using (SHA1CryptoServiceProvider cryptoService = new SHA1CryptoServiceProvider())
            {
                return cryptoService.ComputeHash(bytes).ToHex();
            }
        }

        public string CreateSha256Hash(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            using (SHA256CryptoServiceProvider cryptoService = new SHA256CryptoServiceProvider())
            {
                return cryptoService.ComputeHash(bytes).ToHex();
            }
        }
    }
}