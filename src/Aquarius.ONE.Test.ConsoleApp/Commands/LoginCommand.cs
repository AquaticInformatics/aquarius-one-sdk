using CommandLine;
using ONE;
using ONE.Utilities;
using System;
using System.Threading.Tasks;

namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("login", HelpText = "Login to Aquarius ONE.")]
    public class LoginCommand : ICommand
    {
        [Option('e', "environment", Required = true, HelpText = "Environment")]
        public string Environment { get; set; }

        [Option('u', "username", Required = true, HelpText = "Username")]
        public string Username { get; set; }
        [Option('p', "password", Required = true, HelpText = "Password")]
        public string Password { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            CommandHelper.SetEnvironment(clientSDK, Environment);

            Console.WriteLine($"Executing Login: Username {Username}, Password {Password}");
            if (await clientSDK.Authentication.LoginResourceOwnerAsync(Username, Password))
            {
                Console.WriteLine($"Login Successful!");
                Console.WriteLine($"Access Token: {clientSDK.Authentication.Token.access_token}");
                Console.WriteLine($"Expires: {clientSDK.Authentication.Token.expires.ToString("MM/dd/yyyy HH:mm:ss")}");
                Console.WriteLine($"Login Successful!");
                CommandHelper.SaveConfig(clientSDK);
                return 0;
            }
            return 1;
        }
    }
}
