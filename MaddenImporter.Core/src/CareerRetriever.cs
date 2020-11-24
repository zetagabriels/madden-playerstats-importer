using AngleSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MaddenImporter.Models.Player;

namespace MaddenImporter.Core
{
    public class CareerRetriever
    {
        private IBrowsingContext browser;

        private static readonly Dictionary<PlayerType, string> urlSuffix = new Dictionary<PlayerType, string>
        {
            { PlayerType.Defense, "&order_by=def_int&positions[]=dt&positions[]=de&positions[]=dl&positions[]=ilb&positions[]=olb&positions[]=lb&positions[]=cb&positions[]=s&positions[]=db&cstat[1]=sacks&ccomp[1]=gt&cval[1]=0&cstat[2]=fumbles_rec&ccomp[2]=gt&cval[2]=0&cstat[3]=tackles_solo&ccomp[3]=gt&cval[3]=0" },
            { PlayerType.Kicking, "&order_by=punt&positions[]=k&positions[]=p&cstat[1]=xpm&ccomp[1]=gt&cval[1]=0" },
            { PlayerType.Passing, "&order_by=pass_cmp&positions[]=qb" },
            { PlayerType.Receiving, "&order_by=rec&positions[]=rb&positions[]=wr&positions[]=te" },
            { PlayerType.Returns, "&order_by=kick_ret&positions[]=rb&positions[]=wr&positions[]=cb&positions[]=s&positions[]=db&cstat[1]=punt_ret&ccomp[1]=gt&cval[1]=0" },
            { PlayerType.Rushing, "&order_by=rush_att&positions[]=qb&positions[]=rb&positions[]=wr&positions[]=te&cstat[1]=fumbles&ccomp[1]=gt&cval[1]=0" }
        };

        public CareerRetriever(IBrowsingContext br = null)
        {
            browser = br ?? Extensions.GetDefaultBrowser();
        }

        private static string GetCareerUrl(PlayerType playerType)
        {
            urlSuffix.TryGetValue(playerType, out string suffix);
            suffix ??= "";
            return $"https://stathead.com/football/psl_finder.cgi?request=1&draft_slot_min=1&undrafted=E&draft_year_max={DateTime.Now.Year}&draft_pick_in_round=pick_overall&season_start=1&order_by_asc=0&conference=any&year_min=1995&draft_slot_max=500&match=combined&draft_positions[]=qb&draft_positions[]=rb&draft_positions[]=wr&draft_positions[]=te&draft_positions[]=e&draft_positions[]=t&draft_positions[]=g&draft_positions[]=c&draft_positions[]=ol&draft_positions[]=dt&draft_positions[]=de&draft_positions[]=dl&draft_positions[]=ilb&draft_positions[]=olb&draft_positions[]=lb&draft_positions[]=cb&draft_positions[]=s&draft_positions[]=db&draft_positions[]=k&draft_positions[]=p&year_max={DateTime.Now.Year}&season_end=-1&draft_year_min=1936&draft_type=B&is_active=Y&age_min=0&age_max=99&offset=0{suffix}";
        }

        private async Task<IEnumerable<string>> GetPlayersJson(PlayerType playerType)
        {
            Extensions.PlayerPositions.TryGetValue(playerType, out string pos);
            var url = GetCareerUrl(playerType);
            var document = await browser.OpenAsync(url);
            var jsons = document.QuerySelectorAll("tbody > tr:not(.thead)")
            .Select(el => el.Children)
            .Select(children =>
            {
                var json = "{";
                // the first column is the "rank" based on the search ordering, which we don't care about
                foreach (var td in children.Skip(1))
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
                json += $"\"pos\": \"{pos}\",";
                json = json.Substring(0, json.Length - 1); // remove trailing comma
                json += "}";
                return json;
            });
            return jsons;
        }

        public async Task<IEnumerable<Player>> GetAllPlayers()
        {
            IEnumerable<Player> players = new List<Player>();
            var types = new PlayerType[] { PlayerType.Defense, PlayerType.Passing, PlayerType.Receiving,
            PlayerType.Rushing, PlayerType.Returns, PlayerType.Kicking };
            foreach (var enumType in types)
            {
                var retrieved = await GetPlayersJson(enumType);
                Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.");
                // for now. once we have an account, remove skip
                players = players.Concat(retrieved.Skip(10).Select(p => enumType.ConvertFromJson(p, Extensions.RemapKeys)));
            }

            return players;
        }
    }
}
