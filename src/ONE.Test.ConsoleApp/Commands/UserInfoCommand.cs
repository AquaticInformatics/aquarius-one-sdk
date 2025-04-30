using System;
using System.Threading.Tasks;
using CommandLine;

namespace ONE.Test.ConsoleApp.Commands
{
    [Verb("userinfo", HelpText = "Retrieve User Information.")]
    public class UserInfoCommand: ICommand
    {
       
        async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
        {
            var result = await clientSdk.Authentication.GetUserInfoAsync();
            if (result == null)
                return 0;
            else
            {
                clientSdk.Authentication.User = await clientSdk.UserHelper.GetUserFromUserInfoAsync(result);
                Console.WriteLine($"Accepted TCS: {clientSdk.Authentication.User.IsAcceptedTcs}");
                return 1;
            }
        }
    }
}
