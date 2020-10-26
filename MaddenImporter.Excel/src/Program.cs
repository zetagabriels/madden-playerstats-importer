using System;
using System.Linq;
using System.Threading.Tasks;
using MaddenImporter.Models.Player;

namespace MaddenImporter.Excel
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            int year = DateTime.Now.Year;
            string path = System.IO.Path.GetFullPath("./temp");
            if (args.Length > 0)
            {
                int.TryParse(args[0], out year);
            }

            var workbook = new ClosedXML.Excel.XLWorkbook();
            var players = (await MaddenImporter.Retriever.GetAllPlayers(year)).ToList();
            ExcelExtensions.WritePlayerSheet<PassingPlayer>(workbook, Extensions.GetPlayersOfType<PassingPlayer>(players));

            workbook.SaveAs(path + "/passing.players.xlsx");
            workbook.Dispose();
        }
    }
}
