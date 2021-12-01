using ONE;
using ONE.Utilities;
using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using CommandLine;
using Aquarius.ONE.Test.ConsoleApp.Commands;

namespace Aquarius.ONE.Test.ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            ClientSDK clientSDK = new ClientSDK();
            
            // If no arguments prompt for login
            if (args.Length == 0)
            {
                // Set Environment

                var result = CommandHelper.SetEnvironment(clientSDK);
                if (result == null)
                    return 1;
                while (result == false)
                {
                    result = CommandHelper.SetEnvironment(clientSDK);
                    if (result == null)
                        return 1;
                }
                // Login
                result = await LoginAsync(clientSDK);
                if (result == true)
                    return 0;
               
                return 1;
            }
            var retValue = await Parser.Default.ParseArguments<LoginCommand, UserInfoCommand>(args).WithParsedAsync<ICommand>(t => t.Execute(clientSDK));

           return 0;



        }
        static async Task<bool?> LoginAsync(ClientSDK clientSDK, string username = "", string password = "")
        {
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Enter Username:");
                username = Console.ReadLine();
            }
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Enter Password:");
                password = Console.ReadLine();
            }
            if (await clientSDK.Authentication.LoginResourceOwnerAsync(username, password))
            {
                Console.WriteLine($"Login Successful!");
                Console.WriteLine($"Access Token: {clientSDK.Authentication.Token.access_token}");
                Console.WriteLine($"Expires: {clientSDK.Authentication.Token.expires.ToString("MM/dd/yyyy HH:mm:ss")}");
                Console.WriteLine($"Login Successful!");
                CommandHelper.SaveConfig(clientSDK);
                var result = await clientSDK.Authentication.GetUserInfo();
                return true;
            }
            return false;
        }
        
       
    }
}
