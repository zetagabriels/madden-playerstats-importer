using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using MaddenImporter.Models.Player;
using AngleSharp;

namespace MaddenImporter.Core
{
    public static class Extensions
    {

        internal static IBrowsingContext GetDefaultBrowser()
        {
            var config = AngleSharp.Configuration.Default.WithDefaultCookies().WithDefaultLoader();
            return AngleSharp.BrowsingContext.New(config);
        }

        public static void CheckOrCreatePath(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                System.Console.WriteLine($"Created directory {path}");
            }
        }

        public static List<T> GetPlayersOfType<T>(List<Player> players) where T : Player => players.Where(p => p is T).Select(p => (T)p).ToList();

        internal static readonly Dictionary<PlayerType, string> PlayerPositions = new Dictionary<PlayerType, string>{
            { PlayerType.Defense, "Defense" },
            { PlayerType.Kicking, "Kicking" },
            { PlayerType.Passing, "Passing" },
            { PlayerType.Receiving, "Receiving" },
            { PlayerType.Returns, "Returning" },
            { PlayerType.Rushing, "Rushing" },
        };

        internal static T ConvertFromJson<T>(string json, System.Func<string, string> mapper) where T : Player => JsonSerializer.Deserialize<T>(mapper(json));

        internal static Player ConvertFromJson(this PlayerType type, string json, System.Func<string, string> mapper)
        {
            switch (type)
            {
                case PlayerType.Defense:
                    return ConvertFromJson<DefensePlayer>(json, mapper);
                case PlayerType.Rushing:
                    return ConvertFromJson<RushingPlayer>(json, mapper);
                case PlayerType.Receiving:
                    return ConvertFromJson<ReceivingPlayer>(json, mapper);
                case PlayerType.Kicking:
                    return ConvertFromJson<KickingPlayer>(json, mapper);
                case PlayerType.Returns:
                    return ConvertFromJson<ReturningPlayer>(json, mapper);
                default:
                case PlayerType.Passing:
                    return ConvertFromJson<PassingPlayer>(json, mapper);
            }
        }

        internal static string RemapKeys(string dirtyJson)
        {
            using var inputDoc = System.Text.Json.JsonDocument.Parse(dirtyJson);
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
            TryAddKeyValuePair("ApproximateValue", "av");

            return System.Text.Json.JsonSerializer.Serialize(dict);
        }
    }
}
