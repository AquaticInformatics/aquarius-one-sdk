using System;
using System.Threading.Tasks;
using CommandLine;
using ONE.Models.CSharp;

namespace ONE.Test.ConsoleApp.Commands
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
		async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
		{
			
			if (!string.IsNullOrEmpty(Guid) || !string.IsNullOrEmpty(Name) || Id > 0)
			{
				
				await clientSdk.CacheHelper.LibraryCache.LoadAsync();
				Unit unit = null;
				if (!string.IsNullOrEmpty(Guid))
					unit = clientSdk.CacheHelper.LibraryCache.GetUnit(Guid);
				else if (!string.IsNullOrEmpty(Name))
					unit = clientSdk.CacheHelper.LibraryCache.GetUnitByName(Name);
				else if (Id > 0)
					unit = clientSdk.CacheHelper.LibraryCache.GetUnit(Id);
				if (unit == null)
					return 0;
				else
					Console.WriteLine($"{unit.Id}: {unit.IntId}: {unit.I18NKey}");
			}
			else
			{
				var result = await clientSdk.Library.GetUnitsAsync();
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
