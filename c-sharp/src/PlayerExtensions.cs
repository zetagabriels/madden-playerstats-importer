using System.Text.Json;

namespace MaddenImporter
{
	public static class PlayerExtensions<T> where T : Player
	{
		public static T ConvertFromJson(string json)
		{
			return JsonSerializer.Deserialize<T>(json);
		}
	}
}