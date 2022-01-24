using CommandLine;
using Common.Library.Protobuf.Models;
using ONE;
using ONE.Common.Library;
using ONE.Operations;
using Operations.Spreadsheet.Protobuf.Models;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("operations", HelpText = "Operation Commands.")]
    public class OperationsCommand : ICommand
    {
        [Option('g', "guid", Required = false, HelpText = "Guid of the operation")]
        public string Guid { get; set; }

        [Option('c', "cache", Required = false, HelpText = "Cache Command (Clear)")]
        public string Cache { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
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
