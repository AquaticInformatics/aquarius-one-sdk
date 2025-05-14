using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using ONE.ClientSDK.Utilities.UI;

namespace ONE.Test.ConsoleApp.Commands
{
	[Verb("ui", HelpText = "Loads UI Definition.")]
	public class UIDefinitionCommand: ICommand
	{
		[Option('f', "filename", Required = true, HelpText = "Filename")]
		public string Filename { get; set; }
		async Task<int> ICommand.Execute(ClientSDK.OneApi clientSdk)
		{
			if (!File.Exists(Filename))
				return 0;
			else
			{
				string json = await File.ReadAllTextAsync(Filename);
				UIDefinition uIDefinition = new UIDefinition(json);
				Console.WriteLine(uIDefinition.ToString());
				return 1;
			}
		}
	}
}
