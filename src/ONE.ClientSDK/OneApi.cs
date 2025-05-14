using ONE.ClientSDK.Common.Activity;
using ONE.ClientSDK.Common.Configuration;
using ONE.ClientSDK.Common.Library;
using ONE.ClientSDK.Common.Logbook;
using ONE.ClientSDK.Common.Notifications;
using ONE.ClientSDK.Common.Schedule;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.ClientSDK.Enterprise.Core;
using ONE.ClientSDK.Enterprise.Report;
using ONE.ClientSDK.Enterprise.Twin;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Historian.Data;
using ONE.ClientSDK.Operations.Sample;
using ONE.ClientSDK.Operations.Spreadsheet;
using ONE.ClientSDK.PoEditor;
using ONE.ClientSDK.Utilities;
using System;
using System.Net.Http.Headers;

namespace ONE.ClientSDK
{
	public class OneApi
	{
		private AuthenticationApi _authentication;
		public AuthenticationApi Authentication
		{
			get
			{
				if (_authentication != null)
					return _authentication;

				_authentication = new AuthenticationApi(Environment, ContinueOnCapturedContext, ThrowApiErrors);
				_authentication.Event += Logger.Logger_Event;

				// Clear dependent backing fields
				_apiHelper = null;
				_userHelper = null;

				return _authentication;
			}
		}

		private CoreApi _core;
		public CoreApi Core
		{
			get
			{
				if (_core != null)
					return _core;

				_core = new CoreApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_core.Event += Logger.Logger_Event;

				// Clear dependent backing fields
				_userHelper = null;

				return _core;
			}
		}

		private ConfigurationApi _configuration;
		public ConfigurationApi Configuration
		{
			get
			{
				if (_configuration != null)
					return _configuration;

				_configuration = new ConfigurationApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_configuration.Event += Logger.Logger_Event;

				// Clear dependent backing fields
				_logbook = null;

				return _configuration;
			}
		}

		private LogbookApi _logbook;
		public LogbookApi Logbook
		{
			get
			{
				if (_logbook != null)
					return _logbook;

				_logbook = new LogbookApi(Configuration);

				return _logbook;
			}
		}

		private LibraryApi _library;
		public LibraryApi Library
		{
			get
			{
				if (_library != null)
					return _library;

				_library = new LibraryApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_library.Event += Logger.Logger_Event;

				return _library;
			}
		}

		private ScheduleApi _schedule;
		public ScheduleApi Schedule
		{
			get
			{
				if (_schedule != null)
					return _schedule;

				_schedule = new ScheduleApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_schedule.Event += Logger.Logger_Event;

				return _schedule;
			}
		}

		private ActivityApi _activity;
		public ActivityApi Activity
		{
			get
			{
				if (_activity != null)
					return _activity;

				_activity = new ActivityApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_activity.Event += Logger.Logger_Event;

				// Clear dependent backing fields
				_sample = null;

				return _activity;
			}
		}

		private DigitalTwinApi _digitalTwin;
		public DigitalTwinApi DigitalTwin
		{
			get
			{
				if (_digitalTwin != null)
					return _digitalTwin;

				_digitalTwin = new DigitalTwinApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_digitalTwin.Event += Logger.Logger_Event;

				return _digitalTwin;
			}
		}

		private NotificationApi _notification;
		public NotificationApi Notification
		{
			get
			{
				if (_notification != null)
					return _notification;

				_notification = new NotificationApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_notification.Event += Logger.Logger_Event;

				return _notification;
			}
		}

		private SpreadsheetApi _spreadsheet;
		public SpreadsheetApi Spreadsheet
		{
			get
			{
				if (_spreadsheet != null)
					return _spreadsheet;

				_spreadsheet = new SpreadsheetApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_spreadsheet.Event += Logger.Logger_Event;

				return _spreadsheet;
			}
		}

		private DataApi _data;
		public DataApi Data
		{
			get
			{
				if (_data != null)
					return _data;

				_data = new DataApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_data.Event += Logger.Logger_Event;

				return _data;
			}
		}

		private UserHelper _userHelper;
		public UserHelper UserHelper
		{
			get
			{
				if (_userHelper != null)
					return _userHelper;

				_userHelper = new UserHelper(Authentication, Core);

				return _userHelper;
			}
		}

		private CacheHelper _cacheHelper;
		public CacheHelper CacheHelper
		{
			get
			{
				if (_cacheHelper != null)
					return _cacheHelper;

				_cacheHelper = new CacheHelper(this);

				return _cacheHelper;
			}
		}

		private PoEditorApi _poEditor;
		public PoEditorApi PoEditor
		{
			get
			{
				if (_poEditor != null)
					return _poEditor;

				_poEditor = new PoEditorApi(ThrowApiErrors);
				_poEditor.Event += Logger.Logger_Event;

				return _poEditor;
			}
		}

