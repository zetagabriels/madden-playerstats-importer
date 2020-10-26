using System;
using System.Linq;

namespace MaddenImporter
{
    public class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            int year = DateTime.Now.Year;
            if (args.Length > 0)
            {
                int.TryParse(args[0], out year);
            }

            Console.WriteLine($"Beginning retrieval for year {year}...");

            var players = await Retriever.GetAllPlayers(year);
            players.ToList().ForEach(Console.WriteLine);
        }
    }
}
