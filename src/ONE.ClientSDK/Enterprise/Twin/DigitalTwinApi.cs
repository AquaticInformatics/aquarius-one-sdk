using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Constants.TwinCategory;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Enterprise.Twin
{
    public class DigitalTwinApi
    {
        private readonly IOneApiHelper _apiHelper;
        private readonly bool _continueOnCapturedContext;
        private readonly bool _throwApiErrors;

        private const string EndpointRoot = "enterprise/twin/v1";
        
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public DigitalTwinApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
        {
            _apiHelper = apiHelper;
            _continueOnCapturedContext = continueOnCapturedContext;
            _throwApiErrors = throwApiErrors;
        }
        
        /********************* DigitalTwinTypes *********************/
        public async Task<DigitalTwinType> CreateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinType?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("CreateDigitalTwinTypeAsync", HttpMethod.Post, endpoint,cancellation, digitalTwinType).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwinTypes?.Items.FirstOrDefault();
        }

        public async Task<DigitalTwinType> UpdateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinType/{digitalTwinType.Id}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("UpdateDigitalTwinTypeAsync", HttpMethod.Put, endpoint, cancellation, digitalTwinType).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwinTypes?.Items.FirstOrDefault();
        }

        public async Task<bool> DeleteDigitalTwinTypeAsync(string id, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinType/{id}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("DeleteDigitalTwinTypeAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<List<DigitalTwinType>> GetDigitalTwinTypesAsync(CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinType?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteRequest("GetDigitalTwinTypesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwinTypes?.Items.ToList();
        }

        /********************* DigitalTwinSubTypes *********************/
        public async Task<DigitalTwinSubtype> CreateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinSubType?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteRequest("CreateDigitalTwinSubTypeAsync", HttpMethod.Post, endpoint, cancellation, digitalTwinSubType).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwinSubtypes?.Items.FirstOrDefault();
        }

        public async Task<DigitalTwinSubtype> UpdateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinSubType/{digitalTwinSubType.Id}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("UpdateDigitalTwinSubTypeAsync", HttpMethod.Put, endpoint, CancellationToken.None, digitalTwinSubType).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwinSubtypes?.Items.FirstOrDefault();
        }

        public async Task<bool> DeleteDigitalTwinSubTypeAsync(string id, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinSubType/{id}?requestId={Guid.NewGuid()}";
            
            var apiResponse = await ExecuteRequest("DeleteDigitalTwinSubTypeAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<List<DigitalTwinSubtype>> GetDigitalTwinSubTypesAsync(CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwinSubType?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("GetDigitalTwinSubTypesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwinSubtypes?.Items.ToList();
        }


        /********************* DigitalTwins *********************/

        public async Task<DigitalTwin> CreateSpaceAsync(string parentId, string name, string twinTypeId = SpaceConstants.LocationType.RefId, string twinSubTypeId = SpaceConstants.LocationType.OtherSubType.RefId, CancellationToken cancellation = default)
        {
            var digitalTwin = new DigitalTwin
            {
                ParentTwinReferenceId = parentId,
                Name = name,
                CategoryId = 2,
                TwinReferenceId = Guid.NewGuid().ToString(),
                TwinTypeId = twinTypeId,
                TwinSubTypeId = twinSubTypeId
            };
            return await CreateAsync(digitalTwin, cancellation);
        }

        public async Task<DigitalTwin> CreateAsync(DigitalTwin digitalTwin, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("CreateAsync", HttpMethod.Post, endpoint, cancellation, digitalTwin).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwins?.Items.FirstOrDefault();
        }

        public async Task<List<DigitalTwin>> CreateManyAsync(List<DigitalTwin> digitalTwins, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/many?requestId={Guid.NewGuid()}";

            var twins = new DigitalTwins();
            twins.Items.AddRange(digitalTwins);

            var apiResponse = await ExecuteRequest("CreateManyAsync", HttpMethod.Post, endpoint, cancellation, twins).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwins?.Items.ToList();
        }

        public async Task<DigitalTwin> UpdateAsync(DigitalTwin digitalTwin, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("UpdateAsync", new HttpMethod("PATCH"), endpoint, cancellation, digitalTwin).ConfigureAwait(_continueOnCapturedContext);

            if (apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode())
                return digitalTwin;

            return null;
        }

        public async Task<bool> MoveAsync(string twinRefId, string parentRefId, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/twinRef/{twinRefId}/parentRef/{parentRefId}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("MoveAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<bool> MoveAsync(long id, long parentId, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/{id}/parent/{parentId}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("MoveAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<DigitalTwin> UpdateTwinDataAsync(string twinReferenceId, JsonPatchDocument twinData, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/twinRefId/{twinReferenceId}/UpdateTwinData?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("UpdateTwinDataAsync", new HttpMethod("PATCH"), endpoint, cancellation, twinData).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwins?.Items.FirstOrDefault();
        }

        public async Task<bool> UpdateTwinDataManyAsync(Dictionary<string, JsonPatchDocument> twinDataMany, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/UpdateTwinData/many?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("UpdateTwinDataManyAsync", new HttpMethod("PATCH"), endpoint, cancellation, twinDataMany).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<bool> DeleteAsync(long twinId, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/{twinId}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("DeleteAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<bool> DeleteTreeAsync(string id, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/{id}/tree?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("DeleteTreeAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
        }

        public async Task<DigitalTwin> GetAsync(string twinRefId, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/Ref/{twinRefId}?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("GetAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwins?.Items.FirstOrDefault();
        }

        public async Task<List<DigitalTwin>> GetDescendantsByTypeAsync(string twinRefId, string twinTypeId, CancellationToken cancellation = default)
            => await GetDescendantsAsync(twinRefId, twinTypeId, cancellation);

        public async Task<List<DigitalTwin>> GetDescendantsAsync(string twinRefId, string twinTypeId, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/Type/{twinTypeId}/Descendants?requestId={Guid.NewGuid()}";

            if (string.IsNullOrEmpty(twinTypeId))
                endpoint = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/Descendants?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("GetDescendantsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwins?.Items.ToList();
        }

        public async Task<List<DigitalTwin>> GetDescendantsBySubTypeAsync(string twinRefId, string twinSubTypeId, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/SubType/{twinSubTypeId}/Descendants?requestId={Guid.NewGuid()}";

            if (string.IsNullOrEmpty(twinSubTypeId))
                endpoint = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/Descendants?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("GetDescendantsBySubTypeAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwins?.Items.ToList();
        }

        public async Task<List<DigitalTwin>> GetDescendantsByCategoryAsync(string twinRefId, uint categoryId, CancellationToken cancellation = default)
        {
            var endpoint = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/Category/{categoryId}/Descendants?requestId={Guid.NewGuid()}";

            var apiResponse = await ExecuteRequest("GetDescendantsByCategoryAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

            return apiResponse?.Content?.DigitalTwins?.Items.ToList();
        }

        private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
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
                        Module = "DigitalTwinApi",
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
                        Module = "DigitalTwinApi",
                        Message = $"{callingMethod} Failed - {e.Message}"
                    });
                if (_throwApiErrors)
                    throw;
                return null;
            }
        }
    }
}
