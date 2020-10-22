namespace MaddenImporter
{
    public class RushingPlayer : Player
    {
        public int RushAttempts { get; set; }
        public int Yards { get; set; }
        public int RushTouchdowns { get; set; }
        public int FirstDowns { get; set; }
        public int LongestRush { get; set; }
        public int Fumbles { get; set; }
    }
}
