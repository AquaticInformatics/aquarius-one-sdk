using CommandLine;
using Common.Library.Protobuf.Models;
using ONE;
using ONE.Common.Library;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("i18nKeys", HelpText = "Retrieves i18nKeys.")]
    public class I18nCommand : ICommand
    {
        
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            //var results = await clientSDK.Library.Geti18nKeysAsync("en-US", "AQI_FOUNDATION_LIBRARY");
            var results = await clientSDK.Library.GetMobilei18nKeysAsync("en-US");
            I18NKeyHelper.I18NKeyList = results;
            var pat = I18NKeyHelper.GetValue("SHORT", "PARAMETERTYPE.ALUMINUM_DISSOLVED");
            Console.WriteLine(pat);

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
