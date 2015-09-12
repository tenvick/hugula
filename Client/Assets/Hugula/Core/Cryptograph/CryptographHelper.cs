﻿// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

[SLua.CustomLuaClass]
public class CryptographHelper {
	
	/// <summary>
	/// Md5s  base64 string.
	/// </summary>
	/// <returns>
	/// The base64 string.
	/// </returns>
	/// <param name='source'>
	/// Source.
	/// </param>
	public static string CrypfString(string source,string key="")
	{
		byte[] inputs=Encoding.UTF8.GetBytes(source);
        byte[] hash = inputs;//Md5Instance.ComputeHash(inputs);
		string outStr=System.Convert.ToBase64String(hash);
		outStr=outStr.Replace("=","");
		outStr=outStr.Replace(@"/","-");
		return outStr;
	}

	public static byte[] Decrypt(byte[] encryptedBytes, byte[] Key,byte[] IV)  
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


	public static byte[] Encrypt(byte[] PlainBytes, byte[] Key,byte[] IV )  
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
					csEncrypt.Write(PlainBytes,0,PlainBytes.Length);
					csEncrypt.FlushFinalBlock();
					encrypted = msEncrypt.ToArray();
				}
			}
		}

		// Return the encrypted bytes from the memory stream.
		return encrypted;
	}  
	
    ///// <summary>
    ///// Gets the zip Password
    ///// </summary>
    ///// <value>
    ///// The zip PW.
    ///// </value>
    //internal static string zipPWD
    //{
    //    get{
    //        return CrypfString("tap4fun99@@","99@");
    //    }
    //}
	
	#region memeber 
    //private static MD5 md5;
    //public static MD5 Md5Instance
    //{
    //    get{
    //        if(md5==null)
    //            md5=MD5.Create();
    //        return md5;
    //    }
    //}

	#endregion
}

