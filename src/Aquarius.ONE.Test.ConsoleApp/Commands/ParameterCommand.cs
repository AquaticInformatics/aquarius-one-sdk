using CommandLine;
using Common.Library.Protobuf.Models;
using ONE;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
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

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            if (!string.IsNullOrEmpty(Guid) || !string.IsNullOrEmpty(Name) || Id > 0)
            {

                await clientSDK.CacheHelper.LibaryCache.LoadAsync();
                Parameter parameter = null;
                if (!string.IsNullOrEmpty(Guid))
                    parameter = clientSDK.CacheHelper.LibaryCache.GetParameter(Guid);
                else if (!string.IsNullOrEmpty(Name))
                    parameter = clientSDK.CacheHelper.LibaryCache.GetParameterByName(Name);
                else if (Id > 0)
                    parameter = clientSDK.CacheHelper.LibaryCache.GetParameter(Id);
                if (parameter == null)
                    return 0;
                else
                    Console.WriteLine($"{parameter.Id}: {parameter.IntId}: {parameter.I18NKey}");
            }
            else
            {

                var result = await clientSDK.Library.GetParametersAsync();
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
