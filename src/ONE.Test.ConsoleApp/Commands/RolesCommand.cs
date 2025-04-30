using System;
using System.Threading.Tasks;
using CommandLine;

namespace ONE.Test.ConsoleApp.Commands
{
    [Verb("roles", HelpText = "Retrieve Roles.")]
    public class RolesCommand: ICommand
    {
       
        async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
        {
            var result = await clientSdk.Core.GetRolesAsync(false);
            if (result == null)
                return 0;
            else
            {
                foreach (var role in result)
                {
                    Console.WriteLine($"{role.Name}: {role.Id}");

                }
                return 1;
            }
        }
    }
}
