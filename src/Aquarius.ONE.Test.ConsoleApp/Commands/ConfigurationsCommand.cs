using CommandLine;
using ONE;
using ONE.Enterprise.Authentication;
using ONE.Utilities.UI;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ONE.Common.Configuration;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("configs", HelpText = "Loads Configurations.")]
    public class ConfigurationsCommand : ICommand
    {
        //[Option('f', "filename", Required = true, HelpText = "Filename")]
        //public string Filename { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            var v1Config = await clientSDK.Configuration.GetConfigurationsAsync(5, "d3c3eeb9-cea7-4a45-8b27-a09ff9fb8035");
            var v2Config = await clientSDK.Configuration.GetConfigurationsAsync(Constants.ConfigurationTypes.Worksheets, "d3c3eeb9-cea7-4a45-8b27-a09ff9fb8035");


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


            //await clientSDK.Configuration.CreateConfigurationAsync();


            //if (!File.Exists(Filename))
            //    return 0;
            //else
            //{
            //    string json = File.ReadAllText(Filename);
            //    UIDefinition uIDefinition = new UIDefinition(json);
            //    Console.WriteLine(uIDefinition.ToString());
            //    return 1;
            //}

            return 1;
        }
    }
}
