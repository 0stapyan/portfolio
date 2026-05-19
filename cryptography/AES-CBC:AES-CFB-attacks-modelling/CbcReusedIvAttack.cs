using System;
using System.Text;

namespace CryptographyAes
{
	public static class CbcReusedIvAttack
	{
		public static void Simulate()
		{
			byte[] key = KeyGenerator.GenerateKey(128);
			byte[] iv = KeyGenerator.GenerateIv();
			AesCbc aesCbc = new AesCbc(key, iv);

			string message1 = "First secret message!";
			string message2 = "Second secret message!";
			byte[] plainText1 = Encoding.UTF8.GetBytes(message1);
			byte[] plainText2 = Encoding.UTF8.GetBytes(message2);

			// Шифрування двох повідомлень з одним і тим самим IV
			byte[] cipherText1 = aesCbc.Encrypt(plainText1);
			byte[] cipherText2 = aesCbc.Encrypt(plainText2);

			// Аналіз результатів
			Console.WriteLine("Ciphertext 1: " + BitConverter.ToString(cipherText1));
			Console.WriteLine("Ciphertext 2: " + BitConverter.ToString(cipherText2));

			// Спробуємо знайти спільні частини
			for (int i = 0; i < cipherText1.Length; i++)
			{
				if (cipherText1[i] == cipherText2[i])
				{
					Console.WriteLine($"Possible match at byte {i}");
				}
			}
		}
	}
}
