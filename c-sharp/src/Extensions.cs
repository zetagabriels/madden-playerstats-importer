using static System.IO.Directory;
using System.Threading.Tasks;
using System.Text.Json;

namespace MaddenImporter
{
	public static class Extensions
	{
		public static string GenerateUrl(int year, PlayerType playerType)
		{
			return $"https://www.pro-football-reference.com/years/{year}/{playerType.ToString().ToLower()}.htm";
		}

		public static async Task CheckOrCreatePath(string path)
		{
			await Task.Run(() =>
			{
				if (!Exists(path))
				{
					CreateDirectory(path);
					System.Console.WriteLine($"Created directory {path}.");
				}
			});
		}

		private static string RemapKeys(string dirtyJson)
		{
			return string.Empty;
		}

		public static T ConvertFromJson<T>(string json) where T : Player
		{
			return JsonSerializer.Deserialize<T>(json);
		}
	}
}
