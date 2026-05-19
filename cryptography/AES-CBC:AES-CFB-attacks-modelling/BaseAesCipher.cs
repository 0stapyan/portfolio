namespace CryptographyAes;

public abstract class BaseAesCipher
{
	protected readonly byte[] Key;
	protected readonly byte[] Iv;

	protected BaseAesCipher(byte[] key, byte[] iv)
	{
		Key = key;
		Iv = iv;
	}

	public abstract byte[] Encrypt(byte[] plainText);
	public abstract byte[] Decrypt(byte[] cipherText);

	// Used for padding mode.
	// It should be disabled for NIST test vectors
	// or other data that is a multiple of 16 bytes
	protected bool IsDataMultipleTo16Bytes(byte[] data)
	{
		return data.Length % 16 == 0;
	}
}
