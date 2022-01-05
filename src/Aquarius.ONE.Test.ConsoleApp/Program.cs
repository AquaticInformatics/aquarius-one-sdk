using Aquarius.ONE.Test.ConsoleApp.Commands;
using CommandLine;
using ONE;
using System;
using System.Threading.Tasks;

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
            try
            {
                clientSDK.LogRestfulCalls = true;

                CommandHelper.LoadConfig(clientSDK);

                var retValue = await Parser.Default.ParseArguments<
                    DataCommand,
                    GetRowsCommand,
                    I18nCommand,
                    LoginCommand,
                    ParameterCommand,
                    QuantityTypesCommand,
                    RolesCommand,
                    UnitCommand,
                    UserInfoCommand
                    >(args).WithParsedAsync<ICommand>(t => t.Execute(clientSDK));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

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
