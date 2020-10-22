using static System.IO.Directory;
using System.Threading.Tasks;

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
	}
}