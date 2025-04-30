using System;

namespace ONE.ClientSDK.Utilities
{
    [Serializable]
    public class RestApiException : Exception
    {
        private ServiceResponse _serviceResponse = null;
        public RestApiException()
        { }
        public RestApiException(ServiceResponse serviceResponse)
        {
           _serviceResponse = serviceResponse;
            Data.Add("ElapsedMs", serviceResponse.ElapsedMs);
            Data.Add("ReasonPhrase", _serviceResponse.ResponseMessage.ReasonPhrase);
            Data.Add("StatusCode", _serviceResponse.ResponseMessage.StatusCode);
        }
        
        public override string Message
        {
            get
            {
                if (_serviceResponse == null)
                    return "unknown HTTP Error";
                return ((int)_serviceResponse.ResponseMessage.StatusCode).ToString();
            }
        }
    }
    
}
