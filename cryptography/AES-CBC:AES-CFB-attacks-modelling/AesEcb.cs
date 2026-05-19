using System.Security.Cryptography;

namespace CryptographyAes;

public class AesEcb : BaseAesCipher
{
	public AesEcb(byte[] key, byte[] iv) : base(key, iv) { }

	public override byte[] Encrypt(byte[] plainText)
	{
		using Aes aes = Aes.Create();
		aes.Mode = CipherMode.ECB;
		aes.Padding = IsDataMultipleTo16Bytes(plainText) ? PaddingMode.None : PaddingMode.PKCS7;
		aes.Key = Key;

		using ICryptoTransform encryptor = aes.CreateEncryptor();

		return encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
	}

	public override byte[] Decrypt(byte[] cipherText)
	{
		using Aes aes = Aes.Create();
		aes.Mode = CipherMode.ECB;
		aes.Padding = IsDataMultipleTo16Bytes(cipherText) ? PaddingMode.None : PaddingMode.PKCS7;
		aes.Key = Key;

		using ICryptoTransform decryptor = aes.CreateDecryptor();

		return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
	}
}
