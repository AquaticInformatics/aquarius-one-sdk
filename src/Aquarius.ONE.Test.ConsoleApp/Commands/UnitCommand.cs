using CommandLine;
using ONE;
using ONE.Models.CSharp;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("units", HelpText = "Retrieves units.")]
    public class UnitCommand: ICommand
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
                Unit unit = null;
                if (!string.IsNullOrEmpty(Guid))
                    unit = clientSDK.CacheHelper.LibaryCache.GetUnit(Guid);
                else if (!string.IsNullOrEmpty(Name))
                    unit = clientSDK.CacheHelper.LibaryCache.GetUnitByName(Name);
                else if (Id > 0)
                    unit = clientSDK.CacheHelper.LibaryCache.GetUnit(Id);
                if (unit == null)
                    return 0;
                else
                    Console.WriteLine($"{unit.Id}: {unit.IntId}: {unit.I18NKey}");
            }
            else
            {
                var result = await clientSDK.Library.GetUnitsAsync();
                if (result == null)
                    return 0;
                foreach (var unit in result)
                {

                    Console.WriteLine($"{unit.Id}: {unit.IntId}: {unit.I18NKey}");
                }
            }
            return 1;
        }
    }
}
