using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using ONE.Enterprise.Twin;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Constants;
using ONE.Models.CSharp.Constants.TwinCategory;
using ONE.Models.CSharp.Enums;
using ONE.Models.CSharp.Imposed.Enums;
using ONE.Models.CSharp.Imposed.WorksheetView;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using proto = ONE.Models.CSharp;
using WorksheetView = ONE.Models.CSharp.Imposed.WorksheetView.WorksheetView;

namespace ONE.Operations
{
    public class CloneOperation
    {
        private ClientSDK _clientSDK;
        private OperationCache _operationCache;
        public CloneOperation(ClientSDK clientSDK, OperationCache operationCache)
        {
            _clientSDK = clientSDK;
            _operationCache = operationCache;
        }
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public async Task<bool> CloneAsync(ClientSDK clientSDK, DigitalTwin destinationTenantTwin)
        {
            Dictionary<string, string> sourceToDestinationLocationTwinMapping = new Dictionary<string, string>();
            var sourceOperationDigitalTwin = _operationCache.DigitalTwin;
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Creating Operation Twin" });
            var destinationOperationDigitalTwin = await clientSDK.DigitalTwin.CreateSpaceAsync(destinationTenantTwin.TwinReferenceId, $"Copy of: {sourceOperationDigitalTwin.Name}", sourceOperationDigitalTwin.TwinTypeId, sourceOperationDigitalTwin.TwinSubTypeId);
            if (destinationOperationDigitalTwin == null)
                return false;
            string twinData = Enterprise.Twin.Helper.AddUpdateRootValue("CloneId", sourceOperationDigitalTwin.TwinReferenceId, sourceOperationDigitalTwin.TwinData);
            destinationOperationDigitalTwin.TwinData = twinData;
            destinationOperationDigitalTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
            destinationOperationDigitalTwin = await clientSDK.DigitalTwin.UpdateAsync(destinationOperationDigitalTwin);
            sourceToDestinationLocationTwinMapping.Add(sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

            //pbOperationStatus.Value = 50;
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Creating Spreadsheet" });

            SpreadsheetDefinition sourceSpreadsheetDefinition = _operationCache.SpreadsheetDefinition;
            SpreadsheetDefinition destinationSpreadsheetDefinition = new SpreadsheetDefinition
            {
                TimeWindowOffset = sourceSpreadsheetDefinition.TimeWindowOffset,
                TwinId = destinationOperationDigitalTwin.TwinReferenceId,
                EnumTimeZone = sourceSpreadsheetDefinition.EnumTimeZone
            };
            await clientSDK.Spreadsheet.SaveSpreadsheetDefinitionAsync(destinationOperationDigitalTwin.TwinReferenceId, destinationSpreadsheetDefinition);
            //pbOperationStatus.Value = 100;

            // Task 2 Copying Locations
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Loading Source Locations" });
            //pbLocationStatus.Maximum = sourceLocationDigitalTwins.Count;

            await CopyLocationsAsync(sourceToDestinationLocationTwinMapping, destinationOperationDigitalTwin.TwinReferenceId, _operationCache.DigitalTwin.TwinReferenceId, _operationCache.LocationTwins);

            // Task 3 Copying Columns
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Loading Column Twins" });
            var sourceColumnDigitalTwins = _operationCache.ColumnTwins;


            // Task 3.1 Worksheet 15 Min
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying 15 Min Worksheet" });
            await CopyWorksheetAsync(_operationCache.FifteenMinuteWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetFifteenMinute);

            // Task 3.2 Worksheet 1 hour
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying Hourly Worksheet" });
            await CopyWorksheetAsync(_operationCache.HourlyWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetHour);


            // Task 3.3 Worksheet 4 hour
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying 4 Hour Worksheet" });
            await CopyWorksheetAsync(_operationCache.FourHourWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetFourHour);

            // Task 3.4 Worksheet daily
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying Daily Worksheet" });

            await CopyWorksheetAsync(_operationCache.DailyWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetDaily);

            // Task 4 Converting Formulas
            // Task 4.1 Worksheet 15 Min
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Updating 15 Min Worksheet Computations" });
            await CopyWorksheetComputationsAsync(_operationCache.FifteenMinuteWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

            // Task 4.2 Worksheet 1 hour
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Updating Hourly Worksheet Computations" });
            await CopyWorksheetComputationsAsync(_operationCache.HourlyWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);


            // Task 4.3 Worksheet 4 hour
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Updating 4 Hour Worksheet Computations" });
            await CopyWorksheetComputationsAsync(_operationCache.FourHourWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

            // Task 4.4 Worksheet daily
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Updating Daily Worksheet Computations" });
            await CopyWorksheetComputationsAsync(_operationCache.DailyWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

            // Task 5 Copying Sheets (Views)
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Worksheet Sheets (Views)" });
            await CopySheetsAsync(destinationOperationDigitalTwin.TwinReferenceId);

            return true;

        }
        private dynamic ConvertViewHeader(Dictionary<uint, uint> sourceToDestinationColumnNumberMap, dynamic sourceHeader)
        {

            dynamic destinationHeader;
            if (sourceHeader.HeaderType == EnumHeaderType.column) //column
            {
                destinationHeader = new ColumnHeader { id = sourceToDestinationColumnNumberMap[sourceHeader.id] };
            }
            else
            {
                destinationHeader = new GroupHeader { groupId = sourceHeader.groupId, name = sourceHeader.name };
                destinationHeader.children = new List<dynamic>();
                foreach (var childHeader in sourceHeader.children)
                {
                    destinationHeader.children.Add(ConvertViewHeader(sourceToDestinationColumnNumberMap, childHeader));
                }
            }

            return destinationHeader;
        }
        private async Task<bool> CopySheetsAsync(string destinationOperationId)
        {

            List<Configuration> sourceSheets = _operationCache.Sheets;
            Dictionary<uint, uint> sourceToDestinationColumnNumberMap = await GetSourceToDestinationColumnMap(destinationOperationId);

            foreach (var sourceConfiguration in sourceSheets)
            {
                WorksheetView destinationWorksheetViewConfiguration = new WorksheetView();
                var sourceConfigurationData = sourceConfiguration.ConfigurationData;
                var sourceWorksheetViewConfiguration = Spreadsheet.Helper.GetWorksheetView(sourceConfigurationData);
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying {sourceWorksheetViewConfiguration.worksheetType} Sheet: {sourceWorksheetViewConfiguration.name}" });

                destinationWorksheetViewConfiguration.columnNumbers = new List<uint>();
                foreach (var columnNumber in sourceWorksheetViewConfiguration.columnNumbers)
                {
                    destinationWorksheetViewConfiguration.columnNumbers.Add(sourceToDestinationColumnNumberMap[columnNumber]);
                }
                destinationWorksheetViewConfiguration.worksheetType = sourceWorksheetViewConfiguration.worksheetType;
                destinationWorksheetViewConfiguration.name = sourceWorksheetViewConfiguration.name;
                destinationWorksheetViewConfiguration.headers = new List<dynamic>();
                foreach (var sourceHeader in sourceWorksheetViewConfiguration.headers)
                {
                    destinationWorksheetViewConfiguration.headers.Add(ConvertViewHeader(sourceToDestinationColumnNumberMap, sourceHeader));
                }

                await _clientSDK.Configuration.CreateConfigurationAsync(destinationOperationId, ConfigurationTypeConstants.WorksheetView.Id, destinationWorksheetViewConfiguration.ToString(), true);
            }
            return true;
        }

        private async Task<Dictionary<string, string>> CopyLocationsAsync(Dictionary<string, string> sourceToDestinationTwinMapping, string destinationParentId, string sourceParentId, List<DigitalTwin> sourceDigitalTwins)
        {
            foreach (var sourceTwin in Enterprise.Twin.Helper.GetByParentRef(sourceDigitalTwins, sourceParentId))
            {
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying Location Twin {sourceTwin.Name}" });
                DigitalTwin newTwin = new DigitalTwin
                {
                    Name = sourceTwin.Name,
                    ParentTwinReferenceId = destinationParentId,
                    CategoryId = sourceTwin.CategoryId,
                    TwinTypeId = sourceTwin.TwinTypeId,
                    TwinSubTypeId = sourceTwin.TwinSubTypeId,
                    TwinReferenceId = Guid.NewGuid().ToString()
                };
                newTwin = await _clientSDK.DigitalTwin.CreateAsync(newTwin);
                string twinData = Enterprise.Twin.Helper.AddUpdateRootValue("CloneId", sourceTwin.TwinReferenceId, sourceTwin.TwinData);
                newTwin.TwinData = twinData;
                newTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
                newTwin = await _clientSDK.DigitalTwin.UpdateAsync(newTwin);
                if (newTwin == null)
                    return sourceToDestinationTwinMapping;
                sourceToDestinationTwinMapping.Add(sourceTwin.TwinReferenceId, newTwin.TwinReferenceId);

                //sourceDigitalTwinModelItem.AddUpdateChild(newTwin);
                //var newDigitalTwinModelItem = sourceDigitalTwinModelItem.Get(newTwin.Id);
                //pbLocationStatus.Value++;
                sourceToDestinationTwinMapping = await CopyLocationsAsync(sourceToDestinationTwinMapping, newTwin.TwinReferenceId, sourceTwin.TwinReferenceId, sourceDigitalTwins);
            }
            return sourceToDestinationTwinMapping;
        }
        private Column CopyColumn(string destinationOperationId, Dictionary<string, string> sourceToDestinationTwinMapping, List<DigitalTwin> columnTwins, Column sourceColumn)
        {
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying {sourceColumn.Name}" });
            Column destinationColumn = new Column
            {
                Name = sourceColumn.Name,
                ColumnNumber = sourceColumn.ColumnNumber,
                Description = sourceColumn.ColumnId,  //Temporarily store the Source Column ID here so that it can be transferred a little later into the TwinData CloneId
                DisplayUnitId = sourceColumn.DisplayUnitId,
                IsActive = sourceColumn.IsActive,
                IsNumeric = sourceColumn.IsNumeric
            };
            if (sourceColumn.Limits != null)
            {
                foreach (var sourceLimit in sourceColumn.Limits)
                {
                    destinationColumn.Limits.Add(sourceLimit);
                }
            }
            destinationColumn.ParameterId = sourceColumn.ParameterId;
            destinationColumn.PlantId = destinationOperationId;
            if (destinationColumn.ReportableQualifierDefinitions != null)
            {
                foreach (var sourceReportableQualifierDefinition in destinationColumn.ReportableQualifierDefinitions)
                {
                    sourceColumn.ReportableQualifierDefinitions.Add(sourceReportableQualifierDefinition);
                }
            }
            destinationColumn.ValidValues = destinationColumn.ValidValues;
            var destinationColumnTwin = Enterprise.Twin.Helper.GetByRef(columnTwins, sourceColumn.ColumnId);
            if (destinationColumnTwin == null)
                return null;
            destinationColumn.LocationId = sourceToDestinationTwinMapping[destinationColumnTwin.ParentTwinReferenceId];
            destinationColumn.ColumnId = Guid.NewGuid().ToString();

            return destinationColumn;
        }
        private async Task<bool> CopyWorksheetAsync(WorksheetDefinition sourceWorksheetDefinition, Dictionary<string, string> sourceToDestinationLocationTwinMapping, List<DigitalTwin> columnTwins, string sourceOperationId, string destinationOperationId, EnumWorksheet worksheetType)
        {
            if (sourceWorksheetDefinition == null || sourceWorksheetDefinition.Columns == null || sourceWorksheetDefinition.Columns.Count == 0)
                return true;
            WorksheetDefinition destinationWorksheetDefinition = new WorksheetDefinition
            {
                EnumWorksheet = worksheetType,
                TwinId = destinationOperationId
            };
            int x = 1;

            foreach (var sourceColumn in sourceWorksheetDefinition.Columns)
            {
                var destinationColumn = CopyColumn(destinationOperationId, sourceToDestinationLocationTwinMapping, columnTwins, sourceColumn);
                if (destinationColumn == null)
                    return false;
                destinationWorksheetDefinition.Columns.Add(destinationColumn);
                if (x % 100 == 0)
                {
                    var partialWorksheetDefinition = await _clientSDK.Spreadsheet.WorksheetAddColumnAsync(destinationOperationId, worksheetType, destinationWorksheetDefinition);
                    if (partialWorksheetDefinition == null)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Failed to save worksheet definition" });
                        return false;
                    }
                    else
                    {
                        destinationWorksheetDefinition = new WorksheetDefinition
                        {
                            EnumWorksheet = worksheetType,
                            TwinId = destinationOperationId
                        };
                    }
                }
                x++;
            }
            var updatedWorksheetDefinition = await _clientSDK.Spreadsheet.WorksheetAddColumnAsync(destinationOperationId, worksheetType, destinationWorksheetDefinition);
            if (updatedWorksheetDefinition == null)
            {
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Failed to save worksheet definition" });

                return false;
            }
            else
            {
                // Load up worksheet definition and update mapping
                // Note that the source column Id is currently in the note field.
                // This routine will add the source columnId and columnNumber to the twin property bag of the destination 
                for (x = 0; x < updatedWorksheetDefinition.Columns.Count; x++)
                {
                    var destinatinationColumn = updatedWorksheetDefinition.Columns[x];
                    var sourceColumn = _operationCache.GetColumnByGuid(destinatinationColumn.Description);
                    if (null != await UpdateColumnTwinDataAsync(sourceColumn, destinatinationColumn))
                    {
                        updatedWorksheetDefinition.Columns[x].Description = sourceColumn.Description;
                    }

                }
                var newWorksheetDefinition = await _clientSDK.Spreadsheet.WorksheetUpdateColumnAsync(destinationOperationId, worksheetType, updatedWorksheetDefinition);
                return newWorksheetDefinition != null;

            }
        }

        private Dictionary<uint, uint> _sourceToDestinationColumnMap;
        public async Task<Dictionary<uint, uint>> GetSourceToDestinationColumnMap(string operationId)
        {
            if (_sourceToDestinationColumnMap != null)
                return _sourceToDestinationColumnMap;
            Dictionary<uint, uint> sourceToDestinationColumnMap = new Dictionary<uint, uint>();
            var columnTwins = await _clientSDK.DigitalTwin.GetDescendantsByTypeAsync(operationId, TelemetryConstants.ColumnType.RefId);
            foreach (var columnTwin in columnTwins)
            {
                uint.TryParse(Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, "", "CloneColumnNumber"), out uint sourceColumnNumber);
                uint.TryParse(Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, "columnDefinition", "columnNumber"), out uint destinationColumnNumber);
                if (sourceColumnNumber > 0 && destinationColumnNumber > 0)
                {
                    sourceToDestinationColumnMap.Add(sourceColumnNumber, destinationColumnNumber);

                }
            }
            _sourceToDestinationColumnMap = sourceToDestinationColumnMap;
            return sourceToDestinationColumnMap;
        }

        private async Task<bool> CopyWorksheetComputationsAsync(WorksheetDefinition sourceWorksheetDefinition, string sourceOperationId, string destinationOperationId)
        {
            if (sourceWorksheetDefinition == null)
                return true;
            var destinationWorksheetDefinition = await _clientSDK.Spreadsheet.GetWorksheetDefinitionAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet);
            if (destinationWorksheetDefinition == null)
                return false;
            Dictionary<uint, uint> sourceToDestinationColumnNumberMap = await GetSourceToDestinationColumnMap(destinationOperationId);

            for (int x = 0; x < sourceWorksheetDefinition.Columns.Count; x++)
            {
                var sourceColumn = sourceWorksheetDefinition.Columns[x];
                var destinatinationColumn = GetColumnByNumber(destinationWorksheetDefinition, sourceToDestinationColumnNumberMap[sourceColumn.ColumnNumber]);

                if (sourceColumn.DataSourceBinding != null && !string.IsNullOrEmpty(sourceColumn.DataSourceBinding.BindingId))
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Copying Formula {sourceWorksheetDefinition.EnumWorksheet} Column: {sourceColumn.Name}" });

                    DataSourceBinding destinationDataSourceBinding = new DataSourceBinding();
                    //Convert Computation
                    var sourceDataSourceBinding = sourceColumn.DataSourceBinding;
                    destinationDataSourceBinding.DataSource = sourceDataSourceBinding.DataSource;
                    destinationDataSourceBinding.EnumSamplingStatistic = sourceDataSourceBinding.EnumSamplingStatistic;
                    SpreadsheetComputation sourceSpreadsheetComputation = _operationCache.SpreadsheetComputations[sourceColumn.DataSourceBinding.BindingId];
                    SpreadsheetComputation destinationSpreadsheetComputation = new SpreadsheetComputation
                    {
                        Computation = new Computation()
                    };
                    foreach (var sourceExpressionline in sourceSpreadsheetComputation.Computation.ExpressionLines)
                    {
                        destinationSpreadsheetComputation.Computation.ExpressionLines.Add(sourceExpressionline);
                    }
                    destinationSpreadsheetComputation.Computation.IsActive = sourceSpreadsheetComputation.Computation.IsActive;
                    foreach (var sourceOutputVariable in sourceSpreadsheetComputation.Computation.OutputVariables)
                    {
                        destinationSpreadsheetComputation.Computation.OutputVariables.Add(sourceOutputVariable);
                    }

                    // Recreate Binding
                    destinationSpreadsheetComputation.Binding = new ComputationBinding();
                    foreach (var inputvariable in sourceSpreadsheetComputation.Computation.InputVariables)
                    {
                        destinationSpreadsheetComputation.Computation.InputVariables.Add(new proto.Variable { Name = inputvariable.Name });
                    }
                    foreach (var sourceComputationVariableBinding in sourceSpreadsheetComputation.Binding.InputVariableBindings)
                    {
                        destinationSpreadsheetComputation.Binding.InputVariableBindings.Add(new ComputationVariableBinding
                        {
                            VariableName = sourceComputationVariableBinding.VariableName,
                            ColumnNum = sourceToDestinationColumnNumberMap[sourceComputationVariableBinding.ColumnNum],
                            RowOffset = sourceComputationVariableBinding.RowOffset,
                            CellRange = sourceComputationVariableBinding.CellRange,
                            CellRangeAction = sourceComputationVariableBinding.CellRangeAction,
                            FunctionName = sourceComputationVariableBinding.FunctionName,
                            RequiredValues = sourceComputationVariableBinding.RequiredValues

                        });
                    }
                    destinationSpreadsheetComputation.Binding.RequireAllInputs = sourceSpreadsheetComputation.Binding.RequireAllInputs;
                    destinationSpreadsheetComputation.Binding.OutputColumnNumber = sourceToDestinationColumnNumberMap[sourceSpreadsheetComputation.Binding.OutputColumnNumber];
                    destinationSpreadsheetComputation.Binding.IsValid = sourceSpreadsheetComputation.Binding.IsValid;
                    var errors = await _clientSDK.Spreadsheet.ComputationValidateAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet, destinationSpreadsheetComputation);
                    if (errors != null && errors.Count == 0)
                    {
                        var spreadsheetComputation = await _clientSDK.Spreadsheet.ComputationCreateAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet, destinationSpreadsheetComputation);
                        if (spreadsheetComputation != null)
                        {
                            destinatinationColumn.DataSourceBinding = new DataSourceBinding
                            {
                                DataSource = EnumDataSource.DatasourceComputation,
                                EnumSamplingStatistic = EnumSamplingStatistic.SamplingStatisticRaw,
                                BindingId = spreadsheetComputation.Binding.Id
                            };
                        }

                    }
                }
            }
            await _clientSDK.Spreadsheet.WorksheetUpdateColumnAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet, destinationWorksheetDefinition);
            return true;
        }
        private Column GetColumnByNumber(WorksheetDefinition worksheetDefinition, uint columnNumber)
        {
            if (worksheetDefinition == null)
                return null;
            try
            {
                var matches = worksheetDefinition.Columns.Where(p => p.ColumnNumber == columnNumber);
                if (matches.Count() > 0)
                {
                    return matches.First();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        private async Task<DigitalTwin> UpdateColumnTwinDataAsync(Column sourceColumn, Column destinationColumn)
        {
            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, Module = "OperationExport", Message = $"Updating Destination Column Twin {destinationColumn.ColumnId}" });
            DigitalTwin destinationTwin = await _clientSDK.DigitalTwin.GetAsync(destinationColumn.ColumnId);
            DigitalTwin sourceTwin = _operationCache.GetColumnTwinByGuid(sourceColumn.ColumnId);
            string destinationTwinData = destinationTwin.TwinData;
            dynamic sourceTwinDataJsonObj = JsonConvert.DeserializeObject(sourceTwin.TwinData);
            if (sourceTwinDataJsonObj["wims"] != null)
            {
                destinationTwinData = Enterprise.Twin.Helper.AddUpdateRootValue("wims", sourceTwinDataJsonObj["wims"], destinationTwinData);
            }
            destinationTwinData = Enterprise.Twin.Helper.AddUpdateRootValue("CloneId", sourceTwin.TwinReferenceId, destinationTwinData);
            destinationTwinData = Enterprise.Twin.Helper.AddUpdateRootValue("CloneColumnNumber", sourceColumn.ColumnNumber.ToString(), destinationTwinData);
            destinationTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
            destinationTwin.TwinData = destinationTwinData;
            return await _clientSDK.DigitalTwin.UpdateAsync(destinationTwin);
        }
    }
}
