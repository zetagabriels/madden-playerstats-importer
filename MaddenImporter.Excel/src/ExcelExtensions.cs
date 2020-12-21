using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using MaddenImporter.Models.Player;

namespace MaddenImporter.Excel
{
    internal static class ExcelExtensions
    {
        private static Dictionary<Type, string> playerMapper = new Dictionary<Type, string>{
            { typeof(PassingPlayer), "PASSING" },
            { typeof(KickingPlayer), "KICKING" },
            { typeof(DefensePlayer), "DEFENSE" },
            { typeof(ReceivingPlayer), "RECEIVING" },
            { typeof(ReturningPlayer), "RETURN" },
            { typeof(RushingPlayer), "RUSHING" }
        };

        private static void WriteFields(IXLWorksheet worksheet, int row, object[] values)
        {
            for (int i = 0; i < values.Length; i++)
                worksheet.Cell(row, i + 1).Value = values[i];
        }

        private static void WritePlayer<T>(IXLWorksheet worksheet, int row, T player) where T : Player
        {
            object[] values = new object[1];
            switch (player)
            {
                case PassingPlayer p1:
                    values = new object[] { p1.Name, p1.Completions, p1.GamesPlayed, p1.AttemptedPasses, p1.PassingYards, p1.PassingTouchdowns, p1.Interceptions, p1.SacksTaken, p1.Team };
                    break;
                case KickingPlayer p2:
                    values = new object[] { p2.Name, p2.PuntAttempts, p2.GamesPlayed, p2.ExtraPointsMade, p2.ExtraPointsAttempted, p2.FieldGoalsMade, p2.FieldGoalsAttempted, p2.PuntYards, p2.Team };
                    break;
                case DefensePlayer p3:
                    values = new object[] { p3.Name, p3.Interceptions, p3.GamesPlayed, p3.Sacks, p3.SoloTackles, p3.AssistedTackles, p3.TacklesForLoss, p3.InterceptionTouchdowns, p3.PassesDefended, p3.FumblesRecovered, p3.FumbleYards, p3.FumbleTouchdowns, p3.ForcedFumbles, p3.Team };
                    break;
                case ReceivingPlayer p4:
                    values = new object[] { p4.Name, p4.Receptions, p4.GamesPlayed, p4.YardsReceived, p4.ReceivingTouchdowns, p4.Team };
                    break;
                case ReturningPlayer p5:
                    values = new object[] { p5.Name, p5.KickReturns, p5.GamesPlayed, p5.KickReturnYards, p5.KickReturnTouchdowns, p5.PuntReturnAttempts, p5.PuntReturnYards, p5.PuntReturnTouchdowns, p5.Team };
                    break;
                case RushingPlayer p6:
                    values = new object[] { p6.Name, p6.RushAttempts, p6.GamesPlayed, p6.RushingYards, p6.RushTouchdowns, p6.Fumbles, p6.Team };
                    break;
            }
            WriteFields(worksheet, row, values);
        }

        private static void WriteHeaders<T>(IXLWorksheet worksheet) where T : Player
        {
            worksheet.Cell("A2").Value = "Player";
            worksheet.Cell("C1").Value = "Game";
            worksheet.Cell("C2").Value = "G";
            if (typeof(T) == typeof(PassingPlayer))
            {
                worksheet.Cell("B2").Value = "Cmp";
                worksheet.Cell("D2").Value = "Att";
                worksheet.Cell("E2").Value = "Yds";
                worksheet.Cell("F2").Value = "TD";
                worksheet.Cell("G2").Value = "Int";
                worksheet.Cell("H2").Value = "Sk";
                worksheet.Cell("I2").Value = "Team";

            }
            if (typeof(T) == typeof(KickingPlayer))
            {
                worksheet.Cells("B1, H1").Value = "Punt";
                worksheet.Cells("D1:G1").Value = "Scor";
                worksheet.Cell("B2").Value = "Pnt";
                worksheet.Cell("D2").Value = "XPM";
                worksheet.Cell("E2").Value = "XPA";
                worksheet.Cell("F2").Value = "FGM";
                worksheet.Cell("G2").Value = "FGA";
                worksheet.Cell("H2").Value = "Yds";
                worksheet.Cell("I2").Value = "Team";
            }
            if (typeof(T) == typeof(DefensePlayer))
            {
                worksheet.Cell("B1").Value = "Def";
                worksheet.Cell("B2").Value = "Int";
                worksheet.Cell("D2").Value = "Sk";

                worksheet.Cells("E1:G1").Value = "Tack";
                worksheet.Cell("E2").Value = "Solo";
                worksheet.Cell("F2").Value = "Ast";
                worksheet.Cell("G2").Value = "TFL";

                worksheet.Cells("H1:I1").Value = "Def";
                worksheet.Cell("H2").Value = "TD";
                worksheet.Cell("I2").Value = "PD";

                worksheet.Cells("J1:M1").Value = "Fumb";
                worksheet.Cell("J2").Value = "FR";
                worksheet.Cell("K2").Value = "Yds";
                worksheet.Cell("L2").Value = "TD";
                worksheet.Cell("M2").Value = "FF";
                worksheet.Cell("N2").Value = "Team";
            }
            if (typeof(T) == typeof(ReceivingPlayer))
            {
                worksheet.Cell("B2").Value = "Rec";
                worksheet.Cell("D2").Value = "Yds";
                worksheet.Cell("E2").Value = "TD";
                worksheet.Cell("F2").Value = "Team";
            }
            if (typeof(T) == typeof(ReturningPlayer))
            {
                worksheet.Cells("B1, D1:E1").Value = "Kick";
                worksheet.Cells("F1:H1").Value = "Punt";

                worksheet.Cell("B2").Value = "Rt";
                worksheet.Cell("D2").Value = "Yds";
                worksheet.Cell("E2").Value = "TD";
                worksheet.Cell("F2").Value = "Ret";
                worksheet.Cell("G2").Value = "Yds";
                worksheet.Cell("H2").Value = "TD";
                worksheet.Cell("I2").Value = "Team";
            }
            if (typeof(T) == typeof(RushingPlayer))
            {
                worksheet.Cells("B1, D1:E1").Value = "Rush";
                worksheet.Cell("B2").Value = "Att";
                worksheet.Cell("D1").Value = "Rush";
                worksheet.Cell("D2").Value = "Yds";
                worksheet.Cell("E1").Value = "Rush";
                worksheet.Cell("E2").Value = "TD";
                worksheet.Cell("F2").Value = "Fmb";
                worksheet.Cell("G2").Value = "Team";
            }
        }

        public static void WritePlayerSheet<T>(IXLWorkbook workbook, List<T> players) where T : Player
        {
            playerMapper.TryGetValue(typeof(T), out string sheetName);
            var worksheet = workbook.AddWorksheet(sheetName);
            WriteHeaders<T>(worksheet);
            for (int i = 0; i < players.Count; i++)
                WritePlayer(worksheet, i + 4, players[i]);
            Console.WriteLine($"Wrote Excel sheet {typeof(T)}.");
        }
    }
}
