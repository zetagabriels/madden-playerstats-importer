using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MaddenImporter.Models.Player;
using AngleSharp;

namespace MaddenImporter.Core
{
    public class SeasonalRetriever
    {
        private IBrowsingContext browser;

        public SeasonalRetriever(IBrowsingContext br = null)
        {
            browser = br ?? Extensions.GetDefaultBrowser();
        }

        private static string GetSeasonUrl(int year, PlayerType playerType) =>
        $"https://www.pro-football-reference.com/years/{year}/{playerType.ToString().ToLower()}.htm";

        private async Task<IEnumerable<string>> GetPlayersJson(int year, PlayerType playerType)
        {
            var url = GetSeasonUrl(year, playerType);
            var document = await browser.OpenAsync(url);
            var jsons = document.QuerySelectorAll("tbody > tr:not(.thead)")
            .Select(el => el.Children)
            .Select(children =>
            {
                var json = "{";
                foreach (var td in children)
                {
                    // this part is nastier than my ex wife
                    // pls ignore i swear to god it works
                    var name = td.GetAttribute("data-stat").ToLower();
                    dynamic value;
                    var intOk = int.TryParse(td.TextContent, out int @int);
                    var floatOk = float.TryParse(td.TextContent, out float @float);
                    var str = td.TextContent?.Trim();
                    if (intOk)
                        value = @int;
                    else if (floatOk)
                        value = @float;
                    else if (!string.IsNullOrEmpty(str))
                        value = $"\"{str}\"";
                    else
                    {
                        if (name == "pos") value = "\"N/A\"";
                        else value = 0;
                    }

                    json += $"\"{name}\": {value.ToString()},";
                }
                json = json.Substring(0, json.Length - 1); // remove trailing comma
                json += "}";
                return json;
            });
            return jsons;
        }

        public async Task<IEnumerable<Player>> GetAllPlayers(int year)
        {
            IEnumerable<Player> players = new List<Player>();
            var types = new PlayerType[] { PlayerType.Defense, PlayerType.Passing, PlayerType.Receiving,
            PlayerType.Rushing, PlayerType.Returns, PlayerType.Kicking };
            foreach (var enumType in types)
            {
                var retrieved = await GetPlayersJson(year, enumType);
                System.Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.");
                players = players.Concat(retrieved.Select(p => enumType.ConvertFromJson(p, RemapSeasonalKeys)));
            }

            return players;
        }

        // This is ass but that's how it is on this bitch of an earth
        private static string RemapSeasonalKeys(string dirtyJson)
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
            return System.Text.Json.JsonSerializer.Serialize(dict);
        }
    }
}
