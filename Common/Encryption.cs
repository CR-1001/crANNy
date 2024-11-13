/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Text;
    using System.IO;
    using System.Security.Cryptography;

    public static class Encryption
    {
        private static readonly byte[] Salt
            = new byte[]
            {
                0x42, 0x7a, 0x64, 0x69, 
                0x23, 0x4b, 0x65, 0x68, 
                0x74, 0x6c, 0x66, 0x67, 0x7c
            };

        private static byte[] Encrypt(byte[] clearText, byte[] key, byte[] initializationVector)
        {
            var memoryStream = new MemoryStream();

            var encryptionAlgorithm = Rijndael.Create();
            encryptionAlgorithm.Key = key;
            encryptionAlgorithm.IV = initializationVector;

            var cryptoStream = new CryptoStream(memoryStream, encryptionAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(clearText, 0, clearText.Length);
            cryptoStream.Close();

            byte[] encryptedData = memoryStream.ToArray();

            return encryptedData;
        }

        public static string Encrypt(string clearText, string password)
        {
            if (password == null) return clearText;

            var clearBytes = Encoding.UTF8.GetBytes(clearText);

            var passwordDeriveBytes = new PasswordDeriveBytes(password, Salt);

            var encryptedData 
                = Encrypt(
                    clearBytes, 
                    passwordDeriveBytes.GetBytes(32), 
                    passwordDeriveBytes.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }

        private static byte[] Decrypt(byte[] cipher, byte[] key, byte[] initializationVector)
        {
            var memoryStream = new MemoryStream();

            var encryptionAlgorithm = Rijndael.Create();
            encryptionAlgorithm.Key = key;
            encryptionAlgorithm.IV = initializationVector;

            var cryptoStream = new CryptoStream(
                memoryStream, encryptionAlgorithm.CreateDecryptor(), 
                CryptoStreamMode.Write);

            cryptoStream.Write(cipher, 0, cipher.Length);
            cryptoStream.Close();

            byte[] decryptedData = memoryStream.ToArray();
            return decryptedData;
        }

        public static string Decrypt(string cipherText, string password)
        {
            if (password == null) return cipherText;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            var passwordDeriveBytes = new PasswordDeriveBytes(password, Salt);

            byte[] decryptedData = Decrypt(
                cipherBytes, 
                passwordDeriveBytes.GetBytes(32), 
                passwordDeriveBytes.GetBytes(16));

            return Encoding.UTF8.GetString(decryptedData);
        }

    }
}
