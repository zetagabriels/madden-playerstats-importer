using System;
using System.Linq;
using System.Collections.Generic;
using MaddenImporter.Models.Player;
using OpenQA.Selenium;
using AngleSharp;
using System.Threading.Tasks;

namespace MaddenImporter.Core
{
    public class SeasonalRetriever : System.IDisposable
    {
        private AngleSharp.IBrowsingContext browser;

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
            Console.WriteLine($"Now retrieving {playerType} players.");

            var playerRows = document.QuerySelectorAll($"table.stats_table > tbody > tr:not(.thead)")
            .Select(el => el.Children);

            int playerRowCount = playerRows.Count();

            List<string> jsons = new List<string>();
            foreach (var row in playerRows)
            {
                var json = "{";
                foreach (var td in row)
                {
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
                jsons.Add(json);
            }
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
                System.Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.\n");
                players = players.Concat(retrieved.Select(p => enumType.ConvertFromJson(p, Extensions.RemapKeys)));
            }

            return players;
        }

        public void Dispose()
        {
            browser?.Dispose();
        }
    }
}
