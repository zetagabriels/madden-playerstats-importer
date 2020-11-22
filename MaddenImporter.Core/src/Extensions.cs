using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using MaddenImporter.Models.Player;

namespace MaddenImporter.Core
{
    public static class Extensions
    {
        public static string GetSeasonUrl(int year, PlayerType playerType) => $"https://www.pro-football-reference.com/years/{year}/{playerType.ToString().ToLower()}.htm";

        public static async Task CheckOrCreatePath(string path)
        {
            await Task.Run(() =>
            {
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                    System.Console.WriteLine($"Created directory {path}");
                }
            });
        }

        public static List<T> GetPlayersOfType<T>(List<Player> players) where T : Player
        {
            return players.Where(p => p is T).Select(p => (T)p).ToList();
        }

        private static void WritePlayerList<T>(string path, string filePrefix, List<T> players) where T : Player
        {
            System.IO.File.WriteAllText(path + $"/{filePrefix}.players.json", JsonSerializer.Serialize(players));
        }

        internal static void WritePlayersList(string path, List<Player> players)
        {
            WritePlayerList(path, "passing", GetPlayersOfType<PassingPlayer>(players));
            WritePlayerList(path, "defense", GetPlayersOfType<DefensePlayer>(players));
            WritePlayerList(path, "kicking", GetPlayersOfType<KickingPlayer>(players));
            WritePlayerList(path, "rushing", GetPlayersOfType<RushingPlayer>(players));
            WritePlayerList(path, "receiving", GetPlayersOfType<ReceivingPlayer>(players));
            WritePlayerList(path, "returning", GetPlayersOfType<ReturningPlayer>(players));
        }

        // This is ass but that's how it is on this bitch of an earth
        private static string RemapSeasonalKeys(string dirtyJson)
        {
            using var inputDoc = JsonDocument.Parse(dirtyJson);
            var json = inputDoc.RootElement;
            var dict = new Dictionary<string, object>();
            void TryAddKeyValuePair(string keyName, string valueKeyName)
            {
                var ok = json.TryGetProperty(valueKeyName, out var value);
                if (ok) dict.TryAdd(keyName, value);
            }

            TryAddKeyValuePair("Name", "player");
            TryAddKeyValuePair("Team", "team");
            TryAddKeyValuePair("Position", "pos");
            TryAddKeyValuePair("GamesPlayed", "g");
            TryAddKeyValuePair("GamesStarted", "gs");
            TryAddKeyValuePair("Receptions", "rec");
            TryAddKeyValuePair("LongestReception", "rec_long");
            TryAddKeyValuePair("ReceivingTouchdowns", "rec_td");
            TryAddKeyValuePair("YardsReceived", "rec_yds");
            TryAddKeyValuePair("Fumbles", "fumbles");
            TryAddKeyValuePair("FirstDowns", "rec_first_down");
            TryAddKeyValuePair("Completions", "cmp");
            TryAddKeyValuePair("Completions", "pass_cmp");
            TryAddKeyValuePair("AttemptedPasses", "pass_att");
            TryAddKeyValuePair("PassingTouchdowns", "pass_td");
            TryAddKeyValuePair("Interceptions", "pass_int");
            TryAddKeyValuePair("FirstDowns", "pass_first_down");
            TryAddKeyValuePair("PassingYards", "pass_yds");
            TryAddKeyValuePair("LongestPass", "pass_long");
            TryAddKeyValuePair("SacksTaken", "pass_sacked");
            TryAddKeyValuePair("FourthQuarterCombacks", "comebacks");
            TryAddKeyValuePair("RushAttempts", "rush_att");
            TryAddKeyValuePair("RushingYards", "rush_yds");
            TryAddKeyValuePair("RushTouchdowns", "rush_td");
            TryAddKeyValuePair("FirstDowns", "rush_first_down");
            TryAddKeyValuePair("LongestRush", "rush_long");
            TryAddKeyValuePair("Interceptions", "def_int");
            TryAddKeyValuePair("InterceptionYards", "def_int_yds");
            TryAddKeyValuePair("InterceptionTouchdowns", "def_int_td");
            TryAddKeyValuePair("LongestInterceptionReturn", "def_int_long");
            TryAddKeyValuePair("PassesDefended", "pass_defended");
            TryAddKeyValuePair("ForcedFumbles", "fumbles_forced");
            TryAddKeyValuePair("FumblesRecovered", "fumbles_rec");
            TryAddKeyValuePair("FumbleYards", "fumbles_rec_yds");
            TryAddKeyValuePair("FumbleTouchdowns", "fumbles_rec_td");
            TryAddKeyValuePair("Sacks", "sacks");
            TryAddKeyValuePair("SoloTackles", "tackles_solo");
            TryAddKeyValuePair("AssistedTackles", "tackles_assists");
            TryAddKeyValuePair("TacklesForLoss", "tackles_loss");
            TryAddKeyValuePair("Safety", "safety_md");
            TryAddKeyValuePair("PuntAttempts", "punt");
            TryAddKeyValuePair("PuntYards", "punt_yds");
            TryAddKeyValuePair("FieldGoalsAttempted", "fga");
            TryAddKeyValuePair("FieldGoalsMade", "fgm");
            TryAddKeyValuePair("ExtraPointsAttempted", "xpa");
            TryAddKeyValuePair("ExtraPointsMade", "xpm");
            TryAddKeyValuePair("KickReturns", "kick_ret");
            TryAddKeyValuePair("KickReturnYards", "kick_ret_yds");
            TryAddKeyValuePair("KickReturnTouchdowns", "kick_ret_td");
            TryAddKeyValuePair("PuntReturnAttempts", "punt_ret");
            TryAddKeyValuePair("PuntReturnYards", "punt_ret_yds");
            TryAddKeyValuePair("PuntReturnTouchdowns", "punt_ret_td");
            return JsonSerializer.Serialize(dict);
        }

        public static T ConvertFromJson<T>(string json) where T : Player => JsonSerializer.Deserialize<T>(RemapSeasonalKeys(json));

        public static Player ConvertFromJson(this PlayerType type, string json)
        {
            switch (type)
            {
                case PlayerType.Defense:
                    return ConvertFromJson<DefensePlayer>(json);
                case PlayerType.Rushing:
                    return ConvertFromJson<RushingPlayer>(json);
                case PlayerType.Receiving:
                    return ConvertFromJson<ReceivingPlayer>(json);
                case PlayerType.Kicking:
                    return ConvertFromJson<KickingPlayer>(json);
                case PlayerType.Returns:
                    return ConvertFromJson<ReturningPlayer>(json);
                default:
                case PlayerType.Passing:
                    return ConvertFromJson<PassingPlayer>(json);
            }
        }
    }
}
