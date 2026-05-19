using System;
using System.Text;

namespace CryptographyAes
{
	public static class CfbBitFlipAttack
	{
		public static void Simulate()
		{
			byte[] key = KeyGenerator.GenerateKey(128);
			byte[] iv = KeyGenerator.GenerateIv();
			AesCfb aesCfb = new AesCfb(key, iv);

			string originalMessage = "This is a secret message!";
			byte[] plainText = Encoding.UTF8.GetBytes(originalMessage);

			// Шифрування повідомлення
			byte[] cipherText = aesCfb.Encrypt(plainText);

			// Зміна одного біту в зашифрованому повідомленні
			cipherText[10] ^= 0x01; // Зміна біту

			// Розшифрування зміненого повідомлення
			byte[] decrypted = aesCfb.Decrypt(cipherText);
			string modifiedMessage = Encoding.UTF8.GetString(decrypted);

			Console.WriteLine("Original message: " + originalMessage);
			Console.WriteLine("Modified message: " + modifiedMessage);
		}
	}
}
