using System;
using System.Linq;

namespace ONE.ClientSDK.Utilities
{
    [Serializable]
    public sealed class RestApiException : Exception
    {
        private ServiceResponse _serviceResponse = null;
        public RestApiException()
        { }
        public RestApiException(ServiceResponse serviceResponse)
        {
           _serviceResponse = serviceResponse;
            Data.Add("ElapsedMs", serviceResponse.ElapsedMs);

            if (_serviceResponse.ResponseMessage != null)
            {
                Data.Add("ReasonPhrase", _serviceResponse.ResponseMessage.ReasonPhrase);
                Data.Add("StatusCode", _serviceResponse.ResponseMessage.StatusCode);
            }
            else if (_serviceResponse.ApiResponse != null)
            {
                Data.Add("Errors", _serviceResponse.ApiResponse.Errors);
                Data.Add("StatusCode", _serviceResponse.ApiResponse.StatusCode);
            }
        }
        
        public override string Message
        {
            get
            {
                if (_serviceResponse == null)
                    return "unknown HTTP Error";

                if (_serviceResponse?.ResponseMessage != null)
                    return ((int)_serviceResponse.ResponseMessage.StatusCode).ToString();
                if (_serviceResponse?.ApiResponse?.Errors.FirstOrDefault() != null)
                    return _serviceResponse.ApiResponse.Errors.FirstOrDefault()?.ToString();

                return "unknown HTTP Error";
            }
        }
    }
    
}
