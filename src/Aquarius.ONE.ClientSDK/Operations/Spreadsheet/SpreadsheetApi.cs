using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ONE.Utilities;
using System.Collections.Generic;
using Google.Protobuf;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Enums;
using System.Net;
using ONE.Enums;

namespace ONE.Operations.Spreadsheet
{
    public class SpreadsheetApi
    {
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
        public SpreadsheetApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _throwAPIErrors = throwAPIErrors;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private readonly bool _throwAPIErrors;
        private RestHelper _restHelper;
        public async Task<Cell> CellValidateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, Cell cell)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/validateCell?requestId={requestId}";
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(cell, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var result = apiResponse.Content.Cells.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"CellValidateAsync Success" });
                    return result[0].Value;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"CellValidateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"CellValidateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        public async Task<List<Measurement>> ColumnGetByDayAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var worksheetDefinitionEndpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/column/{columnId}/byday/{date.Year}/{date.Month}/{date.Day}?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, worksheetDefinitionEndpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.Measurements.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ColumnGetByDayAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ColumnGetByDayAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ColumnGetByDayAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<Measurement>> ColumnGetByMonthAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var worksheetDefinitionEndpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/column/{columnId}/bymonth/{date.Year}/{date.Month}?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, worksheetDefinitionEndpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.Measurements.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ColumnGetByMonthAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ColumnGetByMonthAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ColumnGetByMonthAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<Measurement>> ColumnGetByYearAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var worksheetDefinitionEndpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/column/{columnId}/byyear/{date.Year}?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, worksheetDefinitionEndpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.Measurements.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ColumnGetByYearAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ColumnGetByYearAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ColumnGetByYearAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        public async Task<SpreadsheetComputation> ComputationCreateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, SpreadsheetComputation spreadsheetComputation)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/computation?requestId={requestId}";
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(spreadsheetComputation, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var result = apiResponse.Content.SpreadsheetComputations.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationCreateAsync Success" });
                    return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationCreateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ComputationCreateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        
        public async Task<SpreadsheetComputation> ComputationExecuteAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, DataSourceBinding dataSourceBinding)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/execute?startRow={startRow}&endRow={endRow}&requestId={requestId}";
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(dataSourceBinding, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var result = apiResponse.Content.SpreadsheetComputations.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationCreateAsync Success" });
                    return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationCreateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ComputationCreateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
     
        public async Task<SpreadsheetComputation> ComputationGetOneAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, string computationBindingId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/computation/{computationBindingId}?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.SpreadsheetComputations.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationGetOneAsync Success" });
                    if (result.Count == 1)
                        return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationGetOneAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ComputationGetOneAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
       
        public async Task<List<ApiError>> ComputationValidateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, SpreadsheetComputation spreadsheetComputation)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/computation/validate?requestId={requestId}";
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(spreadsheetComputation, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var result = apiResponse.Errors.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationValidateAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ComputationValidateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ComputationValidateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeletePlantAsync(string operationTwinReferenceId)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"operations/spreadsheet/v1/{operationTwinReferenceId}/plant?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"DeletePlantAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"DeletePlantAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"DeletePlantAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> FlushPlantAsync(string operationTwinReferenceId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/plant/flush?requestId={requestId}";
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = "";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    /*
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var result = apiResponse.Content.SpreadsheetComputations.Items.Select(x => x).ToList();
                    */
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"PlantFlushAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"PlantFlushAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"PlantFlushAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }

        public async Task<IEnumerable<CellValueBackup>> CellMonitorCellValuesAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, uint[] columns = null, string viewId = null)
        {
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/cellMonitor/cellValues?requestId={Guid.NewGuid()}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columns, viewId);

            var response = await ExecuteSpreadSheetRequest("CellMonitorCellValuesAsync", EnumHttpMethod.Get, endpoint);

            return response?.Content?.CellValues.Items;
        }

        public async Task<IEnumerable<OutputCellBackup>> CellMonitorOutputCellsAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, uint[] columns = null, string viewId = null)
        {
            var endpoint =
                $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/cellMonitor/outputCells?requestId={Guid.NewGuid()}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columns, viewId);

            var response = await ExecuteSpreadSheetRequest("CellMonitorOutputCellsAsync", EnumHttpMethod.Get, endpoint);

            return response?.Content?.OutputCells.Items;
        }

        public async Task<bool> CellMonitorSyncAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, uint[] columns = null, string viewId = null)
        {
            var endpoint =
                $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/cellMonitor/sync?requestId={Guid.NewGuid()}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columns, viewId);

            var response = await ExecuteSpreadSheetRequest("CellMonitorSyncAsync", EnumHttpMethod.Post, endpoint);

            return response?.StatusCode == 204;
        }

        public async Task<Rows> GetRowsAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, string columnList = null, string viewId = null)
        {
            return await GetSpreadsheetRowsAsync(operationTwinReferenceId, worksheetType, startRow, endRow, columnList, viewId);
        }
        public async Task<Rows> GetSpreadsheetRowsAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, string columnList = null, string viewId = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows?requestId={requestId}&startRow={startRow}&endRow={endRow}";

            endpoint += AddColumnAndViewIdQueryString(columnList?.Split(','), viewId);

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetSpreadsheetRowsAsync Success" });
                    return respContent.ApiResponse.Content.Rows;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetSpreadsheetRowsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"GetSpreadsheetRowsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Rows> GetRowsByDayAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, DateTime date, string columnList = null, string viewId = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows/byday/{date.Year}/{date.Month}/{date.Day}?requestId={requestId}&columns={columnList}&viewid={viewId}";
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetRowsByDayAsync Success" });
                    return respContent.ApiResponse.Content.Rows;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetRowsByDayAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"GetRowsByDayAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Rows> GetRowsByMonthAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, DateTime date, string columnList = null, string viewId = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows/bymonth/{date.Year}/{date.Month}?requestId={requestId}&columns={columnList}&viewid={viewId}";
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetRowsByMonthAsync Success" });
                    return respContent.ApiResponse.Content.Rows;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetRowsByMonthAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"GetRowsByMonthAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<SpreadsheetDefinition> GetSpreadsheetDefinitionAsync(string operationTwinReferenceId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/definition?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.SpreadsheetDefinitions.Items.Select(x => x).ToList();
                    if (result.Count == 1)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetSpreadsheetDefinitionAsync Success" });
                        return result[0];
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetSpreadsheetDefinitionAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"GetSpreadsheetDefinition Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<WorksheetDefinition> GetWorksheetDefinitionAsync(string operationTwinReferenceId, EnumWorksheet worksheetType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var worksheetDefinitionEndpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, worksheetDefinitionEndpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.WorksheetDefinitions.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetWorksheetDefinitionAsync Success" });
                    return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetWorksheetDefinitionAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"GetWorksheetDefinition Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        /// <summary>
        /// Gets all worksheet definitions for an operation
        /// </summary>
        /// <param name="operationTwinReferenceId">The GUID identifier of the operation</param>
        /// <returns></returns>
        public async Task<WorksheetDefinitions> GetWorksheetDefinitionsAsync(string operationTwinReferenceId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var url = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheetdefinitions?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.WorksheetDefinitions;
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetWorksheetDefinitionsAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetWorksheetDefinitionsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"GetWorksheetDefinitionsAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }

        public async Task<RowIndices> GetRowIndexesAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, string relativeTime, DateTime utcTime, bool isInSpeed, bool isRowCooked, bool isColumnsCooked)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/index?relativeTime={relativeTime}&utcTime={utcTime}&isInSpeed={isInSpeed}&isRowCooked={isRowCooked}&isColumnsCooked={isColumnsCooked}&requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.RowIndices;
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetRowIndexesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"GetRowIndexesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"GetRowIndexesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> SaveRowsAsync(Rows rows, string operationTwinReferenceId, EnumWorksheet worksheetType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/rows?requestId={requestId}";
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(rows, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"SaveRowsAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"SaveRowsAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"SaveRowsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> SaveSpreadsheetDefinitionAsync(string operationTwinReferenceId, SpreadsheetDefinition spreadsheetDefinition)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/definition?requestId={requestId}";
            var json = JsonConvert.SerializeObject(spreadsheetDefinition);
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"SaveSpreadsheetDefinitionAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"SaveSpreadsheetDefinitionAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"SaveSpreadsheetDefinition Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<WorksheetDefinition> WorksheetAddColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, WorksheetDefinition worksheetDefinition)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition/columns?requestId={requestId}";
            var json = JsonConvert.SerializeObject(worksheetDefinition, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"WorksheetAddColumnAsync Success" });
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var result = apiResponse.Content.WorksheetDefinitions.Items.Select(x => x).ToList();
                    return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"WorksheetAddColumnAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"WorksheetAddColumnAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<WorksheetDefinition> WorksheetUpdateColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, WorksheetDefinition worksheetDefinition)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition/columns?requestId={requestId}";
            var json = JsonConvert.SerializeObject(worksheetDefinition, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"WorksheetUpdateColumnAsync Success" });
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var result = apiResponse.Content.WorksheetDefinitions.Items.Select(x => x).ToList();
                    return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"WorksheetUpdateColumnAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"WorksheetUpdateColumnAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        
        /// <summary>
        /// Removes a column from an existing Worksheet Definition
        /// </summary>
        /// <param name="operationTwinReferenceId">The GUID identifier of the operation</param>
        /// <param name="worksheetType">Worksheet type</param>
        /// <param name="columnId">The GUID identifier of the column to delete</param>
        /// <returns>True if successful</returns>
        public async Task<bool> WorksheetDeleteColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, string columnId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operations/spreadsheet/v1/{operationTwinReferenceId}/worksheet/{(int)worksheetType}/definition/columns/{columnId}?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"WorksheetDeleteColumnAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"WorksheetDeleteColumnAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"WorksheetDeleteColumnAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }

        /// <summary>
        /// Exports an operation's structure. (not data)
        /// </summary>
        /// <param name="operationTwinReferenceId">The identifier of the operation to export.</param>
        /// <returns>An <see cref="OperationExport"/> object.</returns>
        public async Task<OperationExport> ExportOperationAsync(string operationTwinReferenceId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operation/spreadsheet/v1/{operationTwinReferenceId}/plant/export?requestId={requestId}";

            try
            {
                var response = await _restHelper.GetRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = response.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ExportOperationAsync Success" });
                    return response.ApiResponse.Content.OperationExports.Items.First();
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = response.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ExportOperationAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ExportOperationAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }

        /// <summary>
        /// Imports an operation based on an exported operation.
        /// </summary>
        /// <param name="operation">The operation to be cloned</param>
        /// <param name="operationTwinReferenceId">The identifier that the new operation will be assigned to.</param>
        /// <param name="tenantId">The parent tenant of the new operation.</param>
        /// <returns>True if successful.</returns>
        public async Task<bool> ImportOperationAsync(OperationExport operation, string operationTwinReferenceId, string tenantId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"operation/spreadsheet/v1/{operationTwinReferenceId}/plant/import/{tenantId}?requestId={requestId}";
            var json = JsonConvert.SerializeObject(operation, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            try
            {
                var response = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = response.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ImportOperationAsync Success" });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = response.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SpreadsheetApi", Message = $"ImportOperationAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "SpreadsheetApi", Message = $"ImportOperationAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                    throw;
                return false;
            }
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

        private async Task<ApiResponse> ExecuteSpreadSheetRequest(string callingMethod, EnumHttpMethod httpMethod, string endpoint, IMessage content = null)
        {
            try
            {
                var respContent = await _restHelper.ExecuteProtobufRequestAsync(httpMethod, endpoint, content).ConfigureAwait(_continueOnCapturedContext);

                var message = " Success";
                var eventLevel = EnumLogLevel.Trace;
                
                if (!respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    message = " Failed";
                    eventLevel = EnumLogLevel.Warn;
                }

                Event(null,
                    new ClientApiLoggerEventArgs
                    {
                        EventLevel = eventLevel,
                        HttpStatusCode = respContent.ResponseMessage.StatusCode,
                        ElapsedMs = respContent.ElapsedMs,
                        Module = "SpreadsheetApi",
                        Message = callingMethod + message
                    });

                return respContent.ApiResponse;
            }
            catch (Exception e)
            {
                Event(e,
                    new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumLogLevel.Error,
                        Module = "SpreadsheetApi",
                        Message = $"{callingMethod} Failed - {e.Message}"
                    });
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }
    }
}
