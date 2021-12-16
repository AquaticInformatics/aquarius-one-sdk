using CommandLine;
using Common.Library.Protobuf.Models;
using ONE;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("quantitytypes", HelpText = "Retrieve Quantity Types.")]
    public class QuantityTypesCommand : ICommand
    {
        [Option('g', "guid", Required = false, HelpText = "Unit GUID")]
        public string Guid { get; set; }
        [Option('n', "name", Required = false, HelpText = "Unit Name")]
        public string Name { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            if (!string.IsNullOrEmpty(Guid) || !string.IsNullOrEmpty(Name))
            {
                await clientSDK.LibraryHelper.LoadAsync();
                QuantityType quantityType = null;
                if (!string.IsNullOrEmpty(Guid))
                    quantityType = clientSDK.LibraryHelper.GetQuantityType(Guid);
                else if (!string.IsNullOrEmpty(Name))
                    quantityType = clientSDK.LibraryHelper.GetQuantityTypeByName(Name);
                if (quantityType == null)
                    return 0;
                else
                    Console.WriteLine($"{quantityType.Id}: {quantityType.Name}");
            }
            else
            {
                var result = await clientSDK.Library.GetQuantityTypesAsync();
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
