using System.Text.Json;

namespace MaddenImporter
{
    public static class PlayerExtensions<T> where T : Player
    {
        private static string RemapKeys(string dirtyJson)
        {
            return string.Empty;
        }

        public static T ConvertFromJson(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
