using ONE.Common.Configuration;
using ONE.Common.Library;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Twin;
using ONE.Operations.Spreadsheet;
using ONE.PoEditor;
using ONE.Utilities;

namespace ONE
{
    public class ClientSDK
    {
        public AuthenticationApi Authentication { get; set; }
        public CoreApi Core { get; set; }
        public ConfigurationApi Configuration { get; set; }
        public LibraryApi Library { get; set; }
        public DigitalTwinApi DigitalTwin { get; set; }
        public SpreadsheetApi Spreadsheet { get; set; }
        public DataApi Data { get; set; }
        public UserHelper UserHelper { get; set; }
        public CacheHelper CacheHelper { get; set; }
        public PoEditorApi PoEditor { get; set; }

        private RestHelper _restHelper { get; set; }
        public EventLogger Logger { get; set; }

        public ClientSDK()
        {
            Logger = new EventLogger();

            InstantiateAPIs();
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

            _restHelper = new RestHelper(Authentication, Environment, ContinueOnCapturedContext, LogRestfulCalls);
            _restHelper.Event += Logger.Logger_Event;

            Core = new CoreApi(Environment, ContinueOnCapturedContext, _restHelper);
            Core.Event += Logger.Logger_Event;

            UserHelper = new UserHelper(Core);
            CacheHelper = new CacheHelper(this);

            Configuration = new ConfigurationApi(Environment, ContinueOnCapturedContext, _restHelper);
            Configuration.Event += Logger.Logger_Event;

            Library = new LibraryApi(Environment, ContinueOnCapturedContext, _restHelper);
            Library.Event += Logger.Logger_Event;

            DigitalTwin = new DigitalTwinApi(Environment, ContinueOnCapturedContext, _restHelper);
            DigitalTwin.Event += Logger.Logger_Event;

            Spreadsheet = new SpreadsheetApi(Environment, ContinueOnCapturedContext, _restHelper);
            Spreadsheet.Event += Logger.Logger_Event;

            Data = new DataApi(Environment, ContinueOnCapturedContext, _restHelper);
            Data.Event += Logger.Logger_Event;

            PoEditorApi poEditor = new PoEditorApi(Environment, ContinueOnCapturedContext);
            poEditor.Event += Logger.Logger_Event;
        }
        public bool ContinueOnCapturedContext { get; set; }

        public bool LogRestfulCalls { get; set; }
    }
}
