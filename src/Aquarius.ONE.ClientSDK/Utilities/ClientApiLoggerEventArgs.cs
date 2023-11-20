using ONE.Models.CSharp;
using System.Net;

namespace ONE.Utilities
{
    public class ClientApiLoggerEventArgs
    {
        public string Module;
        public string Message;
        public EnumOneLogLevel EventLevel;
        public long ElapsedMs;
        public HttpStatusCode HttpStatusCode;
        public string File;
        public string RequestId;
    }
}
