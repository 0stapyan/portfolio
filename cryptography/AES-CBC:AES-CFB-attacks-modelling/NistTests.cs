namespace CryptographyAes;

public static class NistTests
{
	#region AES-ECB

	// AES-ECB: https://csrc.nist.gov/CSRC/media/Projects/Cryptographic-Standards-and-Guidelines/documents/examples/AES_ECB.pdf

	public static bool TestAesEcbEncryption()
	{
		byte[] key = Convert.FromHexString("603DEB1015CA71BE2B73AEF0857D77811F352C073B6108D72D9810A30914DFF4");
		byte[] plainText = Convert.FromHexString("6BC1BEE22E409F96E93D7E117393172AAE2D8A571E03AC9C9EB76FAC45AF8E5130C81C46A35CE411E5FBC1191A0A52EFF69F2445DF4F9B17AD2B417BE66C3710");
		byte[] cipherText = Convert.FromHexString("F3EED1BDB5D2A03C064B5A7E3DB181F8591CCB10D410ED26DC5BA74A31362870B6ED21B99CA6F4F9F153E7B1BEAFED1D23304B7A39F9F3FF067D8D8F9E24ECC7");

		AesEcb aesEcb = new AesEcb(key, Array.Empty<byte>());
		byte[] encrypted = aesEcb.Encrypt(plainText);
		return encrypted.SequenceEqual(cipherText);
	}

	public static bool TestAesEcbDecryption()
	{
		byte[] key = Convert.FromHexString("603DEB1015CA71BE2B73AEF0857D77811F352C073B6108D72D9810A30914DFF4");
		byte[] plainText = Convert.FromHexString("6BC1BEE22E409F96E93D7E117393172AAE2D8A571E03AC9C9EB76FAC45AF8E5130C81C46A35CE411E5FBC1191A0A52EFF69F2445DF4F9B17AD2B417BE66C3710");
		byte[] cipherText = Convert.FromHexString("F3EED1BDB5D2A03C064B5A7E3DB181F8591CCB10D410ED26DC5BA74A31362870B6ED21B99CA6F4F9F153E7B1BEAFED1D23304B7A39F9F3FF067D8D8F9E24ECC7");

		AesEcb aesEcb = new AesEcb(key, Array.Empty<byte>());
		byte[] decrypted = aesEcb.Decrypt(cipherText);
		return decrypted.SequenceEqual(plainText);
	}

	#endregion

	#region AES-CBC

	// AES-CBC: https://csrc.nist.gov/CSRC/media/Projects/Cryptographic-Standards-and-Guidelines/documents/examples/AES_CBC.pdf

	public static bool TestAesCbcEncryption()
	{
		byte[] key = Convert.FromHexString("603DEB1015CA71BE2B73AEF0857D77811F352C073B6108D72D9810A30914DFF4");
		byte[] iv = Convert.FromHexString("000102030405060708090A0B0C0D0E0F");
		byte[] plainText = Convert.FromHexString("6BC1BEE22E409F96E93D7E117393172AAE2D8A571E03AC9C9EB76FAC45AF8E5130C81C46A35CE411E5FBC1191A0A52EFF69F2445DF4F9B17AD2B417BE66C3710");
		byte[] cipherText = Convert.FromHexString("F58C4C04D6E5F1BA779EABFB5F7BFBD69CFC4E967EDB808D679F777BC6702C7D39F23369A9D9BACFA530E26304231461B2EB05E2C39BE9FCDA6C19078C6A9D1B");

		AesCbc aesCbc = new AesCbc(key, iv);
		byte[] encrypted = aesCbc.Encrypt(plainText);
		return encrypted.SequenceEqual(cipherText);
	}

	public static bool TestAesCbcDecryption()
	{
		byte[] key = Convert.FromHexString("603DEB1015CA71BE2B73AEF0857D77811F352C073B6108D72D9810A30914DFF4");
		byte[] iv = Convert.FromHexString("000102030405060708090A0B0C0D0E0F");
		byte[] plainText = Convert.FromHexString("6BC1BEE22E409F96E93D7E117393172AAE2D8A571E03AC9C9EB76FAC45AF8E5130C81C46A35CE411E5FBC1191A0A52EFF69F2445DF4F9B17AD2B417BE66C3710");
		byte[] cipherText = Convert.FromHexString("F58C4C04D6E5F1BA779EABFB5F7BFBD69CFC4E967EDB808D679F777BC6702C7D39F23369A9D9BACFA530E26304231461B2EB05E2C39BE9FCDA6C19078C6A9D1B");

		AesCbc aesCbc = new AesCbc(key, iv);
		byte[] decrypted = aesCbc.Decrypt(cipherText);
		return decrypted.SequenceEqual(plainText);
	}

	#endregion

	#region AES-CFB

	// AES-CFB: https://csrc.nist.gov/CSRC/media/Projects/Cryptographic-Standards-and-Guidelines/documents/examples/AES_CFB.pdf

	public static bool TestAesCfbEncryption()
	{
		byte[] key = Convert.FromHexString("603DEB1015CA71BE2B73AEF0857D77811F352C073B6108D72D9810A30914DFF4");
		byte[] iv = Convert.FromHexString("000102030405060708090A0B0C0D0E0F");
		byte[] plainText = Convert.FromHexString("6BC1BEE22E409F96E93D7E117393172AAE2D8A571E03AC9C9EB76FAC45AF8E5130C81C46A35CE411E5FBC1191A0A52EFF69F2445DF4F9B17AD2B417BE66C3710");
		byte[] cipherText = Convert.FromHexString("DC7E84BFDA79164B7ECD8486985D386039FFED143B28B1C832113C6331E5407BDF10132415E54B92A13ED0A8267AE2F975A385741AB9CEF82031623D55B1E471");

		AesCfb aesCfb = new AesCfb(key, iv);
		byte[] encrypted = aesCfb.Encrypt(plainText);
		return encrypted.SequenceEqual(cipherText);
	}

	public static bool TestAesCfbDecryption()
	{
		byte[] key = Convert.FromHexString("603DEB1015CA71BE2B73AEF0857D77811F352C073B6108D72D9810A30914DFF4");
		byte[] iv = Convert.FromHexString("000102030405060708090A0B0C0D0E0F");
		byte[] plainText = Convert.FromHexString("6BC1BEE22E409F96E93D7E117393172AAE2D8A571E03AC9C9EB76FAC45AF8E5130C81C46A35CE411E5FBC1191A0A52EFF69F2445DF4F9B17AD2B417BE66C3710");
		byte[] cipherText = Convert.FromHexString("DC7E84BFDA79164B7ECD8486985D386039FFED143B28B1C832113C6331E5407BDF10132415E54B92A13ED0A8267AE2F975A385741AB9CEF82031623D55B1E471");

		AesCfb aesCfb = new AesCfb(key, iv);
		byte[] decrypted = aesCfb.Decrypt(cipherText);
		return decrypted.SequenceEqual(plainText);
	}

	#endregion
}
