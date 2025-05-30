using ONE.ClientSDK.Enums;
using System;

namespace ONE.ClientSDK.Utilities
{
	public class PlatformEnvironment
	{
		public EnumPlatformEnvironment PlatformEnvironmentEnum { get; set; }
		public string Name { get; set; }
		public Uri BaseUri { get; set; }
		public Uri AuthenticationUri { get; set; }
		public string PoEditorProjectName { get; set; }
	}
}
