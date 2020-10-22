using static System.IO.Directory;
using System.Threading.Tasks;
using System.Text.Json;

namespace MaddenImporter
{
	public static class Extensions
	{
		public static string GenerateUrl(int year, PlayerType playerType) => $"https://www.pro-football-reference.com/years/{year}/{playerType.ToString().ToLower()}.htm";

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

		/// <summary>This is ass but that's how it is on this bitch of an earth</summary>
		private static string RemapKeys(string dirtyJson)
		{
			dirtyJson = dirtyJson.Replace("player", "Name");
			dirtyJson = dirtyJson.Replace("team", "Team");
			dirtyJson = dirtyJson.Replace("pos", "Position");
			dirtyJson = dirtyJson.Replace("fumbles", "Fumbles");
			dirtyJson = dirtyJson.Replace("\"g\"", "\"GamesPlayed\"");
			dirtyJson = dirtyJson.Replace("\"gs\"", "\"GamesStarted\"");
			dirtyJson = dirtyJson.Replace("rec", "Receptions");
			dirtyJson = dirtyJson.Replace("rec_td", "LongestReception");
			dirtyJson = dirtyJson.Replace("rec_first_down", "FirstDowns");
			dirtyJson = dirtyJson.Replace("rec_long", "LongestReception");
			dirtyJson = dirtyJson.Replace("cmp", "Completions");
			dirtyJson = dirtyJson.Replace("pass_att", "AttemptedPasses");
			dirtyJson = dirtyJson.Replace("pass_td", "PassingTouchdowns");
			dirtyJson = dirtyJson.Replace("pass_int", "Interceptions");
			dirtyJson = dirtyJson.Replace("pass_first_down", "FirstDowns");
			return dirtyJson;
		}

		public static T ConvertFromJson<T>(string json) where T : Player => JsonSerializer.Deserialize<T>(RemapKeys(json));
	}
}
