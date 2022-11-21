using ONE.Models.CSharp.Enums;
using System.Net;

namespace ONE.Utilities
{
    public class ClientApiLoggerEventArgs
    {
        public string Module;
        public string Message;
        public EnumLogLevel EventLevel;
        public long ElapsedMs;
        public HttpStatusCode HttpStatusCode;
        public string File;
        public string RequestId;
    }
}
