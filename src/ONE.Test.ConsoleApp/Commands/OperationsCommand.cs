using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using ONE.ClientSDK;
using ONE.ClientSDK.Operations;
using ONE.Models.CSharp.Constants.TwinCategory;

namespace ONE.Test.ConsoleApp.Commands
{
	[Verb("operations", HelpText = "Operation Commands.")]
	public class OperationsCommand : ICommand
	{
		[Option('g', "guid", Required = false, HelpText = "Guid of the operation")]
		public string Guid { get; set; }

		[Option('t', "tenantGuid", Required = false, HelpText = "Guid of the tenant")]
		public string TenantId { get; set; }

		[Option('c', "cache", Required = false, HelpText = "Cache Command (Clear)")]
		public string Cache { get; set; }

		[Option('e', "export", Required = false, HelpText = "Export Operation")]
		public string ExportFileName { get; set; }

		[Option('p', "exportpath", Required = false, HelpText = "Export Each Operation Separately")]
		public string ExportPath { get; set; }

		[Option('i', "import", Required = false, HelpText = "Import Operation")]
		public string ImportFileName { get; set; }

		async Task<int> ICommand.Execute(OneApi clientSdk)
		{
			// Import Cache File

			if (!string.IsNullOrEmpty(ImportFileName))
			{
				if (!File.Exists(ImportFileName))
				{
					Console.WriteLine($"File does not exist: {ImportFileName}");
					return 0;
				}
				if (string.IsNullOrEmpty(TenantId))
				{
					Console.WriteLine("Tenant Id not specified");
					return 0;
				}
				try
				{
					var tenant = await clientSdk.DigitalTwin.GetAsync(TenantId);
					if (tenant == null || tenant.TwinTypeId != OrganizationConstants.TenantType.RefId)
					{
						Console.WriteLine("Tenant Id not valid");
						return 0;
					}
					string json = File.ReadAllText(ImportFileName);
					OperationCache operationCache = new OperationCache(json);
					CloneOperation cloneOperation = new CloneOperation(clientSdk, operationCache);
					cloneOperation.Event += clientSdk.Logger.Logger_Event;
					if (await cloneOperation.CloneAsync(clientSdk, tenant))
					{
						Console.WriteLine("Import Succeeded");
						return 1;
					}
					Console.WriteLine("Import Failed");
					return 0;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error Importing: {ex.Message}");
					return 0;
				}
			}

			//Export Single Operation

			if (!string.IsNullOrEmpty(ExportFileName) && !string.IsNullOrEmpty(Guid))
			{

				try
				{
					await clientSdk.CacheHelper.OperationsCache.LoadOperationsAsync();

					OperationCache operationCache = clientSdk.CacheHelper.OperationsCache.GetOperationById(Guid);
					if (operationCache == null)
					{
						Console.WriteLine($"Operation Id not valid {Guid}");
						return 0;
					}
					if (!Directory.Exists(Path.GetDirectoryName(ExportFileName)))
						Directory.CreateDirectory(Path.GetDirectoryName(ExportFileName));
					File.WriteAllText(ExportFileName, operationCache.ToString());
					Console.WriteLine("Export Successful");
					return 0;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error Exporting: {ex.Message}");
					return 0;
				}
			}
			// Export All Operations

			if (!string.IsNullOrEmpty(ExportFileName))
			{

				try
				{
					await clientSdk.CacheHelper.OperationsCache.LoadOperationsAsync(true);
					if (!Directory.Exists(Path.GetDirectoryName(ExportFileName)))
						Directory.CreateDirectory(Path.GetDirectoryName(ExportFileName));
					File.WriteAllText(ExportFileName, clientSdk.CacheHelper.OperationsCache.ToString());
					Console.WriteLine("Export Successful");
					return 0;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error Exporting: {ex.Message}");
					return 0;
				}
			}
			// Export Each Operation Separately

			if (!string.IsNullOrEmpty(ExportPath))
			{
				try
				{
					await clientSdk.CacheHelper.OperationsCache.LoadOperationsAsync();
					if (!Directory.Exists(Path.GetDirectoryName(ExportPath)))
						Directory.CreateDirectory(Path.GetDirectoryName(ExportPath));
					foreach (var operationCache in clientSdk.CacheHelper.OperationsCache.Operations)
					{
						await operationCache.LoadAsync();
						File.WriteAllText(Path.Combine(ExportPath, operationCache.Id + ".json"), operationCache.ToString());

					}
					
					Console.WriteLine("Export Successful");
					return 0;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error Exporting: {ex.Message}");
					return 0;
				}
			}

			// Clear the client cache if requested
			if (Cache != null && Cache.ToUpper() == "CLEAR")
				CommandHelper.SetConfiguration("OperationsCache", "");
			//Retrieve the operations from the client cache
			string serializedCache = CommandHelper.GetConfiguration("OperationsCache");
			if (string.IsNullOrEmpty(serializedCache))
			{
				// If cache is empty, load all operation twins and cache
				await clientSdk.CacheHelper.OperationsCache.LoadOperationsAsync(true);
				serializedCache = clientSdk.CacheHelper.OperationsCache.ToString();
				CommandHelper.SetConfiguration("OperationsCache", serializedCache);
			}

			// Load the serialized cache back into the OperationCache Object
			var operationsCache = new OperationsCache(serializedCache);

			// If there are no operations, (An empty collection of operations) try reloading
			if (operationsCache.Operations.Count == 0)
			{
				await clientSdk.CacheHelper.OperationsCache.LoadOperationsAsync(true);
				serializedCache = clientSdk.CacheHelper.OperationsCache.ToString();
				CommandHelper.SetConfiguration("OperationsCache", serializedCache);
				operationsCache = new OperationsCache(serializedCache);
			}

			if (string.IsNullOrEmpty(Guid))
			{
				foreach (var operation in operationsCache.Operations)
				{
					Console.WriteLine($"{operation.Id}: {operation.Name}");
					foreach (var colTwin in operation.ColumnTwins)
						Console.WriteLine(operation.GetTelemetryPath(colTwin.TwinReferenceId, true));
				}
			}

			else
			{
				var operationCache = operationsCache.GetOperationById(Guid);
				if (operationCache != null)  // This will be null if the GUID of the operation was invalid
				{
					// If the GUID is valid and the operation exists, check to see if the operation configuration was cached
					if (!operationCache.IsCached)
					{
						if (clientSdk.Authentication.IsAuthenticated)
						{
							clientSdk.CacheHelper.OperationsCache = new(clientSdk, operationsCache.ToString());
							operationCache = clientSdk.CacheHelper.OperationsCache.GetOperationById(Guid);
							if (operationCache != null)
							{
								await operationCache.LoadAsync();
								serializedCache = clientSdk.CacheHelper.OperationsCache.ToString();
								CommandHelper.SetConfiguration("OperationsCache", serializedCache);
							}
						}

					}
					foreach (var columnTwin in operationCache.ColumnTwins)
					{
						Console.WriteLine($"{columnTwin.Name} {operationCache.GetTelemetryPath(columnTwin.TwinReferenceId, false)} {operationCache.Info(columnTwin.TwinReferenceId, "ENTRYMIN")}");
					}
				}

			}
			return 1;
		}
	}
}
