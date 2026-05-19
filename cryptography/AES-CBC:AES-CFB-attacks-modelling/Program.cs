namespace CryptographyAes;

public class Program
{
	public static void Main()
	{
		Console.WriteLine("Testing AES implementations:");
		Console.WriteLine($"Is AES-ECB encryption correctly implemented: {NistTests.TestAesEcbEncryption()}");
		Console.WriteLine($"Is AES-ECB decryption correctly implemented: {NistTests.TestAesEcbDecryption()}");
		Console.WriteLine($"Is AES-CBC encryption correctly implemented: {NistTests.TestAesCbcEncryption()}");
		Console.WriteLine($"Is AES-CBC decryption correctly implemented: {NistTests.TestAesCbcDecryption()}");
		Console.WriteLine($"Is AES-CFB encryption correctly implemented: {NistTests.TestAesCfbEncryption()}");
		Console.WriteLine($"Is AES-CFB decryption correctly implemented: {NistTests.TestAesCfbDecryption()}");
		Console.WriteLine();

		Console.WriteLine("Simulating attacks on AES modes:");

		Console.WriteLine("CBC Bit-flipping Attack:");
		CbcBitFlipAttack.Simulate();
		Console.WriteLine();

		Console.WriteLine("CBC Reused IV Attack:");
		CbcReusedIvAttack.Simulate();
		Console.WriteLine();

		Console.WriteLine("CFB Reused IV Attack:");
		CfbReusedIvAttack.Simulate();
		Console.WriteLine();

		Console.WriteLine("CFB Bit-flipping Attack:");
		CfbBitFlipAttack.Simulate();
		Console.WriteLine();

		Console.WriteLine("ECB Pattern Analysis:");
		EcbPatternAnalysis.Simulate();
		Console.WriteLine();

		Console.WriteLine("ECB Block Substitution:");
		EcbBlockSubstitution.Simulate();
	}
}
