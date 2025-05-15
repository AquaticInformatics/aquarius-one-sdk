using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using ONE.ClientSDK.Enterprise.Twin;
using ONE.ClientSDK.Operations.Spreadsheet;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Constants;
using ONE.Models.CSharp.Constants.TwinCategory;
using ONE.Models.CSharp.Imposed.Enums;
using ONE.Models.CSharp.Imposed.WorksheetView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorksheetView = ONE.Models.CSharp.Imposed.WorksheetView.WorksheetView;

namespace ONE.ClientSDK.Operations
{
	public class CloneOperation
	{
		private readonly OneApi _clientSdk;
		private readonly OperationCache _operationCache;

		public CloneOperation(OneApi clientSdk, OperationCache operationCache)
		{
			_clientSdk = clientSdk;
			_operationCache = operationCache;
		}

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public async Task<bool> CloneAsync(OneApi clientSdk, DigitalTwin destinationTenantTwin)
		{
			var sourceToDestinationLocationTwinMapping = new Dictionary<string, string>();
			var sourceOperationDigitalTwin = _operationCache.DigitalTwin;
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Creating Operation Twin" });
			var destinationOperationDigitalTwin = await clientSdk.DigitalTwin.CreateSpaceAsync(
                destinationTenantTwin.TwinReferenceId, $"Copy of: {sourceOperationDigitalTwin.Name}",
                sourceOperationDigitalTwin.TwinTypeId, sourceOperationDigitalTwin.TwinSubTypeId,
                sourceOperationDigitalTwin.SortOrder);
			if (destinationOperationDigitalTwin == null)
				return false;
			var twinData = DigitalTwinHelper.AddUpdateRootValue("CloneId", sourceOperationDigitalTwin.TwinReferenceId, sourceOperationDigitalTwin.TwinData);
			destinationOperationDigitalTwin.TwinData = twinData;
			destinationOperationDigitalTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
			destinationOperationDigitalTwin = await clientSdk.DigitalTwin.UpdateAsync(destinationOperationDigitalTwin);
			sourceToDestinationLocationTwinMapping.Add(sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

			//pbOperationStatus.Value = 50;
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Creating Spreadsheet" });

			var sourceSpreadsheetDefinition = _operationCache.SpreadsheetDefinition;
			var destinationSpreadsheetDefinition = new SpreadsheetDefinition
			{
				TimeWindowOffset = sourceSpreadsheetDefinition.TimeWindowOffset,
				TwinId = destinationOperationDigitalTwin.TwinReferenceId,
				EnumTimeZone = sourceSpreadsheetDefinition.EnumTimeZone
			};
			await clientSdk.Spreadsheet.SaveSpreadsheetDefinitionAsync(destinationOperationDigitalTwin.TwinReferenceId, destinationSpreadsheetDefinition);
			//pbOperationStatus.Value = 100;

			// Task 2 Copying Locations
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Loading Source Locations" });
			//pbLocationStatus.Maximum = sourceLocationDigitalTwins.Count;

			await CopyLocationsAsync(sourceToDestinationLocationTwinMapping, destinationOperationDigitalTwin.TwinReferenceId, _operationCache.DigitalTwin.TwinReferenceId, _operationCache.LocationTwins);

			// Task 3 Copying Columns
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Loading Column Twins" });
			var sourceColumnDigitalTwins = _operationCache.ColumnTwins;


			// Task 3.1 Worksheet 15 Min
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Copying 15 Min Worksheet" });
			await CopyWorksheetAsync(_operationCache.FifteenMinuteWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetFifteenMinute);

			// Task 3.2 Worksheet 1 hour
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Copying Hourly Worksheet" });
			await CopyWorksheetAsync(_operationCache.HourlyWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetHour);


			// Task 3.3 Worksheet 4 hour
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Copying 4 Hour Worksheet" });
			await CopyWorksheetAsync(_operationCache.FourHourWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetFourHour);

			// Task 3.4 Worksheet daily
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Copying Daily Worksheet" });

			await CopyWorksheetAsync(_operationCache.DailyWorksheetDefinition, sourceToDestinationLocationTwinMapping, sourceColumnDigitalTwins, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId, EnumWorksheet.WorksheetDaily);

			// Task 4 Converting Formulas
			// Task 4.1 Worksheet 15 Min
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Updating 15 Min Worksheet Computations" });
			await CopyWorksheetComputationsAsync(_operationCache.FifteenMinuteWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

			// Task 4.2 Worksheet 1 hour
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Updating Hourly Worksheet Computations" });
			await CopyWorksheetComputationsAsync(_operationCache.HourlyWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);


			// Task 4.3 Worksheet 4 hour
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Updating 4 Hour Worksheet Computations" });
			await CopyWorksheetComputationsAsync(_operationCache.FourHourWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

			// Task 4.4 Worksheet daily
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Updating Daily Worksheet Computations" });
			await CopyWorksheetComputationsAsync(_operationCache.DailyWorksheetDefinition, sourceOperationDigitalTwin.TwinReferenceId, destinationOperationDigitalTwin.TwinReferenceId);

			// Task 5 Copying Sheets (Views)
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Worksheet Sheets (Views)" });
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
			var sourceSheets = _operationCache.Sheets;
			var sourceToDestinationColumnNumberMap = await GetSourceToDestinationColumnMap(destinationOperationId);

			foreach (var sourceConfiguration in sourceSheets)
			{
				var destinationWorksheetViewConfiguration = new WorksheetView();
				var sourceConfigurationData = sourceConfiguration.ConfigurationData;
				var sourceWorksheetViewConfiguration = SpreadsheetHelper.GetWorksheetView(sourceConfigurationData);
				Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = $"Copying {sourceWorksheetViewConfiguration.worksheetType} Sheet: {sourceWorksheetViewConfiguration.name}" });

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

				await _clientSdk.Configuration.CreateConfigurationAsync(destinationOperationId, ConfigurationTypeConstants.WorksheetView.Id, destinationWorksheetViewConfiguration.ToString(), true);
			}

			return true;
		}

		private async Task<Dictionary<string, string>> CopyLocationsAsync(Dictionary<string, string> sourceToDestinationTwinMapping, string destinationParentId, string sourceParentId, List<DigitalTwin> sourceDigitalTwins)
		{
			foreach (var sourceTwin in DigitalTwinHelper.GetByParentRef(sourceDigitalTwins, sourceParentId))
			{
				Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = $"Copying Location Twin {sourceTwin.Name}" });
				var newTwin = new DigitalTwin
				{
					Name = sourceTwin.Name,
					ParentTwinReferenceId = destinationParentId,
					CategoryId = sourceTwin.CategoryId,
					TwinTypeId = sourceTwin.TwinTypeId,
					TwinSubTypeId = sourceTwin.TwinSubTypeId,
					TwinReferenceId = Guid.NewGuid().ToString()
				};
				newTwin = await _clientSdk.DigitalTwin.CreateAsync(newTwin);
				var twinData = DigitalTwinHelper.AddUpdateRootValue("CloneId", sourceTwin.TwinReferenceId, sourceTwin.TwinData);
				newTwin.TwinData = twinData;
				newTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
				newTwin = await _clientSdk.DigitalTwin.UpdateAsync(newTwin);
				if (newTwin == null)
					return sourceToDestinationTwinMapping;
				sourceToDestinationTwinMapping.Add(sourceTwin.TwinReferenceId, newTwin.TwinReferenceId);

				sourceToDestinationTwinMapping = await CopyLocationsAsync(sourceToDestinationTwinMapping, newTwin.TwinReferenceId, sourceTwin.TwinReferenceId, sourceDigitalTwins);
			}

