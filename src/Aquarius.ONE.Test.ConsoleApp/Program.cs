using Aquarius.ONE.Test.ConsoleApp.Commands;
using CommandLine;
using ONE;
using ONE.Utilities;
using System;
using System.Threading.Tasks;

namespace Aquarius.ONE.Test.ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            ClientSDK clientSDK = new ClientSDK();
            clientSDK.Logger.Event += new EventHandler<ClientApiLoggerEventArgs>(SdkEvent);
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

                clientSDK = CommandHelper.LoadConfig(clientSDK);
                clientSDK.LogRestfulCalls = true;

                if (!clientSDK.Authentication.IsAuthenticated)
                {
                    Console.WriteLine("User not Authenticated");
                    var result = CommandHelper.SetEnvironment(clientSDK);
                    if (result == null)
                        return 1;
                    while (result == false)
                    {
                        result = CommandHelper.SetEnvironment(clientSDK);
                        if (result == null)
                            return 1;
                    }
                    result = await LoginAsync(clientSDK);
                    if (result == true)
                        return 0;
                    return 1;
                }

                var retValue = await Parser.Default.ParseArguments<
                    DataCommand,
                    GetRowsCommand,
                    I18nCommand,
                    LoginCommand,
                    NotificationTopicCommand,
                    OperationsCommand,
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
        static void SdkEvent(object sender, ClientApiLoggerEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now} {e.EventLevel} {e.HttpStatusCode} {e.ElapsedMs}ms {e.Module}:{e.Message} ");
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
                var result = await clientSDK.Authentication.GetUserInfoAsync();
                return true;
            }
            return false;
        }
        
       
    }
}
