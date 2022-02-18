using ONE;
using ONE.Enterprise.Authentication;
using ONE.Utilities;
using System;
using System.Configuration;
using System.Net.Http.Headers;

namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    public static class CommandHelper
    {
        public static void SaveConfig(ClientSDK clientSDK)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SetConfiguration(configuration, "AccessToken", clientSDK.Authentication.Token.access_token);
            SetConfiguration(configuration, "TokenCreated", clientSDK.Authentication.Token.created.ToString("MM/dd/yyyy HH:mm:ss"));
            SetConfiguration(configuration, "TokenExpires", clientSDK.Authentication.Token.expires.ToString("MM/dd/yyyy HH:mm:ss"));
            SetConfiguration(configuration, "Environment", clientSDK.Environment.PlatformEnvironmentEnum.ToString());
            configuration.Save(ConfigurationSaveMode.Full);
        }
        public static void SetConfiguration(Configuration configuration, string key, string value)
        {
            if (configuration.AppSettings.Settings[key] == null)
                configuration.AppSettings.Settings.Add(key, value);
            else
                configuration.AppSettings.Settings[key].Value = value;
        }
        public static void SetConfiguration(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[key] == null)
                configuration.AppSettings.Settings.Add(key, value);
            else
                configuration.AppSettings.Settings[key].Value = value;
            configuration.Save(ConfigurationSaveMode.Full);
        }
        public static string GetConfiguration(string key)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[key] == null)
                return null;
            return configuration.AppSettings.Settings[key].Value;
        }
        public static ClientSDK LoadConfig(ClientSDK clientSDK)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string accessToken = "";
            if (configuration.AppSettings.Settings["AccessToken"] != null)
                accessToken = configuration.AppSettings.Settings["AccessToken"].Value;
            string environment = "";
            if (configuration.AppSettings.Settings["Environment"] != null)
                environment = configuration.AppSettings.Settings["Environment"].Value;

            DateTime tokenExpires = DateTime.MinValue;
            if (configuration.AppSettings.Settings["TokenExpires"] != null)
                DateTime.TryParse(configuration.AppSettings.Settings["TokenExpires"].Value, out tokenExpires);
            
            return new ClientSDK(configuration.AppSettings.Settings["Environment"].Value, accessToken);
            
        }
        public static void SetEnvironment(ClientSDK clientSDK, string environment)
        {
            switch (environment)
            {
                case "AqiIntegration":
                case "2":
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiIntegration);
                    break;
                case "AqiStage":
                case "3":
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);
                    break;
                case "AqiUSProduction":
                case "4":
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiUSProduction);
                    break;
                default:
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);
                    break;
            }
        }
        public static bool? SetEnvironment(ClientSDK clientSDK)
        {
            Console.WriteLine("Pick Environment");
            Console.WriteLine("x: Exit");
            Console.WriteLine("1: AQI Feature");
            Console.WriteLine("2: AQI Integration");
            Console.WriteLine("3: AQI Stage");
            Console.WriteLine("4: AQI US Production");
            var environment = Console.ReadLine().ToUpper();
            switch (environment)
            {
                case "1":
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);
                    return true;
                case "2":
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiIntegration);
                    return true;
                case "3":
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);
                    return true;
                case "4":
                    clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiUSProduction);
                    return true;
                case "X":
                    return null;
            }
            return false;
        }
    }
}
