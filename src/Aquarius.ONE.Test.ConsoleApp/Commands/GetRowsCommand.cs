using CommandLine;
using ONE;
using ONE.Models.CSharp;
using ONE.Operations.Spreadsheet;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("rows", HelpText = "Retrieves Rows.")]
    public class GetRowsCommand: ICommand
    {
        [Option('g', "guid", Required = true, HelpText = "Guid of the operation")]
        public string Guid { get; set; }
        [Option('t', "type", Required = true, HelpText = "Worksheet Type (1 Fifteen Min, 2 Hourly, 3 Four hour, 4 Daily")]
        public uint Type { get; set; }
        [Option('r', "row", Required = true, HelpText = "Row number")]
        public uint RowNumber { get; set; }
        [Option('c', "column", Required = true, HelpText = "Column number")]
        public uint ColumnNumber { get; set; }
        [Option('a', "action", Required = true, HelpText = "Action")]
        public string Action { get; set; }
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            Console.WriteLine($"OperationId: {Guid}");
            Console.WriteLine($"Type: {Type}");
            Console.WriteLine($"RowNumber: {RowNumber}");
            EnumWorksheet worksheetType = EnumWorksheet.WorksheetUnknown;
            switch (Type)
            {
                case 1:
                    worksheetType = EnumWorksheet.WorksheetFifteenMinute;
                    break;
                case 2:
                    worksheetType = EnumWorksheet.WorksheetHour;
                    break;
                case 3:
                    worksheetType = EnumWorksheet.WorksheetFourHour;
                    break;
                case 4:
                    worksheetType = EnumWorksheet.WorksheetDaily;
                    break;

            }
            var rows = await clientSDK.Spreadsheet.GetRowsAsync(Guid, worksheetType, RowNumber, RowNumber);

            switch (Action.ToUpper())
            {
                default:
                    return 1;
                case "CELL":
                    if (rows.Items != null && rows.Items.Count > 0)
                    {
                        var cell = Helper.GetCellWithLatestCellDataAndNotes(rows.Items[RowNumber], ColumnNumber);

                    }
                    else
                    {
                        return 0;
                    }
                    return 1;
            }
        }
    }
}
