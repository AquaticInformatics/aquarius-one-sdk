
using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Utilities
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
        }
        public override string Message
        {
            get
            {
                if (_serviceResponse == null)
                    return "unknown HTTP Error";
                return _serviceResponse.ResponseMessage.ReasonPhrase;
            }
        }
    }
    
}
