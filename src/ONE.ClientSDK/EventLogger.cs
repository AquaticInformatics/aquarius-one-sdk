using System;
using ONE.ClientSDK.Utilities;

namespace ONE.ClientSDK
{
    public class EventLogger
    {
        public delegate void NotificationEventHandler(object sender, ClientApiLoggerEventArgs args);

        public event EventHandler<ClientApiLoggerEventArgs> Event;
        public void Logger_Event(object sender, ClientApiLoggerEventArgs e)
        {
            if (Event != null)
                Event(sender, e);
        }
    }
}
