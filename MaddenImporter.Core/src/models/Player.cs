namespace MaddenImporter.Models.Player
{
    public abstract class Player
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesStarted { get; set; }
        public int ApproximateValue { get; set; }

        public override string ToString()
        {
            return $"Player {Name}, from team {Team}, position {Position}";
        }
    }
}
