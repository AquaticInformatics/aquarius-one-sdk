using CommandLine;
using ONE;
using ONE.Enterprise.Twin;
using ONE.Operations;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
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

        [Option('i', "import", Required = false, HelpText = "Import Operation")]
        public string ImportFileName { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            if (!string.IsNullOrEmpty(ImportFileName))
            {
                if (!File.Exists(ImportFileName))
                {
                    Console.WriteLine($"File does not exist: {ImportFileName}");
                    return 0;
                }
                if (string.IsNullOrEmpty(TenantId))
                {
                    Console.WriteLine($"Tenant Id not specified");
                    return 0;
                }
                try
                {
                    var tenant = await clientSDK.DigitalTwin.GetAsync(TenantId);
                    if (tenant == null || tenant.TwinTypeId != Constants.OrganizationCategory.TenantType.RefId)
                    {
                        Console.WriteLine($"Tenant Id not valid");
                        return 0;
                    }
                    string json = File.ReadAllText(ImportFileName);
                    OperationCache operationCache = new OperationCache(json);
                    CloneOperation cloneOperation = new CloneOperation(clientSDK, operationCache);
                    cloneOperation.Event += clientSDK.Logger.Logger_Event;
                    if (await cloneOperation.CloneAsync(clientSDK, tenant))
                    {
                        Console.WriteLine($"Import Succeeded");
                        return 1;
                    }
                    Console.WriteLine($"Import Failed");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Importing: {ex.Message}");
                    return 0;
                }
            }
            else if (string.IsNullOrEmpty(ExportFileName))
            {
                
                if (string.IsNullOrEmpty(Guid))
                {
                    Console.WriteLine($"Operation Id not specified");
                    return 0;
                }
                try
                {
                    await clientSDK.CacheHelper.OperationsCache.LoadOperationsAsync();
                    
                    OperationCache operationCache = clientSDK.CacheHelper.OperationsCache.GetOperationById(Guid);
                    if (operationCache == null)
                    {
                        Console.WriteLine($"Operation Id not valid {Guid}");
                        return 0;
                    }
                    if (!Directory.Exists(Path.GetDirectoryName(ExportFileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(ExportFileName));
                    File.WriteAllText(ExportFileName, operationCache.ToString());
                    Console.WriteLine($"Export Successful");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Exporting: {ex.Message}");
                    return 0;
                }
            }
            else
            {
                // Clear the client cache if requested
                if (Cache != null && Cache.ToUpper() == "CLEAR")
                    CommandHelper.SetConfiguration("OperationsCache", "");
                //Retrieve the operations from the client cache
                string serializedCache = CommandHelper.GetConfiguration("OperationsCache");
                if (string.IsNullOrEmpty(serializedCache))
                {
                    // If cache is empty, load all operation twins and cache
                    await clientSDK.CacheHelper.OperationsCache.LoadOperationsAsync();
                    serializedCache = clientSDK.CacheHelper.OperationsCache.ToString();
                    CommandHelper.SetConfiguration("OperationsCache", serializedCache);
                }

                // Load the serialized cache back into the OperationCache Object
                var operationsCache = new OperationsCache(serializedCache);

                // If there are no operations, (An empty collection of operations) try reloading
                if (operationsCache.Operations.Count == 0)
                {
                    await clientSDK.CacheHelper.OperationsCache.LoadOperationsAsync();
                    serializedCache = clientSDK.CacheHelper.OperationsCache.ToString();
                    CommandHelper.SetConfiguration("OperationsCache", serializedCache);
                    operationsCache = new OperationsCache(serializedCache);
                }

                if (string.IsNullOrEmpty(Guid))
                {
                    foreach (var operation in operationsCache.Operations)
                    {
                        Console.WriteLine($"{operation.Id}: {operation.Name}");
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
                            if (clientSDK.Authentication.IsAuthenticated)
                            {
                                clientSDK.CacheHelper.OperationsCache = new(clientSDK, operationsCache.ToString());
                                operationCache = clientSDK.CacheHelper.OperationsCache.GetOperationById(Guid);
                                if (operationCache != null)
                                {
                                    await operationCache.LoadAsync();
                                    serializedCache = clientSDK.CacheHelper.OperationsCache.ToString();
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
}
