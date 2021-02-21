using System;
using System.Collections.Generic;
using System.Linq;
using MaddenImporter.Models.Player;
using OpenQA.Selenium;
using AngleSharp;

namespace MaddenImporter.Core
{
    public class CareerRetriever : IDisposable
    {
        private IWebDriver driver;
        private const string loginUrl = "https://stathead.com/users/login.cgi";

        private static readonly Dictionary<PlayerType, string> urlSuffix = new Dictionary<PlayerType, string>
        {
            { PlayerType.Defense, "&order_by=def_int&positions[]=dt&positions[]=de&positions[]=dl&positions[]=ilb&positions[]=olb&positions[]=lb&positions[]=cb&positions[]=s&positions[]=db&cstat[1]=sacks&ccomp[1]=gt&cval[1]=0&cstat[2]=fumbles_rec&ccomp[2]=gt&cval[2]=0&cstat[3]=tackles_solo&ccomp[3]=gt&cval[3]=0" },
            { PlayerType.Kicking, "&order_by=punt&positions[]=k&positions[]=p&cstat[1]=xpm&ccomp[1]=gt&cval[1]=0" },
            { PlayerType.Passing, "&order_by=pass_cmp&positions[]=qb" },
            { PlayerType.Receiving, "&order_by=rec&positions[]=rb&positions[]=wr&positions[]=te" },
            { PlayerType.Returns, "&order_by=kick_ret&positions[]=rb&positions[]=wr&positions[]=cb&positions[]=s&positions[]=db&cstat[1]=punt_ret&ccomp[1]=gt&cval[1]=0" },
            { PlayerType.Rushing, "&order_by=rush_att&positions[]=qb&positions[]=rb&positions[]=wr&positions[]=te&cstat[1]=fumbles&ccomp[1]=gt&cval[1]=0" }
        };

        public CareerRetriever(string geckoPath, IBrowsingContext br = null)
        {
            var firefoxOptions = new OpenQA.Selenium.Firefox.FirefoxOptions();
            firefoxOptions.AddArgument("-headless");
            driver = new OpenQA.Selenium.Firefox.FirefoxDriver(geckoPath, firefoxOptions);
        }

        private static string GetCareerUrl(PlayerType playerType, int offset, int startYear, int endYear)
        {
            urlSuffix.TryGetValue(playerType, out string suffix);
            suffix ??= "";
            return $"https://stathead.com/football/psl_finder.cgi?request=1&draft_slot_min=1&undrafted=E&draft_year_max={DateTime.Now.Year}&draft_pick_in_round=pick_overall&season_start=1&order_by_asc=0&conference=any&year_min={startYear}&draft_slot_max=500&match=combined&draft_positions[]=qb&draft_positions[]=rb&draft_positions[]=wr&draft_positions[]=te&draft_positions[]=e&draft_positions[]=t&draft_positions[]=g&draft_positions[]=c&draft_positions[]=ol&draft_positions[]=dt&draft_positions[]=de&draft_positions[]=dl&draft_positions[]=ilb&draft_positions[]=olb&draft_positions[]=lb&draft_positions[]=cb&draft_positions[]=s&draft_positions[]=db&draft_positions[]=k&draft_positions[]=p&year_max={endYear}&season_end=-1&draft_year_min=1936&draft_type=B&age_min=0&age_max=99&offset={offset}{suffix}";
        }

        private IEnumerable<string> GetPageAsJson(IEnumerable<IEnumerable<AngleSharp.Dom.IElement>> playerRows, string pos)
        {
            List<string> jsons = new List<string>();
            int playerRowCount = playerRows.Count();
            foreach (var row in playerRows)
            {
                int count = row.Count() - 1;
                var json = "{";
                json += $"\"pos\": \"{pos}\",";
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

        private IEnumerable<string> GetPlayersJson(PlayerType playerType, int startYear, int endYear)
        {
            // pull data
            int offset = 0;
            Extensions.PlayerPositions.TryGetValue(playerType, out string pos);
            var url = GetCareerUrl(playerType, offset, startYear, endYear);
            driver.Navigate().GoToUrl(url);
            Console.WriteLine($"Now retrieving {playerType} players.");
            Console.WriteLine("Please wait... (This may take a while)");
            int playerRowCount = 0;
            List<string> jsons = new List<string>();
            bool paginate = true;
            var parser = new AngleSharp.Html.Parser.HtmlParser();

            do
            {
                var html = driver.PageSource;
                var document = parser.ParseDocument(html);
                var playerRows = document.QuerySelectorAll("tbody > tr:not(.thead)")
                .Select(el => el.Children.ToList());

                var page = GetPageAsJson(playerRows, pos);
                jsons = jsons.Concat(page).ToList();

                playerRowCount = playerRows.Count();
                paginate = playerRowCount == 100;
                if (paginate)
                {
                    offset += 100;
                    url = GetCareerUrl(playerType, offset, startYear, endYear);
                    driver.Navigate().GoToUrl(url);
                }
            } while (paginate);

            return jsons;
        }

        public IEnumerable<Player> GetAllPlayers(string username, string password, int startYear, int endYear)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentNullException();

            IEnumerable<Player> players = new List<Player>();
            var types = new PlayerType[] { PlayerType.Defense, PlayerType.Passing, PlayerType.Receiving,
            PlayerType.Rushing, PlayerType.Returns, PlayerType.Kicking };

            // login
            driver.Navigate().GoToUrl(loginUrl);
            driver.FindElement(By.Id("username")).SendKeys(username);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.CssSelector("input[type=submit]")).Click();

            try
            {
                var error = driver.FindElement(By.CssSelector("div.error"));
                if (error != null && error.Displayed)
                    throw new ApplicationException();
            }
            // login OK
            catch (NoSuchElementException) { }

            foreach (var enumType in types)
            {
                var retrieved = GetPlayersJson(enumType, startYear, endYear);
                var r = retrieved.Select(p => enumType.ConvertFromJson(p, Extensions.RemapKeys));
                Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.");
                players = players.Concat(r);
            }

            return players;
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
