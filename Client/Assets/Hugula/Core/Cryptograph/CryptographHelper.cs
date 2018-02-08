// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

namespace Hugula.Cryptograph
{
    [SLua.CustomLuaClass]
    public class CryptographHelper
    {
        private static SHA1CryptoServiceProvider KeepClass()
        {
            SHA1CryptoServiceProvider p = new SHA1CryptoServiceProvider();
            return p;
        }

        /// <summary>
        /// Md5s  base64 string.
        /// </summary>
        /// <returns>
        /// The base64 string.
        /// </returns>
        /// <param name='source'>
        /// Source.
        /// </param>
        public static string CrypfString(string source, string key = "")
        {
            byte[] inputs = Encoding.UTF8.GetBytes(source);
            byte[] hash = inputs;//Md5Instance.ComputeHash(inputs);
            string outStr = Convert.ToBase64String(hash);
            return outStr;
        }

        public static byte[] Decrypt(byte[] encryptedBytes, byte[] Key, byte[] IV)
        {

            // Create an RijndaelManaged object
            // with the specified key and IV.
            byte[] original = null;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            byte[] Buffer = new byte[1024];
                            int readBytes = 0;
                            while ((readBytes = csDecrypt.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }

                            original = originalMemory.ToArray();
                        }
                    }
                }

            }

            return original;

        }

        public static byte[] Encrypt(byte[] PlainBytes, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(PlainBytes, 0, PlainBytes.Length);
                        csEncrypt.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        #region md5
        private static MD5 md5;
        private static MD5 Md5Instance
        {
            get
            {
                if (md5 == null)
                    md5 = new MD5CryptoServiceProvider(); //MD5.Create()
                return md5;
            }
        }

        /// <summary>
        /// string md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5String(string source)
        {
            return Md5Bytes(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// byte[] md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5Bytes(byte[] inputs)
        {
            byte[] result = Md5Instance.ComputeHash(inputs);
            string md5str = string.Empty;
            md5str = System.BitConverter.ToString(result).Replace("-",string.Empty).ToLower();
            return md5str;
        }

        /// <summary>
        /// byte[] md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5BytesTo32(byte[] inputs)
        {
            return Md5Bytes(inputs);
        }

        #endregion

        #region base64
        public static string Base64ToString(byte[] src)
        {
            return Convert.ToBase64String(src);
        }

        public static byte[] Base64ToBinary(string src)
        {
            //byte[] a = Convert.FromBase64String (src);
            return Convert.FromBase64String(src);
        }
        #endregion
    }
}

