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
            object[] values;
            switch (player)
            {
                case PassingPlayer p1:
                    values = new object[] { p1.Name, p1.Completions, p1.GamesPlayed, p1.AttemptedPasses, p1.PassingYards, p1.PassingTouchdowns, p1.Interceptions, p1.SacksTaken };
                    WriteFields(worksheet, row, values);
                    break;
                case KickingPlayer p2:
                case DefensePlayer p3:
                case ReceivingPlayer p4:
                case ReturningPlayer p5:
                case RushingPlayer p6:
                    break;
            }
        }

        private static void WriteHeaders<T>(IXLWorksheet worksheet) where T : Player
        {
            if (typeof(T) == typeof(PassingPlayer))
            {
                worksheet.Cell("B2").Value = "Cmp";
                worksheet.Cell("C2").Value = "G";
                worksheet.Cell("D2").Value = "Att";
                worksheet.Cell("E2").Value = "Yds";
                worksheet.Cell("F2").Value = "TD";
                worksheet.Cell("G2").Value = "Int";
                worksheet.Cell("H2").Value = "Sk";
            }
            if (typeof(T) == typeof(KickingPlayer))
            {

            }
            if (typeof(T) == typeof(DefensePlayer))
            {

            }
            if (typeof(T) == typeof(ReceivingPlayer))
            {

            }
            if (typeof(T) == typeof(ReturningPlayer))
            {

            }
            if (typeof(T) == typeof(RushingPlayer))
            {

            }
        }

        public static void WritePlayerSheet<T>(IXLWorkbook workbook, List<T> players) where T : Player
        {
            playerMapper.TryGetValue(typeof(T), out string sheetName);
            var worksheet = workbook.AddWorksheet(sheetName);
            worksheet.Cell("A2").Value = "Player";
            WriteHeaders<T>(worksheet);
            for (int i = 0; i < players.Count; i++)
            {
                WritePlayer(worksheet, i + 4, players[i]);
            }
        }
    }
}
