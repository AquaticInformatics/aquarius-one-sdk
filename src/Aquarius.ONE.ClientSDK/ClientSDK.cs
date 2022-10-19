using ONE.Common.Activity;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Common.Library;
using ONE.Common.Logbook;
using ONE.Common.Notification;
using ONE.Common.Schedule;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Report;
using ONE.Enterprise.Twin;
using ONE.Operations.Sample;
using ONE.Operations.Spreadsheet;
using ONE.PoEditor;
using ONE.Utilities;
using System;
using System.Net.Http.Headers;


namespace ONE
{
    public class ClientSDK
    {
        public AuthenticationApi Authentication { get; set; }
        public CoreApi Core { get; set; }
        public ConfigurationApi Configuration { get; set; }
        public LogbookApi Logbook { get; set; }
        public LibraryApi Library { get; set; }
        public ScheduleApi Schedule { get; set; }
        public ActivityApi Activity { get; set; }
        public DigitalTwinApi DigitalTwin { get; set; }
        public NotificationApi Notification { get; set; }
        public SpreadsheetApi Spreadsheet { get; set; }
        public DataApi Data { get; set; }
        public UserHelper UserHelper { get; set; }
        public CacheHelper CacheHelper { get; set; }
        public PoEditorApi PoEditor { get; set; }
        public ReportApi Report { get; set; }
        public SampleApi Sample { get; set; }
        private RestHelper _restHelper { get; set; }
        public EventLogger Logger { get; set; }

        public ClientSDK()
        {
            Logger = new EventLogger();
            InstantiateAPIs();
        }
        public ClientSDK(string environment, string token, DateTime? expiration = null, bool throwApiErrors = false)
        {
            ThrowAPIErrors = throwApiErrors;
            Logger = new EventLogger();
            InstantiateAPIs();
            Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(environment);
            if (expiration == null || expiration == DateTime.MinValue)
                expiration = DateTime.Now.AddHours(12);
            Authentication.Token = new Token();
            if (!string.IsNullOrEmpty(token))
                Authentication.Token.access_token = token;
            Authentication.Token.expires = (DateTime)expiration;
            Authentication.HttpJsonClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", token);
            Authentication.HttpProtocolBufferClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", token);
        }
        public ClientSDK(string environment, bool throwApiErrors = false)
        {
            ThrowAPIErrors = throwApiErrors;
            Logger = new EventLogger();
            InstantiateAPIs();
            Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(environment);
        }
        public ClientSDK(EnumPlatformEnvironment platformEnvironment, bool throwApiErrors = false)
        {
            ThrowAPIErrors = throwApiErrors;
            Logger = new EventLogger();
            InstantiateAPIs();
            Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(platformEnvironment);
        }
        private PlatformEnvironment _environment { get; set; }
        public PlatformEnvironment Environment
        {
            get
            {
                return _environment;
            }
            set
            {
                if (Authentication != null)
                    Authentication.Logout();
                _environment = value;
                InstantiateAPIs();
            }
        }
        private void InstantiateAPIs()
        {

            Authentication = new AuthenticationApi(_environment, ContinueOnCapturedContext);
            Authentication.Event += Logger.Logger_Event;

            _restHelper = new RestHelper(Authentication, Environment, ContinueOnCapturedContext, LogRestfulCalls, ThrowAPIErrors);
            _restHelper.Event += Logger.Logger_Event;

            Core = new CoreApi(Environment, ContinueOnCapturedContext, _restHelper);
            Core.Event += Logger.Logger_Event;

            UserHelper = new UserHelper(Authentication, Core);
            CacheHelper = new CacheHelper(this);

            Configuration = new ConfigurationApi(Environment, ContinueOnCapturedContext, _restHelper);
            Configuration.Event += Logger.Logger_Event;

            // This is a wrapper around the ConfigurationApi no events are logged directly
            Logbook = new LogbookApi(Environment, ContinueOnCapturedContext, _restHelper);

            Library = new LibraryApi(Environment, ContinueOnCapturedContext, _restHelper);
            Library.Event += Logger.Logger_Event;

            Schedule = new ScheduleApi(Environment, ContinueOnCapturedContext, _restHelper);
            Schedule.Event += Logger.Logger_Event;

            Activity = new ActivityApi(Environment, ContinueOnCapturedContext, _restHelper);
            Activity.Event += Logger.Logger_Event;

            DigitalTwin = new DigitalTwinApi(Environment, ContinueOnCapturedContext, _restHelper);
            DigitalTwin.Event += Logger.Logger_Event;

            Notification = new NotificationApi(Environment, ContinueOnCapturedContext, _restHelper);
            Notification.Event += Logger.Logger_Event;

            Report = new ReportApi(Environment, ContinueOnCapturedContext, _restHelper);
            Report.Event += Logger.Logger_Event;

            Spreadsheet = new SpreadsheetApi(Environment, ContinueOnCapturedContext, _restHelper);
            Spreadsheet.Event += Logger.Logger_Event;

            Data = new DataApi(Environment, ContinueOnCapturedContext, _restHelper);
            Data.Event += Logger.Logger_Event;

            Sample = new SampleApi(Environment, ContinueOnCapturedContext, _restHelper, this);
            Data.Event += Logger.Logger_Event;
            
            PoEditorApi poEditor = new PoEditorApi(Environment, ContinueOnCapturedContext);
            poEditor.Event += Logger.Logger_Event;

        }
        public bool ContinueOnCapturedContext { get; set; }

        public bool LogRestfulCalls { get; set; }

        public bool ThrowAPIErrors { get; set; }
    }
}
