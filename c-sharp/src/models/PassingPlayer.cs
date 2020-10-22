namespace MaddenImporter
{
	public class PassingPlayer : Player
	{
		public int Completions { get; set; }
		public int AttemptedPasses { get; set; }
		public int PassingYards { get; set; }
		public int PassingTouchdowns { get; set; }
		public int Interceptions { get; set; }
		public int FirstDowns { get; set; }
		public int LongestPass { get; set; }
		public int SacksTaken { get; set; }
		public int FourthQuarterComebacks { get; set; }
	}
}