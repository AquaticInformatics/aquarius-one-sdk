using CommandLine;
using ONE;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("parameter", HelpText = "Retrieve all parameters.")]
    public class ParameterCommand: ICommand
    {
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            var result = await clientSDK.Library.GetParametersAsync();
            if (result == null)
                return 0;
            foreach (var parameter in result)
            {
                
                Console.WriteLine($"{parameter.Id}: {parameter.IntId}: {parameter.I18NKey}" );
            }
            return 1;
        }
    }
}
