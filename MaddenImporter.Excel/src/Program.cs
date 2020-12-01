using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using MaddenImporter.Core;
using MaddenImporter.Models.Player;

namespace MaddenImporter.Excel
{
    public class Program
    {
        /// <summary>
        /// Retrieves all player info and formats it for Madden 2020.
        /// </summary>
        /// <param name=""></param>
        static int Main(params string[] args)
        {
            RootCommand rootCommand = new RootCommand("Retrieves all player info and formats it for Madden 2020.")
            {
                new Option<int>(
                    "--year",
                    getDefaultValue: () => DateTime.Now.Year,
                    description: "For seasonal imports, sets the year to pull data from."
                ),
                new Option<bool>(
                    "--career",
                    getDefaultValue: () => false,
                    description: "Whether or not to use a career import."
                ),
                new Option<string>(
                    "--path",
                    getDefaultValue: () => "./temp",
                    description: "The path to save to. Defaults to ./temp/"
                )
            };

            rootCommand.Handler = System.CommandLine.Invocation.CommandHandler.Create<int, bool, string>(ChooseImport);
            return rootCommand.InvokeAsync(args).Result;
        }

        static async Task ChooseImport(int year, bool career, string path)
        {
            path = System.IO.Path.GetFullPath(path);
            if (!career)
            {
                Console.WriteLine($"Beginning seasonal import for year {year}.");
                Console.WriteLine($"\tSaving to path {path}");
                await SeasonalImport(path, year);
            }

            if (career)
            {
                Console.WriteLine("Beginning career import.");
                Console.WriteLine($"\tSaving to path {path}");
                CareerImport(path);
            }
        }

        static async Task SeasonalImport(string path, int year)
        {
            using var seasonalRetriever = new MaddenImporter.Core.SeasonalRetriever();
            var workbook = new ClosedXML.Excel.XLWorkbook();
            var players = (await seasonalRetriever.GetAllPlayers(year)).ToList();
            ExcelExtensions.WritePlayerSheet<PassingPlayer>(workbook, Extensions.GetPlayersOfType<PassingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<RushingPlayer>(workbook, Extensions.GetPlayersOfType<RushingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<DefensePlayer>(workbook, Extensions.GetPlayersOfType<DefensePlayer>(players));
            ExcelExtensions.WritePlayerSheet<ReturningPlayer>(workbook, Extensions.GetPlayersOfType<ReturningPlayer>(players));
            ExcelExtensions.WritePlayerSheet<KickingPlayer>(workbook, Extensions.GetPlayersOfType<KickingPlayer>(players));
            ExcelExtensions.WritePlayerSheet<ReceivingPlayer>(workbook, Extensions.GetPlayersOfType<ReceivingPlayer>(players));

            workbook.SaveAs(path + "/players.xlsx");
            workbook.Dispose();
        }

        static void CareerImport(string path)
        {
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

            path += "/players-career.xlsx";
            workbook.SaveAs(path);
            Console.WriteLine($"Saved workbook to {path}");
            workbook.Dispose();
        }
    }
}
