using System;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System.IO;

class CEncryption
{

	public static string Scramble(string text ) {

	//	Debug.Log ("NULL TERMINATOR: " + (byte)("\0" [0]));

#if  UNITY_FLASH
		byte[] clear = Encoding.ASCII.GetBytes (text);
		string output = Encoding.ASCII.GetString( Xor(clear) );
#else
		byte[] clear = Encoding.Unicode.GetBytes (text);
		string output = Encoding.Unicode.GetString( Xor(clear) );
#endif
		//Debug.Log ( "SCRAMBLE: " + text + "len(" + text.Length + " => " + output + " len(" + output.Length + ")");
		return output;
	}

	public static string Unscramble(string text ) {

#if  UNITY_FLASH
		byte[] crypt = Encoding.ASCII.GetBytes (text);
		string output = Encoding.ASCII.GetString( Xor(crypt) );
#else
		byte[] crypt = Encoding.Unicode.GetBytes (text);
		string output = Encoding.Unicode.GetString( Xor(crypt) );
#endif


		//Debug.Log ( "UNSCRAMBLE: " + text + "len(" + text.Length + " => " + output + " len(" + output.Length + ")");
		return output;
	}

	static byte[] GetBytes(string str)
	{
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	static string GetString(byte[] bytes)
	{
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}

	//public static string Unscramble(string text ) {
	//	byte[] clear = Encoding.UTF8.GetBytes (text);
	//	return Encoding.UTF8.GetString( Xor( clear) );
	//}

	public static byte[] Xor(byte[] clearTextBytes )
	{

		byte[] key = GenKey (clearTextBytes.Length);

		//Debug.Log ("KEY : " + Encoding.Unicode.GetString (key));

		MemoryStream ms = new MemoryStream();
	
		for (int i = 0; i < clearTextBytes.Length; i++) {

			byte b = (byte)(  (clearTextBytes [i] ^ key [i]) );

			//if (b == 0 ) {
			//	b = key [i];
				//Debug.Log ("GOt NULL BYTE FROM KEY: " + key [i] + ", BYTE: " + clearTextBytes [i]);
			//}

			ms.WriteByte ( b );
		}

		byte[] output =  ms.ToArray();
		//Debug.Log ("SCRAM OUT: " + output.Length);
		return output;

	}

	const string keyA = "YA3I2IUD1Kz5CLO528AesNMRk9peuyOS9elHmmjKadDked6Ddx9atB1T6i3BcxXo5VlnBiY", keyB = "PTfrVYVoKwOwlHQmy1rkSWhJzNheA";

	static byte[] GenKey( int length ) {
		byte[] key = new byte[length];
		for( int i = 0 ; i < length ; i++ ) {
			//key [i] = (byte)keyA [i % keyA.Length];//(keyA [i % keyA.Length] ^ keyB [i % keyB.Length]);
			key [i] = (byte)(keyA [i % keyA.Length] ^ keyB [i % keyB.Length]);
			if (key [i] == byte.MinValue)
				key [i] = 137;
			if (key [i] == byte.MaxValue)
				key [i] = 137;
		}
		return key;
	}

}