using CommandLine;
using Common.Library.Protobuf.Models;
using ONE;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("i18nKeys", HelpText = "Retrieves i18nKeys.")]
    public class I18nCommand : ICommand
    {
        
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            var results = await clientSDK.Library.Geti18nKeysAsync("en-US", "AQI_FOUNDATION_LIBRARY");
            if (results == null)
                return 0;
            foreach (var result in results)
            {

                Console.WriteLine($"{result.Key}: {result.Type}: {result.Value}");
            }
            return 1;
        }
    }
}
