using System;
using System.Net.Http.Headers;
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

namespace ONE.ClientSDK
{
	public class OneApi
	{
		public AuthenticationApi Authentication
		{
			get
			{
				if (_initialized)
					return _authentication;

				InstantiateApis();
				return _authentication;
			}
		}

		public CoreApi Core
		{
			get
			{
				if (_initialized)
					return _core;

				InstantiateApis();
				return _core;
			}
		}

		public ConfigurationApi Configuration
		{
			get
			{
				if (_initialized)
					return _configuration;

				InstantiateApis();
				return _configuration;
			}
		}

		public LogbookApi Logbook
		{
			get
			{
				if (_initialized)
					return _logbook;

				InstantiateApis();
				return _logbook;
			}
		}

		public LibraryApi Library
		{
			get
			{
				if (_initialized)
					return _library;

				InstantiateApis();
				return _library;
			}
		}

		public ScheduleApi Schedule
		{
			get
			{
				if (_initialized)
					return _schedule;

				InstantiateApis();
				return _schedule;
			}
		}

		public ActivityApi Activity
		{
			get
			{
				if (_initialized)
					return _activity;

				InstantiateApis();
				return _activity;
			}
		}

		public DigitalTwinApi DigitalTwin
		{
			get
			{
				if (_initialized)
					return _digitalTwin;

				InstantiateApis();
				return _digitalTwin;
			}
		}

		public NotificationApi Notification
		{
			get
			{
				if (_initialized)
					return _notification;

				InstantiateApis();
				return _notification;
			}
		}

		public SpreadsheetApi Spreadsheet
		{
			get
			{
				if (_initialized)
					return _spreadsheet;

				InstantiateApis();
				return _spreadsheet;
			}
		}

		public DataApi Data
		{
			get
			{
				if (_initialized)
					return _data;

				InstantiateApis();
				return _data;
			}
		}

		public UserHelper UserHelper
		{
			get
			{
				if (_initialized)
					return _userHelper;

				InstantiateApis();
				return _userHelper;
			}
		}

		public CacheHelper CacheHelper
		{
			get
			{
				if (_initialized)
					return _cacheHelper;

				InstantiateApis();
				return _cacheHelper;
			}
		}

		public PoEditorApi PoEditor
		{
			get
			{
				if (_initialized)
					return _poEditor;

				InstantiateApis();
				return _poEditor;
			}
		}

		public ReportApi Report
		{
			get
			{
				if (_initialized)
					return _report;

				InstantiateApis();
				return _report;
			}
		}

		public SampleApi Sample
		{
			get
			{
				if (_initialized)
					return _sample;

				InstantiateApis();
				return _sample;
			}
		}

		public EventLogger Logger { get; set; }

		public OneApi()
		{
			Logger = new EventLogger();
		}

