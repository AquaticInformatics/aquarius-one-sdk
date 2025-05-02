using System;
using System.Collections.Generic;
using System.Linq;
using ONE.ClientSDK.Enums;

namespace ONE.ClientSDK.Utilities
{
	public static class PlatformEnvironmentHelper
	{
		private static List<PlatformEnvironment> _environments;
		public static PlatformEnvironment GetPlatformEnvironment(string name)
		{
			var match = Environments.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase));

			if (match != null)
				return match;

			if (string.IsNullOrEmpty(name))
				return GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);

			switch (name.ToUpper())
			{
				case "INTEGRATION":
				case "ONEINTEGRATION":
				case "ONE INTEGRATION":
				case "AQIINTEGRATION":
				case "AQI INTEGRATION":
				case "2":
					return GetPlatformEnvironment(EnumPlatformEnvironment.AqiIntegration);
				case "AQISTAGE":
				case "AQI STAGE":
				case "ONESTAGE":
				case "ONE STAGE":
				case "STAGE":
				case "3":
					return GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);
				case "AQIUSPRODUCTION":
				case "AQI US PRODUCTION":
				case "ONE US PRODUCTION":
				case "ONEUSPRODUCTION":
				case "USPRODUCTION":
				case "US PRODUCTION":
				case "PRODUCTION":
				case "4":
					return GetPlatformEnvironment(EnumPlatformEnvironment.AqiUSProduction);
				case "AQIEUPRODUCTION":
				case "AQI EU PRODUCTION":
				case "ONE EU PRODUCTION":
				case "ONEEUPRODUCTION":
				case "EUPRODUCTION":
				case "EU PRODUCTION":
				case "EU-PRODUCTION":
				case "EU":
				case "5":
					return GetPlatformEnvironment(EnumPlatformEnvironment.AqiEUProduction);
				case "AQIAUPRODUCTION":
				case "AQI AU PRODUCTION":
				case "ONE AU PRODUCTION":
				case "ONEAUPRODUCTION":
				case "AUPRODUCTION":
				case "AU PRODUCTION":
				case "AU-PRODUCTION":
				case "AU":
				case "6":
					return GetPlatformEnvironment(EnumPlatformEnvironment.AqiAUProduction);
				default:
					return GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);
			}
		}

		public static PlatformEnvironment GetPlatformEnvironment(EnumPlatformEnvironment enumPlatformEnvironment) =>
			Environments.FirstOrDefault(p => p.PlatformEnvironmentEnum == enumPlatformEnvironment);

		public static List<PlatformEnvironment> Environments =>
			_environments ?? (_environments = new List<PlatformEnvironment>
			{
				new PlatformEnvironment
				{
					Name = "Local",
					PlatformEnvironmentEnum = EnumPlatformEnvironment.Local,
					BaseUri = new Uri("http://localhost:8262"),
					AuthenticationUri = new Uri("https://localhost:8262/"),
					PoEditorProjectName = "AQI_FOUNDATION_LIBRARY"
				},
				new PlatformEnvironment
				{
					Name = "ONE Feature",
					PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiFeature,
					BaseUri = new Uri("https://api-feature-us.aquaticinformatics.net/"),
					AuthenticationUri = new Uri("https://api-feature-us.aquaticinformatics.net/"),
					PoEditorProjectName = "AQI_FOUNDATION_LIBRARY"
				},
				new PlatformEnvironment
				{
					Name = "ONE Integration",
					PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiIntegration,
					BaseUri = new Uri("https://api-integration-us.aquaticinformatics.net/"),
					AuthenticationUri = new Uri("https://api-integration-us.aquaticinformatics.net/"),
					PoEditorProjectName = "AQI_FOUNDATION_LIBRARY"
				},
				new PlatformEnvironment
				{
					Name = "ONE Stage",
					PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiStage,
					BaseUri = new Uri("https://api-stage-us.aquaticinformatics.net/"),
					AuthenticationUri = new Uri("https://api-stage-us.aquaticinformatics.net/"),
					PoEditorProjectName = "AQI_FOUNDATION_LIBRARY"
				},
				new PlatformEnvironment
				{
					Name = "ONE US Production",
					PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiUSProduction,
					BaseUri = new Uri("https://api-us.aquaticinformatics.net/"),
					AuthenticationUri = new Uri("https://api-us.aquaticinformatics.net/"),
					PoEditorProjectName = "AQI_FOUNDATION_LIBRARY"
				},
				new PlatformEnvironment
				{
					Name = "ONE EU Production",
					PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiEUProduction,
					BaseUri = new Uri("https://api-eu.aquaticinformatics.net/"),
					AuthenticationUri = new Uri("https://api-eu.aquaticinformatics.net/"),
					PoEditorProjectName = "AQI_FOUNDATION_LIBRARY"
				},
				new PlatformEnvironment
				{
					Name = "ONE AU Production",
					PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiAUProduction,
					BaseUri = new Uri("https://api-au.aquaticinformatics.net/"),
					AuthenticationUri = new Uri("https://api-au.aquaticinformatics.net/"),
					PoEditorProjectName = "AQI_FOUNDATION_LIBRARY"
				}
			});
	}
}
