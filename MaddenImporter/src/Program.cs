using System;
using System.Linq;

namespace MaddenImporter
{
    public class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            int year = DateTime.Now.Year;
            string path = System.IO.Path.GetFullPath("./temp");
            if (args.Length > 0)
            {
                int.TryParse(args[0], out year);
            }

            Console.WriteLine($"Beginning retrieval for year {year}...");
            await Extensions.CheckOrCreatePath(path);

            var players = await Retriever.GetAllPlayers(year);
            Extensions.WritePlayersList(path, players.ToList());
            //players.ToList().ForEach(Console.WriteLine);
        }
    }
}
