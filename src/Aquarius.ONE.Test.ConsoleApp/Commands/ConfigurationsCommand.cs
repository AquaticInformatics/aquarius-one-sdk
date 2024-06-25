using CommandLine;
using ONE;
using ONE.Models.CSharp.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("config", HelpText = "Loads Configurations.")]
    public class ConfigurationsCommand : ICommand
    {
        [Option('t', "twinRefId", Required = true, HelpText = "TwinRefId")]
        public string TwinRefId { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var v1Config = await clientSDK.Configuration.GetConfigurationsAsync(5, TwinRefId);
#pragma warning restore CS0618 // Type or member is obsolete
            var v2Config = await clientSDK.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.WorksheetView.Id, TwinRefId);

            if (v1Config.Any() && v2Config.Any())
            {
                bool match = v1Config[0].Id == v2Config[0].Id;

                Console.WriteLine("V1 Configs...");
                Console.WriteLine();

                foreach (var config in v1Config)
                {
                    string configString = config.ToString();
                    Console.WriteLine(configString);
                    Console.WriteLine();
                }

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
