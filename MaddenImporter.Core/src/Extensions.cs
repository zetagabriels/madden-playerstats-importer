using System.Threading.Tasks;
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
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            return AngleSharp.BrowsingContext.New(config);
        }

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
    }
}
