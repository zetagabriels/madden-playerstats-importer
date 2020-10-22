using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AngleSharp;

namespace MaddenImporter
{
	public static class Retriever
	{
		private static async Task<List<string>> GetPlayersJson(int year, PlayerType playerType)
		{
			var config = AngleSharp.Configuration.Default.WithDefaultLoader();
			var url = Extensions.GenerateUrl(year, playerType);
			var browser = AngleSharp.BrowsingContext.New(config);
			var document = await browser.OpenAsync(url);
			var jsons = document.QuerySelectorAll("tbody > tr:not(.thead)")
			.Select(el => el.Children)
			.Select(children =>
			{
				var json = "{";
				foreach (var td in children)
				{
					var name = td.GetAttribute("data-stat").ToLower();
					string strValue = string.Empty;
					int? intValue = null;
					try
					{
						intValue = int.Parse(td.TextContent);
					}
					catch
					{
						strValue = $"\"{td.TextContent}\"";
					}

					json += $"\"{name}\": {intValue?.ToString() ?? strValue},";
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
			var players = await GetPlayersJson(year, PlayerType.Receiving);

			return players.Select(p => Extensions.ConvertFromJson<ReceivingPlayer>(p));
		}
	}
}
