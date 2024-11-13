/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Text;
    using System.Security.Cryptography;

    public enum HashAlgorithmType
    {
        Sha256,
        Sha1,
    }


    public static class HashCode
    {
        public static String GetCryptographicHashCode(
            String input, HashAlgorithmType hashAlgorithmType)
        {
            HashAlgorithm hashAlgorithm;

            switch (hashAlgorithmType)
            {
                case HashAlgorithmType.Sha256:
                    hashAlgorithm = new SHA256Managed();
                    break;
                case HashAlgorithmType.Sha1:
                    hashAlgorithm = new SHA1Managed();
                    break;
                default:
                    throw new NotSupportedException();
            }

            using (hashAlgorithm)
            {
                byte[] hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }

        }
    }
}
