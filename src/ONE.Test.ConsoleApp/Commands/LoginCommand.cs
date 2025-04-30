using System;
using System.Threading.Tasks;
using CommandLine;

namespace ONE.Test.ConsoleApp.Commands
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

        async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
        {
            CommandHelper.SetEnvironment(clientSdk, Environment);

            Console.WriteLine($"Executing Login: Username {Username}, Password {Password}");
            if (await clientSdk.Authentication.LoginAsync(Username, Password))
            {
                Console.WriteLine("Login Successful!");
                Console.WriteLine($"Access Token: {clientSdk.Authentication.Token.access_token}");
                Console.WriteLine($"Expires: {clientSdk.Authentication.Token.expires.ToString("MM/dd/yyyy HH:mm:ss")}");
                Console.WriteLine($"Login Successful!");
                CommandHelper.SaveConfig(clientSdk);
                return 0;
            }
            return 1;
        }
    }
}
