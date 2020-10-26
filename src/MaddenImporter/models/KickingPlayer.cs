namespace MaddenImporter.Models.Player
{
    public class KickingPlayer : Player
    {
        public int PuntAttempts { get; set; }
        public int PuntYards { get; set; }
        public int ExtraPointsAttempted { get; set; }
        public int ExtraPointsMade { get; set; }
        public int FieldGoalsAttempted { get; set; }
        public int FieldGoalsMade { get; set; }
    }
}
