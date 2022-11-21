using CommandLine;
using ONE;
using System;
using System.Linq;
using System.Threading.Tasks;
using ONE.Models.CSharp.Common.Configuration;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("config", HelpText = "Loads Configurations.")]
    public class ConfigurationsCommand : ICommand
    {
        [Option('t', "twinRefId", Required = true, HelpText = "TwinRefId")]
        public string TwinRefId { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            var v1Config = await clientSDK.Configuration.GetConfigurationsAsync(5, TwinRefId);
            var v2Config = await clientSDK.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.Worksheets.Id, TwinRefId);

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
