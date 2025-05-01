using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using ONE.Models.CSharp.Constants;

namespace ONE.Test.ConsoleApp.Commands
{
    [Verb("config", HelpText = "Loads Configurations.")]
    public class ConfigurationsCommand : ICommand
    {
        [Option('t', "twinRefId", Required = true, HelpText = "TwinRefId")]
        public string TwinRefId { get; set; }

        async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
        {
            var v2Config = await clientSdk.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.WorksheetView.Id, TwinRefId);

            if (v2Config.Any())
            {
                Console.WriteLine("V2 Configs...");
                Console.WriteLine();

                foreach (var config in v2Config)
                {
                    string configString = config.ToString();
                    Console.WriteLine(configString);
                    Console.WriteLine();
                }
            }

            return 1;
        }
    }
}
