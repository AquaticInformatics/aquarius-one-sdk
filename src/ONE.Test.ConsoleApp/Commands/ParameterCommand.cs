using System;
using System.Threading.Tasks;
using CommandLine;
using ONE.ClientSDK.Common.Library;
using ONE.Models.CSharp;

namespace ONE.Test.ConsoleApp.Commands
{
    [Verb("parameters", HelpText = "Retrieve all parameters.")]
    public class ParameterCommand : ICommand
    {
        [Option('g', "guid", Required = false, HelpText = "Unit GUID")]
        public string Guid { get; set; }
        [Option('i', "intid", Required = false, HelpText = "Unit Integer Id")]
        public long Id { get; set; }
        [Option('n', "name", Required = false, HelpText = "Unit Name")]
        public string Name { get; set; }
        
        [Option('s', "show", Required = false, HelpText = "Show Cache")]
        public bool ShowCache { get; set; }

        [Option('c', "cache", Required = false, HelpText = "Cache Command (Clear)")]
        public string CacheCommand { get; set; }

        async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
        {
            if (CacheCommand != null && CacheCommand.ToUpper() == "CLEAR")
                CommandHelper.SetConfiguration("LibraryCache", "");
            string serializedCache = CommandHelper.GetConfiguration("LibraryCache");
            if (string.IsNullOrEmpty(serializedCache))
            {
                await clientSdk.CacheHelper.LibraryCache.LoadAsync("en-us", "AQI_MOBILE_RIO,AQI_FOUNDATION_LIBRARY");
                serializedCache = clientSdk.CacheHelper.LibraryCache.ToString();
                CommandHelper.SetConfiguration("LibraryCache", serializedCache);
            }
            // Load Cache
            var cache = LibraryCache.Load(serializedCache);

            if (ShowCache)
            {

                foreach (var parameter in cache.Parameters)
                {

                    Console.WriteLine($"{parameter.Id}: {parameter.IntId}: {parameter.I18NKey}");
                }
            }
            else if (!string.IsNullOrEmpty(Guid) || !string.IsNullOrEmpty(Name) || Id > 0)
            {

                await clientSdk.CacheHelper.LibraryCache.LoadAsync();
                Parameter parameter = null;
                if (!string.IsNullOrEmpty(Guid))
                    parameter = cache.GetParameter(Guid);
                else if (!string.IsNullOrEmpty(Name))
                    parameter = cache.GetParameterByName(Name);
                else if (Id > 0)
                    parameter = cache.GetParameter(Id);
                if (parameter == null)
                    return 0;
                else
                    Console.WriteLine($"{parameter.Id}: {parameter.IntId}: {parameter.I18NKey}");
            }
            else
            {

                var result = await clientSdk.Library.GetParametersAsync();
                if (result == null)
                    return 0;
                foreach (var parameter in result)
                {

                    Console.WriteLine($"{parameter.Id}: {parameter.IntId}: {parameter.I18NKey}");
                }
            }
            return 1;
        }
    }
}
