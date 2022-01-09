using CommandLine;
using ONE;
using ONE.Enterprise.Authentication;
using System;
using System.Configuration;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("userinfo", HelpText = "Retrieve User Information.")]
    public class UserInfoCommand: ICommand
    {
       
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            var result = await clientSDK.Authentication.GetUserInfoAync();
            if (result == null)
                return 0;
            else
            {
                Console.WriteLine(result);
                return 1;
            }
        }
    }
}