		private ReportApi _report;
		public ReportApi Report
		{
			get
			{
				if (_report != null)
					return _report;

				_report = new ReportApi(ApiHelper, ContinueOnCapturedContext, ThrowApiErrors);
				_report.Event += Logger.Logger_Event;

				return _report;
			}
		}

		private SampleApi _sample;
		public SampleApi Sample
		{
			get
			{
				if (_sample != null)
					return _sample;

				_sample = new SampleApi(ApiHelper, Activity, ContinueOnCapturedContext, ThrowApiErrors);
				_sample.Event += Logger.Logger_Event;

				return _sample;
			}
		}

		public EventLogger Logger { get; set; }

		public OneApi()
		{
			Logger = new EventLogger();
			UseProtobufModels = true;
		}

		public OneApi(string environment, string token, DateTime? expiration = null, bool throwApiErrors = false, bool useProtobufModels = true)
		{
			ThrowApiErrors = throwApiErrors;
			UseProtobufModels = useProtobufModels;
			Logger = new EventLogger();
			Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(environment);
			
			if (expiration == null || expiration == DateTime.MinValue)
				expiration = DateTime.Now.AddHours(12);
			
			if (!string.IsNullOrEmpty(token))
				Authentication.Token = new Token{access_token = token, expires = expiration.Value};
			
			Authentication.HttpJsonClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			Authentication.HttpProtocolBufferClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		public OneApi(string environment, bool throwApiErrors = false, bool useProtobufModels = true)
		{
			ThrowApiErrors = throwApiErrors;
			UseProtobufModels = useProtobufModels;
			Logger = new EventLogger();
			Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(environment);
		}

		public OneApi(EnumPlatformEnvironment platformEnvironment, bool throwApiErrors = false, bool useProtobufModels = true)
		{
			ThrowApiErrors = throwApiErrors;
			UseProtobufModels = useProtobufModels;
			Logger = new EventLogger();
			Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(platformEnvironment);
		}

		public OneApi(bool throwApiErrors, bool useProtobufModels, bool continueOnCapturedContext, bool logRestfulCalls)
		{
			ThrowApiErrors = throwApiErrors;
			UseProtobufModels = useProtobufModels;
			ContinueOnCapturedContext = continueOnCapturedContext;
			LogRestfulCalls = logRestfulCalls;
			Logger = new EventLogger();
		}

		private PlatformEnvironment _environment;
		public PlatformEnvironment Environment
		{
			get => _environment;
			set
			{
				if (_environment == value)
					return;

				_environment = value;

				// Clear dependent backing fields
				_authentication = null;
			}
		}

		private bool _continueOnCapturedContext;
		public bool ContinueOnCapturedContext
		{
			get => _continueOnCapturedContext;
			set
			{
				if (_continueOnCapturedContext == value)
					return;

				_continueOnCapturedContext = value;

				// Clear dependent backing fields
				_authentication = null;
				_apiHelper = null;
				_core = null;
				_configuration = null;
				_library = null;
				_schedule = null;
				_activity = null;
				_digitalTwin = null;
				_notification = null;
				_report = null;
				_spreadsheet = null;
				_data = null;
				_sample = null;
			}
		}

		private bool _logRestfulCalls;
		public bool LogRestfulCalls
		{
			get => _logRestfulCalls;
			set
			{
				if (_logRestfulCalls == value)
					return;

				_logRestfulCalls = value;

				// Clear dependent backing fields
				_apiHelper = null;
			}
		}

		private bool _throwApiErrors;
		public bool ThrowApiErrors
		{
			get => _throwApiErrors;
			set
			{
				if (_throwApiErrors == value)
					return;

				_throwApiErrors = value;

				// Clear dependent backing fields
				_authentication = null;
				_core = null;
				_configuration = null;
				_library = null;
				_schedule = null;
				_activity = null;
				_digitalTwin = null;
				_notification = null;
				_report = null;
				_spreadsheet = null;
				_data = null;
				_sample = null;
				_poEditor = null;
			}
		}

		private bool _useProtobufModels;
		public bool UseProtobufModels
		{
			get => _useProtobufModels;
			set
			{
				if (_useProtobufModels == value)
					return;

				_useProtobufModels = value;

				// Clear dependent backing fields
				_apiHelper = null;
			}
		}

		private IOneApiHelper _apiHelper;
		private IOneApiHelper ApiHelper
		{
			get
			{
				if (_apiHelper != null)
					return _apiHelper;

				_apiHelper = new OneApiHelper(Authentication, ContinueOnCapturedContext, UseProtobufModels, LogRestfulCalls);
				_apiHelper.Event += Logger.Logger_Event;

				// Clear dependent backing fields
				_core = null;
				_configuration = null;
				_library = null;
				_schedule = null;
				_activity = null;
				_digitalTwin = null;
				_notification = null;
				_report = null;
				_spreadsheet = null;
				_data = null;
				_sample = null;

				return _apiHelper;
			}
		}
	}
}
