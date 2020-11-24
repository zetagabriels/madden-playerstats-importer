using System;
using System.Linq;
using System.Threading.Tasks;
using MaddenImporter.Core;
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

            /* var seasonalRetriever = new MaddenImporter.Core.SeasonalRetriever();
            var workbook = new ClosedXML.Excel.XLWorkbook();
            var players = (await seasonalRetriever.GetAllPlayers(year)).ToList();
            ExcelExtensions.WritePlayerSheet<PassingPlayer>(workbook, Extensions.GetPlayersOfType<PassingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<RushingPlayer>(workbook, Extensions.GetPlayersOfType<RushingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<DefensePlayer>(workbook, Extensions.GetPlayersOfType<DefensePlayer>(players));
            ExcelExtensions.WritePlayerSheet<ReturningPlayer>(workbook, Extensions.GetPlayersOfType<ReturningPlayer>(players));
            ExcelExtensions.WritePlayerSheet<KickingPlayer>(workbook, Extensions.GetPlayersOfType<KickingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<ReceivingPlayer>(workbook, Extensions.GetPlayersOfType<ReceivingPlayer>(players));

            workbook.SaveAs(path + "/players.xlsx");
            workbook.Dispose(); */

            var careerRetriever = new CareerRetriever();
            var players = (await careerRetriever.GetAllPlayers()).ToList();

            var workbook = new ClosedXML.Excel.XLWorkbook();
            ExcelExtensions.WritePlayerSheet<PassingPlayer>(workbook, Extensions.GetPlayersOfType<PassingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<RushingPlayer>(workbook, Extensions.GetPlayersOfType<RushingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<DefensePlayer>(workbook, Extensions.GetPlayersOfType<DefensePlayer>(players));
            ExcelExtensions.WritePlayerSheet<ReturningPlayer>(workbook, Extensions.GetPlayersOfType<ReturningPlayer>(players));
            ExcelExtensions.WritePlayerSheet<KickingPlayer>(workbook, Extensions.GetPlayersOfType<KickingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<ReceivingPlayer>(workbook, Extensions.GetPlayersOfType<ReceivingPlayer>(players));

            workbook.SaveAs(path + "/players-career.xlsx");
            workbook.Dispose();
        }
    }
}
