using CommandLine;
using ONE;
using ONE.Enterprise.Authentication;
using ONE.Utilities.UI;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("ui", HelpText = "Loads UI Defnition.")]
    public class UIDefinitionCommand: ICommand
    {
        [Option('f', "filename", Required = true, HelpText = "Filename")]
        public string Filename { get; set; }
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            if (!File.Exists(Filename))
                return 0;
            else
            {
                string json = File.ReadAllText(Filename);
                UIDefinition uIDefinition = new UIDefinition(json);
                Console.WriteLine(uIDefinition.ToString());
                return 1;
            }
        }
    }
}
