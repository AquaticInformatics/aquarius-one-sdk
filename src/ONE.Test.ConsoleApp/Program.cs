using System;
using System.Threading.Tasks;
using CommandLine;
using ONE.ClientSDK.Utilities;
using ONE.Test.ConsoleApp.Commands;

namespace ONE.Test.ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            ClientSDK.OneApi clientSdk = new ClientSDK.OneApi();

            clientSdk.Logger.Event += new EventHandler<ClientApiLoggerEventArgs>(SdkEvent);
            // If no arguments prompt for login
            if (args.Length == 0)
            {
                // Set Environment
                clientSdk = CommandHelper.SetThrowAPIErrors(clientSdk);
                var result = CommandHelper.SetEnvironment(clientSdk);
                if (result == null)
                    return 1;
                while (result == false)
                {
                    result = CommandHelper.SetEnvironment(clientSdk);
                    if (result == null)
                        return 1;
                }
                // Login
                result = await LoginAsync(clientSdk);
                if (result == true)
                    return 0;

                return 1;
            }
          
            try
            {

                clientSdk = CommandHelper.LoadConfig();
                clientSdk.LogRestfulCalls = true;

                if (!clientSdk.Authentication.IsAuthenticated)
                {
                    Console.WriteLine("User not Authenticated");
                    var result = CommandHelper.SetEnvironment(clientSdk);
                    if (result == null)
                        return 1;
                    while (result == false)
                    {
                        result = CommandHelper.SetEnvironment(clientSdk);
                        if (result == null)
                            return 1;
                    }
                    result = await LoginAsync(clientSdk);
                    if (result == true)
                        return 0;
                    return 1;
                }

                var retValue = await Parser.Default.ParseArguments<
                    DataCommand,
                    GetRowsCommand,
                    I18NCommand,
                    LoginCommand,
                    NotificationTopicCommand,
                    OperationsCommand,
                    ParameterCommand,
                    QuantityTypesCommand,
                    RolesCommand,
                    UIDefinitionCommand,
                    UnitCommand,
                    UserInfoCommand,
                    ConfigurationsCommand,
                    GetTelemetryPathCommand,
                    ReportCommand
                    >(args).WithParsedAsync<ICommand>(t => t.Execute(clientSdk));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }

           return 0;



        }
        static void SdkEvent(object sender, ClientApiLoggerEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now} {e.EventLevel} {e.HttpStatusCode} {e.ElapsedMs}ms {e.Module}:{e.Message} ");
        }
        static async Task<bool?> LoginAsync(ClientSDK.OneApi clientSDK, string username = "", string password = "")
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
            if (await clientSDK.Authentication.LoginAsync(username, password))
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
