using System;

namespace MaddenImporter
{
	public class Program
	{
		static void Main(string[] args)
		{
			int year = DateTime.Now.Year;
			if (args.Length > 0)
			{
				int.TryParse(args[0], out year);
			}

			var players = Retriever.GetAllPlayers(year).GetAwaiter().GetResult();
		}
	}
}
