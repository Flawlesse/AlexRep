using System.Text;
using System.Security.Cryptography;
using System.IO.Compression;
using System.IO;
using System;

namespace STW_Service
{
    public static class Encryptor
    {
        private static readonly DESCryptoServiceProvider crypto = new DESCryptoServiceProvider
        {
            Key = Encoding.ASCII.GetBytes("pQ1IS5hq"),
            IV = Encoding.ASCII.GetBytes("pQ1IS5hq")
        };

        public static void Encrypt(Stream sourceStream, Stream targetEncryptedStream)
        {
            try
            {
                using (var ecStream = new CryptoStream(targetEncryptedStream, crypto.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    sourceStream.CopyTo(ecStream);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter errorStream = new StreamWriter(new FileStream(Archivator.errorLog, FileMode.OpenOrCreate)))
                {
                    errorStream.Write(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        public static void Decrypt(Stream sourceEncryptedStream, Stream targetDecryptedStream)
        {
            try
            {
                using (var dcStream = new CryptoStream(sourceEncryptedStream, crypto.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    dcStream.CopyTo(targetDecryptedStream);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter errorStream = new StreamWriter(new FileStream(Archivator.errorLog, FileMode.OpenOrCreate)))
                {
                    errorStream.Write(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }
}
