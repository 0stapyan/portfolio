using System.Security.Cryptography;

namespace CryptographyAes;

public class KeyGenerator
{
	public static byte[] GenerateKey(int keySize)
	{
		byte[] key = new byte[keySize / 8]; // Divide by 8 to get number of bytes, while keySize is a number of bits
		using RandomNumberGenerator generator = RandomNumberGenerator.Create();
		generator.GetBytes(key);
		return key;
	}

	public static byte[] GenerateIv()
	{
		byte[] iv = new byte[16]; // 128/8 = 16 bytes
		using RandomNumberGenerator generator = RandomNumberGenerator.Create();
		generator.GetBytes(iv);
		return iv;
	}
}