			return sourceToDestinationTwinMapping;
		}

		private Column CopyColumn(string destinationOperationId, Dictionary<string, string> sourceToDestinationTwinMapping, List<DigitalTwin> columnTwins, Column sourceColumn)
		{
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = $"Copying {sourceColumn.Name}" });
			var destinationColumn = new Column
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
			var destinationColumnTwin = DigitalTwinHelper.GetByRef(columnTwins, sourceColumn.ColumnId);

			if (destinationColumnTwin == null)
				return null;

			destinationColumn.LocationId = sourceToDestinationTwinMapping[destinationColumnTwin.ParentTwinReferenceId];
			destinationColumn.ColumnId = Guid.NewGuid().ToString();

			return destinationColumn;
		}

		private async Task CopyWorksheetAsync(WorksheetDefinition sourceWorksheetDefinition,
			Dictionary<string, string> sourceToDestinationLocationTwinMapping, List<DigitalTwin> columnTwins,
			string sourceOperationId, string destinationOperationId, EnumWorksheet worksheetType)
		{
			if (sourceWorksheetDefinition?.Columns == null || sourceWorksheetDefinition.Columns.Count == 0)
				return;

			var destinationWorksheetDefinition = new WorksheetDefinition
			{
				EnumWorksheet = worksheetType,
				TwinId = destinationOperationId
			};

			var x = 1;

			foreach (var sourceColumn in sourceWorksheetDefinition.Columns)
			{
				var destinationColumn = CopyColumn(destinationOperationId, sourceToDestinationLocationTwinMapping, columnTwins, sourceColumn);
				if (destinationColumn == null) return;
				destinationWorksheetDefinition.Columns.Add(destinationColumn);
				if (x % 100 == 0)
				{
					var partialWorksheetDefinition = await _clientSdk.Spreadsheet.WorksheetAddColumnAsync(destinationOperationId, worksheetType, destinationWorksheetDefinition);
					if (partialWorksheetDefinition == null)
					{
						Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Failed to save worksheet definition" });
						return;
					}

					destinationWorksheetDefinition = new WorksheetDefinition
					{
						EnumWorksheet = worksheetType,
						TwinId = destinationOperationId
					};
				}

				x++;
			}

			var updatedWorksheetDefinition = await _clientSdk.Spreadsheet.WorksheetAddColumnAsync(destinationOperationId, worksheetType, destinationWorksheetDefinition);

			if (updatedWorksheetDefinition == null)
			{
				Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = "Failed to save worksheet definition" });

				return;
			}

			// Load up worksheet definition and update mapping
			// Note that the source column ID is currently in the note field.
			// This routine will add the source columnId and columnNumber to the twin property bag of the destination 
			for (x = 0; x < updatedWorksheetDefinition.Columns.Count; x++)
			{
				var destinationColumn = updatedWorksheetDefinition.Columns[x];
				var sourceColumn = _operationCache.GetColumnByGuid(destinationColumn.Description);
				if (null != await UpdateColumnTwinDataAsync(sourceColumn, destinationColumn))
				{
					updatedWorksheetDefinition.Columns[x].Description = sourceColumn.Description;
				}

			}
			var newWorksheetDefinition = await _clientSdk.Spreadsheet.WorksheetUpdateColumnAsync(destinationOperationId, worksheetType, updatedWorksheetDefinition);
		}

		private Dictionary<uint, uint> _sourceToDestinationColumnMap;
		public async Task<Dictionary<uint, uint>> GetSourceToDestinationColumnMap(string operationId)
		{
			if (_sourceToDestinationColumnMap != null)
				return _sourceToDestinationColumnMap;

			var sourceToDestinationColumnMap = new Dictionary<uint, uint>();
			var columnTwins = await _clientSdk.DigitalTwin.GetDescendantsByTypeAsync(operationId, TelemetryConstants.ColumnType.RefId);

			foreach (var columnTwin in columnTwins)
			{
				uint.TryParse(DigitalTwinHelper.GetTwinDataProperty(columnTwin, "", "CloneColumnNumber"), out var sourceColumnNumber);
				uint.TryParse(DigitalTwinHelper.GetTwinDataProperty(columnTwin, "columnDefinition", "columnNumber"), out var destinationColumnNumber);
				if (sourceColumnNumber > 0 && destinationColumnNumber > 0)
				{
					sourceToDestinationColumnMap.Add(sourceColumnNumber, destinationColumnNumber);

				}
			}
			_sourceToDestinationColumnMap = sourceToDestinationColumnMap;
			return sourceToDestinationColumnMap;
		}

		private async Task CopyWorksheetComputationsAsync(WorksheetDefinition sourceWorksheetDefinition,
			string sourceOperationId, string destinationOperationId)
		{
			if (sourceWorksheetDefinition == null) return;
			var destinationWorksheetDefinition = await _clientSdk.Spreadsheet.GetWorksheetDefinitionAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet);
			if (destinationWorksheetDefinition == null) return;
			var sourceToDestinationColumnNumberMap = await GetSourceToDestinationColumnMap(destinationOperationId);

			for (var x = 0; x < sourceWorksheetDefinition.Columns.Count; x++)
			{
				var sourceColumn = sourceWorksheetDefinition.Columns[x];
				var destinationColumn = GetColumnByNumber(destinationWorksheetDefinition, sourceToDestinationColumnNumberMap[sourceColumn.ColumnNumber]);

				if (sourceColumn.DataSourceBinding != null && !string.IsNullOrEmpty(sourceColumn.DataSourceBinding.BindingId))
				{
					Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = $"Copying Formula {sourceWorksheetDefinition.EnumWorksheet} Column: {sourceColumn.Name}" });

					var destinationDataSourceBinding = new DataSourceBinding();
					//Convert Computation
					var sourceDataSourceBinding = sourceColumn.DataSourceBinding;
					destinationDataSourceBinding.DataSource = sourceDataSourceBinding.DataSource;
					destinationDataSourceBinding.EnumSamplingStatistic = sourceDataSourceBinding.EnumSamplingStatistic;
					var sourceSpreadsheetComputation = _operationCache.SpreadsheetComputations[sourceColumn.DataSourceBinding.BindingId];
					var destinationSpreadsheetComputation = new SpreadsheetComputation { Computation = new Computation() };

					foreach (var sourceExpressionLine in sourceSpreadsheetComputation.Computation.ExpressionLines)
						destinationSpreadsheetComputation.Computation.ExpressionLines.Add(sourceExpressionLine);
					
					destinationSpreadsheetComputation.Computation.IsActive = sourceSpreadsheetComputation.Computation.IsActive;
					foreach (var sourceOutputVariable in sourceSpreadsheetComputation.Computation.OutputVariables)
						destinationSpreadsheetComputation.Computation.OutputVariables.Add(sourceOutputVariable);
					

					// Recreate Binding
					destinationSpreadsheetComputation.Binding = new ComputationBinding();
					foreach (var inputVariable in sourceSpreadsheetComputation.Computation.InputVariables)
						destinationSpreadsheetComputation.Computation.InputVariables.Add(new Variable { Name = inputVariable.Name });
					
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
					var errors = await _clientSdk.Spreadsheet.ComputationValidateAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet, destinationSpreadsheetComputation);
					
					if (errors != null && errors.Count == 0)
					{
						var spreadsheetComputation = await _clientSdk.Spreadsheet.ComputationCreateAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet, destinationSpreadsheetComputation);
						if (spreadsheetComputation != null)
						{
							destinationColumn.DataSourceBinding = new DataSourceBinding
							{
								DataSource = EnumDataSource.DatasourceComputation,
								EnumSamplingStatistic = EnumSamplingStatistic.SamplingStatisticRaw,
								BindingId = spreadsheetComputation.Binding.Id
							};
						}
					}
				}
			}

			await _clientSdk.Spreadsheet.WorksheetUpdateColumnAsync(destinationOperationId, sourceWorksheetDefinition.EnumWorksheet, destinationWorksheetDefinition);
		}

		private Column GetColumnByNumber(WorksheetDefinition worksheetDefinition, uint columnNumber)
		{
			try
			{
				return worksheetDefinition?.Columns?.FirstOrDefault(p => p.ColumnNumber == columnNumber);
			}
			catch
			{
				return null;
			}
		}

		private async Task<DigitalTwin> UpdateColumnTwinDataAsync(Column sourceColumn, Column destinationColumn)
		{
			Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OperationExport", Message = $"Updating Destination Column Twin {destinationColumn.ColumnId}" });
			var destinationTwin = await _clientSdk.DigitalTwin.GetAsync(destinationColumn.ColumnId);
			var sourceTwin = _operationCache.GetColumnTwinByGuid(sourceColumn.ColumnId);
			var destinationTwinData = destinationTwin.TwinData;
			dynamic sourceTwinDataJsonObj = JsonConvert.DeserializeObject(sourceTwin.TwinData);

			if (sourceTwinDataJsonObj["wims"] != null)
				destinationTwinData = DigitalTwinHelper.AddUpdateRootValue("wims", sourceTwinDataJsonObj["wims"], destinationTwinData);
			

			destinationTwinData = DigitalTwinHelper.AddUpdateRootValue("CloneId", sourceTwin.TwinReferenceId, destinationTwinData);
			destinationTwinData = DigitalTwinHelper.AddUpdateRootValue("CloneColumnNumber", sourceColumn.ColumnNumber.ToString(), destinationTwinData);
			destinationTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
			destinationTwin.TwinData = destinationTwinData;
			return await _clientSdk.DigitalTwin.UpdateAsync(destinationTwin);
		}
	}
}
