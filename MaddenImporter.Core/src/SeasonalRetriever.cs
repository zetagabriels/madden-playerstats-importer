using System.Linq;
using System.Collections.Generic;
using MaddenImporter.Models.Player;
using OpenQA.Selenium;

namespace MaddenImporter.Core
{
    public class SeasonalRetriever : System.IDisposable
    {
        private IWebDriver browser;

        public SeasonalRetriever(IWebDriver br = null)
        {
            browser = br ?? Extensions.GetDefaultBrowser();
        }

        private static string GetSeasonUrl(int year, PlayerType playerType) =>
        $"https://www.pro-football-reference.com/years/{year}/{playerType.ToString().ToLower()}.htm";

        private IEnumerable<string> GetPlayersJson(int year, PlayerType playerType)
        {
            var url = GetSeasonUrl(year, playerType);
            browser.Navigate().GoToUrl(url);
            var playerRows = browser.FindElements(By.CssSelector("tbody > tr:not(.thead)"))
            .Select(el => el.FindElements(By.TagName("td")));

            List<string> jsons = new List<string>();
            foreach (var row in playerRows)
            {
                var json = "{";
                foreach (var td in row)
                {
                    var name = td.GetAttribute("data-stat").ToLower();
                    dynamic value;
                    var intOk = int.TryParse(td.Text, out int @int);
                    var floatOk = float.TryParse(td.Text, out float @float);
                    var str = td.Text?.Trim();
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

        public IEnumerable<Player> GetAllPlayers(int year)
        {
            IEnumerable<Player> players = new List<Player>();
            var types = new PlayerType[] { PlayerType.Defense, PlayerType.Passing, PlayerType.Receiving,
            PlayerType.Rushing, PlayerType.Returns, PlayerType.Kicking };
            foreach (var enumType in types)
            {
                var retrieved = GetPlayersJson(year, enumType);
                System.Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.");
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
