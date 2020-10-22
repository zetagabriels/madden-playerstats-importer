namespace MaddenImporter
{
    public class DefensePlayer : Player
    {
        public int Interceptions { get; set; }
        public int InterceptionYards { get; set; }
        public int InterceptionTouchdowns { get; set; }
        public int LongestInterceptionReturn { get; set; }
        public int PassesDefended { get; set; }
        public int ForcedFumbles { get; set; }
        public int FumblesRecovered { get; set; }
        public int FumbleYards { get; set; }
        public int FumbleTouchdowns { get; set; }
        public int Sacks { get; set; }
        public int AssistedTackles { get; set; }
        public int TacklesForLoss { get; set; }
        public int Safety { get; set; }
    }
}
