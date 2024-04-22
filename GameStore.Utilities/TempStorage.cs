using System.Text;

namespace GameStore.Utilities
{
	public static class TempStorage
	{
		public static readonly string TempFolder = Path.Combine(Path.GetTempPath(), AppDomain.CurrentDomain.FriendlyName);

		public static FileStream GetFileStream(string filename)
		{
			var path = Path.Combine(TempFolder, filename);

			var directory = Path.GetDirectoryName(path);
			if (!Path.Exists(directory))
				CreateDirectory(directory);

			return File.Create(path);
		}
		public static string ReadAllText(string filename) => File.ReadAllText(Path.Combine(TempFolder, filename), Encoding.UTF32);
		public static void Delete(string name) => File.Delete(Path.Combine(TempFolder, $"{name}.txt"));

		private static void CreateDirectory(string? path)
		{
			if (string.IsNullOrEmpty(path))
				return;

			var directory = Path.GetDirectoryName(path);
			if (!Path.Exists(directory) && !string.IsNullOrEmpty(directory))
				Directory.CreateDirectory(directory);

			Directory.CreateDirectory(path);
		}
		public static void Write(string name, string data)
		{
			using var fileStream = GetFileStream($"{name}.txt");

			fileStream.Write(Encoding.UTF32.GetBytes(data));
		}

		public static string Read(string name)
		{
			try
			{
				return ReadAllText($"{name}.txt");
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}
