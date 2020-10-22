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
			Console.WriteLine(document == null);
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
				System.Console.WriteLine(json);
				return json;
			})
			.ToList();
			return jsons;
		}

		public static async Task<List<Player>> GetAllPlayers(int year)
		{
			await GetPlayersJson(year, PlayerType.Receiving);

			var p = PlayerExtensions<DefensePlayer>.ConvertFromJson("{\"name\": \"Brian Urlacher\"}");
			return new List<Player> { p };
		}
	}
}