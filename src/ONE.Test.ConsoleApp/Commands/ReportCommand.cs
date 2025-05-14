using System;
using System.Threading.Tasks;
using CommandLine;
using ONE.ClientSDK.Enterprise.Report;

namespace ONE.Test.ConsoleApp.Commands
{
	[Verb("report", HelpText = "Retrieves Rows.")]
	public class ReportCommand: ICommand
	{
		[Option('g', "guid", Required = true, HelpText = "Guid of the operation")]
		public string Guid { get; set; }
		async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
		{
			Console.WriteLine($"OperationId: {Guid}");
			
			var reports = await clientSdk.Report.GetDefinitionsAsync(Guid);
			if (reports == null)
				return 0;
			foreach (var report in reports)
			{
				var def = new ReportDataDefinition(report.ReportDefinitionJson);
				Console.WriteLine($"{def.StartTime} {report.Id}: {report.Name}: {report.ReportDefinitionJson}");
			}
			return 1;

		}
	}
}
