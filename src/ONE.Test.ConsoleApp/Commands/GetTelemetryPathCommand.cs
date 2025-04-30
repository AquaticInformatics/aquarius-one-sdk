using System;
using System.Threading.Tasks;
using CommandLine;
using ONE.ClientSDK.Enterprise.Twin;

namespace ONE.Test.ConsoleApp.Commands
{
    [Verb("telemetrypath", HelpText = "Get Telemetry Path.")]
    public class GetTelemetryPathCommand: ICommand
    {
        [Option('o', "guid", Required = true, HelpText = "OperationId GUID")]
        public string OperationId { get; set; }

        [Option('t', "guid", Required = true, HelpText = "TwinRefId")]
        public string TwinRefId { get; set; }


        async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
        {
            var twinHelper = new Helper(clientSdk.DigitalTwin);

            bool success = await twinHelper.LoadOperationAsync(OperationId);

            string path = twinHelper.GetTelemetryPath(TwinRefId, false);

            Console.WriteLine(path);

            return 1;
        }
    }
}
