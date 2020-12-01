using System;
using System.Linq;
using MaddenImporter.Core;
using MaddenImporter.Models.Player;

namespace MaddenImporter.Excel
{
    public class Program
    {
        static void Main(string[] args)
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

            using var careerRetriever = new CareerRetriever();
            string username = string.Empty;
            string password = string.Empty;
            if (System.IO.File.Exists("login.private"))
            {
                var data = System.IO.File.ReadAllLines("login.private");
                if (data.Length < 2)
                {
                    System.Console.WriteLine("Login file exists, but username and password were not found.");
                    throw new ArgumentException();
                }
                username = data[0].Trim();
                password = data[1].Trim();
            }
            var players = careerRetriever.GetAllPlayers(username, password).ToList();

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
