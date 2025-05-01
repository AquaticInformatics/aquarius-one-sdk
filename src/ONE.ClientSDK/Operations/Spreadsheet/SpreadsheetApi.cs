using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Operations.Spreadsheet
{
    public class SpreadsheetApi
    {
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public SpreadsheetApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
        {
            _apiHelper = apiHelper;
            _continueOnCapturedContext = continueOnCapturedContext;
            _throwApiErrors = throwApiErrors;
        }

        private readonly IOneApiHelper _apiHelper;
        private readonly bool _continueOnCapturedContext;
        private readonly bool _throwApiErrors;
        
        public async Task<Cell> CellValidateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, Cell cell, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/validateCell?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteSpreadSheetRequest("CellValidateAsync", HttpMethod.Post, endpoint, cancellation, cell).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.Cells?.Items.Values.FirstOrDefault();
        }

        public async Task<List<Measurement>> ColumnGetByDayAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date, CancellationToken cancellation = default)
        {
            var endpointUrl = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/column/{columnId}/byday/{date.Year}/{date.Month}/{date.Day}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ColumnGetByDayAsync", HttpMethod.Get, endpointUrl, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.Measurements?.Items.ToList();
        }

        public async Task<List<Measurement>> ColumnGetByMonthAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date, CancellationToken cancellation = default)
        {
            var endpointUrl = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/column/{columnId}/bymonth/{date.Year}/{date.Month}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ColumnGetByMonthAsync", HttpMethod.Get, endpointUrl, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.Measurements?.Items.ToList();
        }

        public async Task<List<Measurement>> ColumnGetByYearAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date, CancellationToken cancellation = default)
        {
            var endpointUrl = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/column/{columnId}/byyear/{date.Year}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ColumnGetByYearAsync", HttpMethod.Get, endpointUrl, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.Measurements?.Items.ToList();
        }

        public async Task<SpreadsheetComputation> ComputationCreateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, SpreadsheetComputation spreadsheetComputation, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/computation?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteSpreadSheetRequest("ComputationCreateAsync", HttpMethod.Post, endpoint, cancellation, spreadsheetComputation).ConfigureAwait(_continueOnCapturedContext);
                
            return apiResponse?.Content?.SpreadsheetComputations?.Items.FirstOrDefault();
        }

        public async Task<bool> ComputationExecuteAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, DataSourceBinding dataSourceBinding, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/execute?startRow={startRow}&endRow={endRow}&requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ComputationExecuteAsync", HttpMethod.Post, endpoint, cancellation, dataSourceBinding).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }
     
        public async Task<SpreadsheetComputation> ComputationGetOneAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, string computationBindingId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/computation/{computationBindingId}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ComputationGetOneAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.SpreadsheetComputations?.Items.FirstOrDefault();
        }
       
        public async Task<List<ApiError>> ComputationValidateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, SpreadsheetComputation spreadsheetComputation, CancellationToken cancellation)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/computation/validate?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ComputationValidateAsync", HttpMethod.Post, endpoint, cancellation, spreadsheetComputation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Errors.ToList() ?? new List<ApiError>();
        }

        public async Task<bool> DeletePlantAsync(string operationTwinReferenceId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/plant?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("DeletePlantAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<bool> FlushPlantAsync(string operationTwinReferenceId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/plant/flush?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteSpreadSheetRequest("FlushPlantAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<IEnumerable<CellValueBackup>> CellMonitorCellValuesAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, uint[] columns = null, string viewId = null, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/cellMonitor/cellValues?requestId={Guid.NewGuid()}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columns, viewId);

            var response = await ExecuteSpreadSheetRequest("CellMonitorCellValuesAsync", HttpMethod.Get, endpoint, cancellation);

            return response?.Content?.CellValues.Items;
        }

        public async Task<IEnumerable<OutputCellBackup>> CellMonitorOutputCellsAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, uint[] columns = null, string viewId = null, CancellationToken cancellation = default)
        {
            var endpoint =
                $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/cellMonitor/outputCells?requestId={Guid.NewGuid()}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columns, viewId);

            var response = await ExecuteSpreadSheetRequest("CellMonitorOutputCellsAsync", HttpMethod.Get, endpoint, cancellation);

            return response?.Content?.OutputCells.Items;
        }

        public async Task<bool> CellMonitorSyncAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, uint[] columns = null, string viewId = null, CancellationToken cancellation = default)
        {
            var endpoint =
                $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/cellMonitor/sync?requestId={Guid.NewGuid()}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columns, viewId);

            var response = await ExecuteSpreadSheetRequest("CellMonitorSyncAsync", HttpMethod.Post, endpoint, cancellation);

            return response?.StatusCode == 204;
        }

        public async Task<Rows> GetRowsAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, string columnList = null, string viewId = null, CancellationToken cancellation = default)
            => await GetSpreadsheetRowsAsync(operationTwinReferenceId, worksheetType, startRow, endRow, columnList, viewId, cancellation);

        public async Task<Rows> GetSpreadsheetRowsAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, string columnList = null, string viewId = null, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows?requestId={Guid.NewGuid()}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columnList?.Split(','), viewId);

            var apiResponse = await ExecuteSpreadSheetRequest("GetSpreadsheetRowsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.Rows;
        }

        public async Task<Rows> GetRowsByDayAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, DateTime date, string columnList = null, string viewId = null, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows/byday/{date.Year}/{date.Month}/{date.Day}?requestId={Guid.NewGuid()}";

            endpoint += AddColumnAndViewIdQueryString(columnList?.Split(','), viewId);

            var apiResponse = await ExecuteSpreadSheetRequest("GetRowsByDayAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.Rows;
        }

        public async Task<Rows> GetRowsByMonthAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, DateTime date, string columnList = null, string viewId = null, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows/bymonth/{date.Year}/{date.Month}?requestId={Guid.NewGuid()}";

            endpoint += AddColumnAndViewIdQueryString(columnList?.Split(','), viewId);

            var apiResponse = await ExecuteSpreadSheetRequest("GetRowsByMonthAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.Rows;
        }

        public async Task<SpreadsheetDefinition> GetSpreadsheetDefinitionAsync(string operationTwinReferenceId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/definition?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("GetSpreadsheetDefinitionAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.SpreadsheetDefinitions?.Items.FirstOrDefault();
        }

        public async Task<WorksheetDefinition> GetWorksheetDefinitionAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("GetWorksheetDefinitionAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.WorksheetDefinitions?.Items.FirstOrDefault();
        }

        /// <summary>
        /// Gets all worksheet definitions for an operation
        /// </summary>
        /// <param name="operationTwinReferenceId">The GUID identifier of the operation</param>
        /// <param name="cancellation"></param>
        public async Task<WorksheetDefinitions> GetWorksheetDefinitionsAsync(string operationTwinReferenceId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheetdefinitions?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("GetWorksheetDefinitionsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.WorksheetDefinitions;
        }

        public async Task<RowIndices> GetRowIndexesAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, string relativeTime, DateTime utcTime, bool isInSpeed, bool isRowCooked, bool isColumnsCooked, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/index?relativeTime={relativeTime}&utcTime={utcTime}&isInSpeed={isInSpeed}&isRowCooked={isRowCooked}&isColumnsCooked={isColumnsCooked}&requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("GetRowIndexesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);
            
            return apiResponse?.Content?.RowIndices;
        }

        public async Task<bool> SaveRowsAsync(Rows rows, string operationTwinReferenceId, EnumWorksheet worksheetType, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteSpreadSheetRequest("SaveRowsAsync", HttpMethod.Post, endpoint, cancellation, rows).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<bool> SaveSpreadsheetDefinitionAsync(string operationTwinReferenceId, SpreadsheetDefinition spreadsheetDefinition, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/definition?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteSpreadSheetRequest("SaveSpreadsheetDefinitionAsync", HttpMethod.Post, endpoint, cancellation, spreadsheetDefinition).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<WorksheetDefinition> WorksheetAddColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, WorksheetDefinition worksheetDefinition, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition/columns?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("WorksheetAddColumnAsync", HttpMethod.Post, endpoint, cancellation, worksheetDefinition).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.WorksheetDefinitions?.Items.FirstOrDefault();
        }

        public async Task<WorksheetDefinition> WorksheetUpdateColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, WorksheetDefinition worksheetDefinition, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition/columns?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("WorksheetUpdateColumnAsync", HttpMethod.Put, endpoint, cancellation, worksheetDefinition).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.WorksheetDefinitions?.Items.FirstOrDefault();
        }

        /// <summary>
        /// Removes a column from an existing Worksheet Definition
        /// </summary>
        /// <param name="operationTwinReferenceId">The GUID identifier of the operation</param>
        /// <param name="worksheetType">Worksheet type</param>
        /// <param name="columnId">The GUID identifier of the column to delete</param>
        /// <param name="cancellation"></param>
        /// <returns>True if successful</returns>
        public async Task<bool> WorksheetDeleteColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, string columnId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition/columns/{columnId}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("WorksheetDeleteColumnAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        /// <summary>
        /// Exports an operation's structure. (not data)
        /// </summary>
        /// <param name="operationTwinReferenceId">The identifier of the operation to export.</param>
        /// <param name="cancellation"></param>
        /// <returns>An <see cref="OperationExport"/> object.</returns>
        public async Task<OperationExport> ExportOperationAsync(string operationTwinReferenceId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/plant/export?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ExportOperationAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.OperationExports?.Items.FirstOrDefault();
        }

        /// <summary>
        /// Imports an operation based on an exported operation.
        /// </summary>
        /// <param name="operation">The operation to be cloned</param>
        /// <param name="operationTwinReferenceId">The identifier that the new operation will be assigned to.</param>
        /// <param name="tenantId">The parent tenant of the new operation.</param>
        /// <param name="cancellation"></param>
        /// <returns>True if successful.</returns>
        public async Task<KeyValues> ImportOperationAsync(OperationExport operation, string operationTwinReferenceId, string tenantId, CancellationToken cancellation = default)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/plant/import/{tenantId}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteSpreadSheetRequest("ImportOperationAsync", HttpMethod.Post, endpoint, cancellation, operation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse.Content.KeyValues;
        }

        private string AddColumnAndViewIdQueryString(IEnumerable<string> columnList = null, string viewId = null)
        {
            var i = 0;
            var queryString = string.Empty;

            if (columnList != null)
                queryString = columnList.Aggregate(queryString, (current, columnNumber) => current + $"&columns[{i++}]={columnNumber}");

            if (viewId != null)
                queryString += $"&viewId={viewId}";

            return queryString;
        }

        private string AddColumnAndViewIdQueryString(IEnumerable<uint> columnList = null, string viewId = null) =>
            AddColumnAndViewIdQueryString(columnList?.Select(c => c.ToString()), viewId);

        private async Task<ApiResponse> ExecuteSpreadSheetRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                
                var apiResponse = await _apiHelper.BuildRequestAndSendAsync<ApiResponse>(httpMethod, endpoint, cancellation, content).ConfigureAwait(_continueOnCapturedContext);

                watch.Stop();

                var message = " Success";
                var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

                if (!apiResponse.StatusCode.IsSuccessStatusCode())
                {
                    message = " Failed";
                    eventLevel = EnumOneLogLevel.OneLogLevelWarn;

                    if (_throwApiErrors)
                        throw new RestApiException(new ServiceResponse { ApiResponse = apiResponse, ElapsedMs = watch.ElapsedMilliseconds });
                }
                
                Event(null,
                    new ClientApiLoggerEventArgs
                    {
                        EventLevel = eventLevel,
                        HttpStatusCode = (HttpStatusCode)apiResponse.StatusCode,
                        ElapsedMs = watch.ElapsedMilliseconds,
                        Module = "SpreadsheetApi",
                        Message = callingMethod + message
                    });

                return apiResponse;
            }
            catch (Exception e)
            {
                Event(e,
                    new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelError,
                        Module = "SpreadsheetApi",
                        Message = $"{callingMethod} Failed - {e.Message}"
                    });
                if (_throwApiErrors)
                    throw;
                return null;
            }
        }
    }
}