		public OneApi(string environment, string token, DateTime? expiration = null, bool throwApiErrors = false, bool useProtobufModels = true)
		{
			ThrowApiErrors = throwApiErrors;
			UseProtobufModels = useProtobufModels;
			Logger = new EventLogger();
			Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(environment);
			if (expiration == null || expiration == DateTime.MinValue)
				expiration = DateTime.Now.AddHours(12);
			Authentication.Token = new Token();
			if (!string.IsNullOrEmpty(token))
				Authentication.Token.access_token = token;
			Authentication.Token.expires = (DateTime)expiration;
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

		public PlatformEnvironment Environment
		{
			get => _environment;
			set
			{
				if (_environment == value)
					return;

				_environment = value;
				_initialized = false;
			}
		}

		public bool ContinueOnCapturedContext
		{
			get => _continueOnCapturedContext;
			set
			{
				if (_continueOnCapturedContext == value)
					return;

				_continueOnCapturedContext = value;
				_initialized = false;
			}
		}

		public bool LogRestfulCalls
		{
			get => _logRestfulCalls;
			set
			{
				if (_logRestfulCalls == value)
					return;

				_logRestfulCalls = value;
				_initialized = false;
			}
		}

		public bool ThrowApiErrors
		{
			get => _throwApiErrors;
			set
			{
				if (_throwApiErrors == value)
					return;

				_throwApiErrors = value;
				_initialized = false;
			}
		}

		public bool UseProtobufModels
		{
			get => _useProtobufModels;
			set
			{
				if (_useProtobufModels == value)
					return;

				_useProtobufModels = value;
				_initialized = false;
			}
		}

		private AuthenticationApi _authentication;
		private CoreApi _core;
		private ConfigurationApi _configuration;
		private LogbookApi _logbook;
		private LibraryApi _library;
		private ScheduleApi _schedule;
		private ActivityApi _activity;
		private DigitalTwinApi _digitalTwin;
		private NotificationApi _notification;
		private SpreadsheetApi _spreadsheet;
		private DataApi _data;
		private UserHelper _userHelper;
		private CacheHelper _cacheHelper;
		private PoEditorApi _poEditor;
		private ReportApi _report;
		private SampleApi _sample;
		private RestHelper _restHelper;
		private IOneApiHelper _apiHelper;
		private PlatformEnvironment _environment;
		private bool _continueOnCapturedContext;
		private bool _logRestfulCalls;
		private bool _throwApiErrors;
		private bool _useProtobufModels;
		private bool _initialized;

		private void InstantiateApis()
		{
			_authentication = new AuthenticationApi(Environment, ContinueOnCapturedContext, ThrowApiErrors);
			_authentication.Event += Logger.Logger_Event;

			_apiHelper = new OneApiHelper(Authentication, ContinueOnCapturedContext, UseProtobufModels, LogRestfulCalls);
			_apiHelper.Event += Logger.Logger_Event;

			_restHelper = new RestHelper(Authentication, Environment, ContinueOnCapturedContext, LogRestfulCalls, ThrowApiErrors);
			_restHelper.Event += Logger.Logger_Event;

			_core = new CoreApi(_apiHelper, ContinueOnCapturedContext, ThrowApiErrors);
			_core.Event += Logger.Logger_Event;

			_userHelper = new UserHelper(Authentication, Core);
			_cacheHelper = new CacheHelper(this);

			_configuration = new ConfigurationApi(_apiHelper, ContinueOnCapturedContext, ThrowApiErrors);
			_configuration.Event += Logger.Logger_Event;

			// This is a wrapper around the ConfigurationApi no events are logged directly and no other properties are required
			_logbook = new LogbookApi(Configuration);

			_library = new LibraryApi(Environment, ContinueOnCapturedContext, _restHelper, ThrowApiErrors);
			_library.Event += Logger.Logger_Event;

			_schedule = new ScheduleApi(Environment, ContinueOnCapturedContext, _restHelper, ThrowApiErrors);
			_schedule.Event += Logger.Logger_Event;

			_activity = new ActivityApi(_apiHelper, ContinueOnCapturedContext, ThrowApiErrors);
			_activity.Event += Logger.Logger_Event;

			_digitalTwin = new DigitalTwinApi(_apiHelper, ContinueOnCapturedContext, ThrowApiErrors);
			_digitalTwin.Event += Logger.Logger_Event;

			_notification = new NotificationApi(Environment, ContinueOnCapturedContext, _restHelper, ThrowApiErrors);
			_notification.Event += Logger.Logger_Event;

			_report = new ReportApi(Environment, ContinueOnCapturedContext, _restHelper, ThrowApiErrors);
			_report.Event += Logger.Logger_Event;

			_spreadsheet = new SpreadsheetApi(_apiHelper, ContinueOnCapturedContext, ThrowApiErrors);
			_spreadsheet.Event += Logger.Logger_Event;

			_data = new DataApi(_apiHelper, ContinueOnCapturedContext, ThrowApiErrors);
			_data.Event += Logger.Logger_Event;

			_sample = new SampleApi(Environment, ContinueOnCapturedContext, _restHelper, Activity, ThrowApiErrors);
			_sample.Event += Logger.Logger_Event;
			
			_poEditor = new PoEditorApi(Environment, ContinueOnCapturedContext, ThrowApiErrors);
			_poEditor.Event += Logger.Logger_Event;

			_initialized = true;
		}
	}
}
