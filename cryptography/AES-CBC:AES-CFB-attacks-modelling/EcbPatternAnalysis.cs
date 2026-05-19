using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptographyAes
{
	public static class EcbPatternAnalysis
	{
		public static void Simulate()
		{
			byte[] key = KeyGenerator.GenerateKey(128);
			AesEcb aesEcb = new AesEcb(key, Array.Empty<byte>());

			string originalMessage = "This is a secret message! This is a secret message!";
			byte[] plainText = Encoding.UTF8.GetBytes(originalMessage);

			// Шифрування повідомлення
			byte[] cipherText = aesEcb.Encrypt(plainText);

			// Виведення шифротексту для аналізу
			Console.WriteLine("Ciphertext: " + BitConverter.ToString(cipherText));

			// Аналіз повторень
			var blocks = new List<string>();
			for (int i = 0; i < cipherText.Length; i += 16)
			{
				byte[] block = new byte[16];
				Array.Copy(cipherText, i, block, 0, 16);
				blocks.Add(BitConverter.ToString(block));
			}

			Console.WriteLine("Repeated blocks:");
			var repeatedBlocks = blocks.GroupBy(x => x).Where(g => g.Count() > 1);
			foreach (var group in repeatedBlocks)
			{
				Console.WriteLine($"Block: {group.Key}, Count: {group.Count()}");
			}
		}
	}
}
