namespace GameStore.Utilities
{
	public static class EasyEncryption
	{
		public static string Encrypt(string input, string key)
		{
			var inputCharacters = input.ToCharArray();
			var keyCharacters = key.ToCharArray();
			var encryptedCharacters = new char[inputCharacters.Length];

			for (int i = 0; i < inputCharacters.Length; i++)
			{
				encryptedCharacters[i] = (char)((byte)inputCharacters[i] ^ keyCharacters[i % keyCharacters.Length]);
			}

			return new String(encryptedCharacters);
		}

		public static string Decrypt(string input, string key) => Encrypt(input, key);
	}
}
