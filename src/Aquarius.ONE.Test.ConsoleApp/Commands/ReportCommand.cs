using CommandLine;
using ONE;
using ONE.Models.CSharp;
using ONE.Operations.Spreadsheet;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("report", HelpText = "Retrieves Rows.")]
    public class ReportCommand: ICommand
    {
        [Option('g', "guid", Required = true, HelpText = "Guid of the operation")]
        public string Guid { get; set; }
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            Console.WriteLine($"OperationId: {Guid}");
            
            var reports = await clientSDK.Report.GetDefinitionsAsync(Guid);
            if (reports == null)
                return 0;
            foreach (var report in reports)
            {

                Console.WriteLine($"{report.Id}: {report.Name}: {report.ReportDefinitionJson}");
            }
            return 1;

        }
    }
}
