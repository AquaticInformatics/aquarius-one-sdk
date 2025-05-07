using System;
using System.Threading.Tasks;
using CommandLine;

namespace ONE.Test.ConsoleApp.Commands
{
	[Verb("i18nKeys", HelpText = "Retrieves i18nKeys.")]
	public class I18NCommand : ICommand
	{
		
		async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
		{
			var results = await clientSdk.Library.Geti18nKeysAsync("en-US", "AQI_FOUNDATION_LIBRARY");
			
			if (results == null)
				return 0;

			foreach (var result in results)
			{
				Console.WriteLine($"{result.Key}: {result.Type}: {result.Value}");
			}
			return 1;
		}
	}
}
