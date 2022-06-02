using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONE
{
    public static class ExceptionHelper
    {
        public static Exception GetException(ServiceResponse serviceResponse)
        {
            switch (serviceResponse.ApiResponse.StatusCode)
            {
                default:
                    return new Exception();
            }
            
        }
    }
}
