namespace MaddenImporter
{
	public abstract class Player
	{
		public string Name { get; set; }
		public string Team { get; set; }
		public string Position { get; set; }
		public int GamesPlayed { get; set; }
		public int GamesStarted { get; set; }
	}
}
