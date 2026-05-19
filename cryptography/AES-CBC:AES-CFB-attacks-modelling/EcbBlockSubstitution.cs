using System;
using System.Text;

namespace CryptographyAes
{
	public static class EcbBlockSubstitution
	{
		public static void Simulate()
		{
			byte[] key = KeyGenerator.GenerateKey(128);
			AesEcb aesEcb = new AesEcb(key, Array.Empty<byte>());

			string originalMessage = "This is a secret message!";
			byte[] plainText = Encoding.UTF8.GetBytes(originalMessage);

			// Шифрування повідомлення
			byte[] cipherText = aesEcb.Encrypt(plainText);

			// Підміна одного блоку
			byte[] maliciousBlock = new byte[16];
			Array.Copy(cipherText, 16, maliciousBlock, 0, 16); // Копіюємо другий блок

			// Підміна першого блоку
			Array.Copy(maliciousBlock, 0, cipherText, 0, 16);

			// Розшифрування зміненого повідомлення
			byte[] decrypted = aesEcb.Decrypt(cipherText);
			string modifiedMessage = Encoding.UTF8.GetString(decrypted);

			Console.WriteLine("Original message: " + originalMessage);
			Console.WriteLine("Modified message: " + modifiedMessage);
		}
	}
}
