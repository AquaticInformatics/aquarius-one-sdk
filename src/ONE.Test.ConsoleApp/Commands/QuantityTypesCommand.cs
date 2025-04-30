using System;
using System.Threading.Tasks;
using CommandLine;
using ONE.Models.CSharp;

namespace ONE.Test.ConsoleApp.Commands
{
    [Verb("quantitytypes", HelpText = "Retrieve Quantity Types.")]
    public class QuantityTypesCommand : ICommand
    {
        [Option('g', "guid", Required = false, HelpText = "Unit GUID")]
        public string Guid { get; set; }
        [Option('n', "name", Required = false, HelpText = "Unit Name")]
        public string Name { get; set; }

        async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
        {
            if (!string.IsNullOrEmpty(Guid) || !string.IsNullOrEmpty(Name))
            {
                await clientSdk.CacheHelper.LibraryCache.LoadAsync();
                QuantityType quantityType = null;
                if (!string.IsNullOrEmpty(Guid))
                    quantityType = clientSdk.CacheHelper.LibraryCache.GetQuantityType(Guid);
                else if (!string.IsNullOrEmpty(Name))
                    quantityType = clientSdk.CacheHelper.LibraryCache.GetQuantityTypeByName(Name);
                if (quantityType == null)
                    return 0;
                else
                    Console.WriteLine($"{quantityType.Id}: {quantityType.Name}");
            }
            else
            {
                var result = await clientSdk.Library.GetQuantityTypesAsync();
                if (result == null)
                    return 0;
                foreach (var quantityType in result)
                {
                    Console.WriteLine($"{quantityType.Id}: {quantityType.Name}");
                }
            }
            return 1;
        }
    }
}
