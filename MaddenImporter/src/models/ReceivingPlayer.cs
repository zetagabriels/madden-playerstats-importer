namespace MaddenImporter.Models.Player
{
    public class ReceivingPlayer : Player
    {
        public int Receptions { get; set; }
        public int YardsReceived { get; set; }
        public int ReceivingTouchdowns { get; set; }
        public int FirstDowns { get; set; }
        public int LongestReception { get; set; }
        public int Fumbles { get; set; }
    }
}
