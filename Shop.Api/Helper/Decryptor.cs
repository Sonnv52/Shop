using System.Security.Cryptography;
using System.Text;

namespace Shop.Api.Helper
{
    public static class Decryptor
    {
        private static IConfiguration _configuration;

        static Decryptor()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();
        }

        public static string DecryptString( this string hashCode)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(hashCode);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_configuration["Key_Decrypt"] ?? "");
                    aes.IV = new byte[16];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        byte[] decryptedBytes = ms.ToArray();
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return "Cant convert!!";
            }
        }
    }
}
