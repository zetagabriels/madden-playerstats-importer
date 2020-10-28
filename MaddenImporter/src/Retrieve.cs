using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AngleSharp;
using MaddenImporter.Models.Player;

namespace MaddenImporter
{
    public static class Retriever
    {
        private static async Task<List<string>> GetPlayersJson(int year, PlayerType playerType)
        {
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var url = Extensions.GetSeasonUrl(year, playerType);
            var browser = AngleSharp.BrowsingContext.New(config);
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
                    string strValue = string.Empty;
                    int? intValue = null;
                    float? floatValue = null;
                    try
                    {
                        intValue = int.Parse(td.TextContent);
                    }
                    catch
                    {
                        try
                        {
                            floatValue = float.Parse(td.TextContent);
                        }
                        catch
                        {
                            strValue = $"\"{td.TextContent?.Trim()}\"";
                            if (string.IsNullOrEmpty(td.TextContent))
                            {
                                if (name == "pos")
                                    strValue = "\"N/A\"";
                                else
                                    intValue = 0;
                            }
                        }
                    }

                    json += $"\"{name}\": {intValue?.ToString() ?? floatValue?.ToString() ?? strValue},";
                }
                json = json.Substring(0, json.Length - 1);
                json += "}";
                return json;
            })
            .ToList();
            return jsons;
        }

        public static async Task<IEnumerable<Player>> GetAllPlayers(int year)
        {
            IEnumerable<Player> players = new List<Player>();
            var types = new PlayerType[] { PlayerType.Defense, PlayerType.Passing, PlayerType.Receiving, PlayerType.Rushing, PlayerType.Returns, PlayerType.Kicking };
            foreach (var enumType in types)
            {
                var retrieved = await GetPlayersJson(year, enumType);
                System.Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.");
                players = players.Concat(retrieved.Select(p => enumType.ConvertFromJson(p)));
            }

            return players;
        }
    }
}
