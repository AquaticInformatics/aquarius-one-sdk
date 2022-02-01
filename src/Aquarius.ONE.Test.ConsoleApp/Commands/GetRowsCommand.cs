using CommandLine;
using ONE;
using ONE.Models.CSharp;
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
            
            return 1;
        }
    }
}
