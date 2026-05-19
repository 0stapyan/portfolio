using System.Security.Cryptography;

namespace CryptographyAes;

public class AesCfb : BaseAesCipher
{
	public AesCfb(byte[] key, byte[] iv) : base(key, iv) { }

	public override byte[] Encrypt(byte[] plainText)
	{
		using Aes aes = Aes.Create();
		aes.Mode = CipherMode.CFB;
		aes.FeedbackSize = 128;
		aes.Padding = PaddingMode.None;
		aes.Key = Key;
		aes.IV = Iv;

		using ICryptoTransform encryptor = aes.CreateEncryptor();

		return encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
	}

	public override byte[] Decrypt(byte[] cipherText)
	{
		using Aes aes = Aes.Create();
		aes.Mode = CipherMode.CFB;
		aes.FeedbackSize = 128;
		aes.Padding = PaddingMode.None;
		aes.Key = Key;
		aes.IV = Iv;

		using ICryptoTransform decryptor = aes.CreateDecryptor();

		return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
	}
}
