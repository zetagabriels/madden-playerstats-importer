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
        static int Main(params string[] args)
        {
            RootCommand rootCommand = new RootCommand("Retrieves all player info and formats it for Madden 2020.")
            {
                new Option<int>(
                    new string[]{"--year", "-y"},
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
                    getDefaultValue: () => "./output",
                    description: "The path to save to."
                ),
                new Option<string>(
                    new string[]{"--username", "-u"},
                    description: "For career imports, your stathead.com username."
                ),
                new Option<string>(
                    new string[]{"--password", "-p"},
                    description: "For career imports, your stathead.com password."
                )
            };

            rootCommand.Handler = System.CommandLine.Invocation.CommandHandler.Create<int, bool, string, string, string>(ChooseImport);
            return rootCommand.InvokeAsync(args).Result;
        }

        static async Task ChooseImport(int year, bool career, string path, string username, string password)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("...    Madden Importer v0.1    ...");
            Console.ForegroundColor = ConsoleColor.White;
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
                CareerImport(path, username, password);
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

        static void CareerImport(string path, string username, string password)
        {
            if (!System.IO.File.Exists("./geckodriver") && !System.IO.File.Exists("./geckodriver.exe"))
            {
                Console.WriteLine("\nGecko driver not found. Please download it from https://github.com/mozilla/geckodriver/releases and put it in this folder.");
                Console.WriteLine("If you are a GNU/Linux user, you may need to run chmod +x geckodriver");
                return;
            }
            if (System.IO.File.Exists("login.private"))
            {
                var data = System.IO.File.ReadAllLines("login.private");
                if (data.Length < 2)
                {
                    System.Console.WriteLine("Login file exists, but username and password were not found.");
                    throw new ArgumentException();
                }
                username ??= data[0].Trim();
                password ??= data[1].Trim();
            }

            try
            {
                using var careerRetriever = new CareerRetriever("./");
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
            catch (ArgumentNullException)
            {
                Console.WriteLine("You must provide a username and password, either through the command-line options, or in a login.private file in the same directory as the executable.");
            }
            catch (ApplicationException)
            {
                Console.WriteLine("Login failed. Check your username and password.\nMake sure you can log in with your own browser first.");
            }
        }
    }
}
