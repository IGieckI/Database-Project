using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Utilities
{
    /// <summary>
    /// Utility class for basic operation with passwords.
    /// </summary>
    public static class PasswordManager
    {
        private const string PASSWORD_KEY = "nw0SKdEM2uJQCvsFS6bcwWyR4ATiyYDHD7sBp6Ag3go=";

        /// <summary>
        /// Check if the encrypted password is the password inserted
        /// </summary>
        /// <param name="input">Entered password</param>
        /// <param name="encryptedPassword">Encrypted password</param>
        /// <returns>True if the inserted password is correct, false otherwise</returns>
        public static bool CheckPassword(string input, string encryptedPassword)
        {
            return EncryptPassword(input) == encryptedPassword;
        }

        /// <summary>
        /// Encrypt the inserted password
        /// </summary>
        /// <param name="password">Give password</param>
        /// <returns>Encrypted password</returns>
        public static string EncryptPassword(string password)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(PASSWORD_KEY, new byte[]
                {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }

            return password;
        }

        /// <summary>
        /// Decrypt the entered password
        /// </summary>
        /// <param name="encryptedPassword">Encrypted password</param>
        /// <returns>The decrypted password</returns>
        private static string DecryptPassword(string encryptedPassword)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedPassword);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(PASSWORD_KEY, new byte[]
                {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encryptedPassword = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return encryptedPassword;
        }
    }
}
