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
            RootCommand rootCommand = new RootCommand("Retrieves NFL Season/Career Player Stats and formats it for use with the Madden 2020 Franchise Editor.")
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
                ),
                new Option<int>(
                    "--startYear",
                    getDefaultValue: () => 0, //For example, if endYear is the default 2020, startYear would be 1995
                    description: "For Career imports, set the year to start at."
                ),
                new Option<int>(
                    "--endYear",
                    getDefaultValue: () => DateTime.Now.Year,
                    description: "For Career imports, set the year to end at."
                )
            };

            rootCommand.Handler = System.CommandLine.Invocation.CommandHandler.Create<int, bool, string, string, string, int, int>(ChooseImport);
            return rootCommand.InvokeAsync(args).Result;
        }

        static async Task ChooseImport(int year, bool career, string path, string username, string password, int startYear, int endYear)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("...    Madden Importer v1.0    ...");
            Console.ForegroundColor = ConsoleColor.White;
            path = System.IO.Path.GetFullPath(path);
            if (!career)
            {
                Console.WriteLine($"Beginning seasonal import for year {year}...");
                Console.WriteLine($"\tSaving to path {path}");
                await SeasonalImport(path, year);
            }

            if (career)
            {
                Console.WriteLine("Beginning career import...");
                Console.WriteLine($"\tSaving to path {path}");
                CareerImport(path, username, password, startYear, endYear);
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

        static void CareerImport(string path, string username, string password, int startYear, int endYear)
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
                    ThrowFatalError("Login file exists, but username and password were not found.");
                }
                username ??= data[0].Trim();
                password ??= data[1].Trim();
            }

            if (startYear > 0 && (startYear < 1950 || startYear > DateTime.Now.Year))
                ThrowFatalError("Start year is too early.");

            if (endYear < 1950 || endYear > DateTime.Now.Year)
                ThrowFatalError("End year is invalid.");

            if (startYear == 0)
                startYear = endYear - 25;

            Console.WriteLine($"Using start year {startYear} and end year {endYear}.");

            try
            {
                using var careerRetriever = new CareerRetriever("./");
                var players = careerRetriever.GetAllPlayers(username, password, startYear, endYear).ToList();

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
                ThrowFatalError("You must provide a username and password, either through the command-line options, or in a login.private file in the same directory as the executable.", "Use the --help parameter if needed.");
            }
            catch (ApplicationException)
            {
                ThrowFatalError("Login failed. Check your username and password.", "Make sure you can log in with your own browser first.");
            }
        }

        private static void ThrowFatalError(params string[] errorMessages)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var m in errorMessages)
                Console.WriteLine(m);
            System.Environment.Exit(1);
        }
    }
}
