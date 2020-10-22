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

		// This is ass but that's how it is on this bitch of an earth
		private static string RemapKeys(string dirtyJson)
		{
			dirtyJson = dirtyJson.Replace("player", "Name");
			dirtyJson = dirtyJson.Replace("team", "Team");
			dirtyJson = dirtyJson.Replace("\"pos\"", "\"Position\"");
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
			dirtyJson = dirtyJson.Replace("pass_yds", "PassingYards");
			dirtyJson = dirtyJson.Replace("pass_long", "LongestPass");
			dirtyJson = dirtyJson.Replace("pass_sacked", "SacksTaken");
			dirtyJson = dirtyJson.Replace("comebacks", "FourthQuarterCombacks");
			dirtyJson = dirtyJson.Replace("rush_att", "RushAttempts");
			dirtyJson = dirtyJson.Replace("rush_yds", "Yards");
			dirtyJson = dirtyJson.Replace("rush_first_down", "FirstDowns");
			dirtyJson = dirtyJson.Replace("rush_long", "LongestRush");
			dirtyJson = dirtyJson.Replace("def_int", "Interceptions");
			dirtyJson = dirtyJson.Replace("def_int_yds", "InterceptionYards");
			dirtyJson = dirtyJson.Replace("def_int_td", "InterceptionTouchdowns");
			dirtyJson = dirtyJson.Replace("def_int_long", "LongestInterceptionReturn");
			dirtyJson = dirtyJson.Replace("pass_defended", "PassesDefended");
			dirtyJson = dirtyJson.Replace("fumbles_forces", "ForcedFumbles");
			dirtyJson = dirtyJson.Replace("fumbles_rec", "FumblesRecovered");
			dirtyJson = dirtyJson.Replace("fumbles_rec_yds", "FumbleYards");
			dirtyJson = dirtyJson.Replace("fumbles_rec_td", "FumbleTouchdowns");
			dirtyJson = dirtyJson.Replace("sacks", "Sacks");
			dirtyJson = dirtyJson.Replace("tackles_assists", "AssistedTackles");
			dirtyJson = dirtyJson.Replace("tackles_loss", "TacklesForLoss");
			dirtyJson = dirtyJson.Replace("safety_md", "Safety");
			return dirtyJson;
		}

		public static T ConvertFromJson<T>(string json) where T : Player => JsonSerializer.Deserialize<T>(RemapKeys(json));

		public static Player ConvertFromJson(this PlayerType type, string json)
		{
			switch (type)
			{
				case PlayerType.Defense:
					return ConvertFromJson<DefensePlayer>(json);
				case PlayerType.Rushing:
					return ConvertFromJson<DefensePlayer>(json);
				case PlayerType.Receiving:
					return ConvertFromJson<DefensePlayer>(json);
				default:
				case PlayerType.Passing:
					return ConvertFromJson<DefensePlayer>(json);
			}
		}
	}
}
