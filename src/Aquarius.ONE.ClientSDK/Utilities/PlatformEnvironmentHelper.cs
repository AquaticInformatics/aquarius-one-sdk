using System;
using System.Collections.Generic;
using System.Linq;

namespace ONE.Utilities
{
    public static class PlatformEnvironmentHelper
    {
        private static List<PlatformEnvironment> _environments;
        public static PlatformEnvironment GetPlatformEnvironment(string name)
        {
            var matches = Environments.Where(p => string.Equals(p.Name, name, StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);
                switch (name.ToUpper())
                {
                    case "INTEGRATION":
                    case "ONEINTEGRATION":
                    case "ONE INTEGRATION":
                    case "AQIINTEGRATION":
                    case "AQI INTEGRATION":
                    case "2":
                        return PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiIntegration);
                    case "AQISTAGE":
                    case "AQI STAGE":
                    case "ONESTAGE":
                    case "ONE STAGE":
                    case "STAGE":
                    case "3":
                        return PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);
                    case "AQIUSPRODUCTION":
                    case "AQI US PRODUCTION":
                    case "ONE US PRODUCTION":
                    case "ONEUSPRODUCTION":
                    case "USPRODUCTION":
                    case "US PRODUCTION":
                    case "PRODUCTION":
                    case "4":
                        return PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiUSProduction);
                    default:
                        return PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);
                }

            }
        }
        public static PlatformEnvironment GetPlatformEnvironment(EnumPlatformEnvironment enumPlatformEnvironment)
        {
            var matches = Environments.Where(p => p.PlatformEnvironmentEnum == enumPlatformEnvironment);
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public static List<PlatformEnvironment> Environments
        {
            get
            {
                if (_environments == null)
                {
                    _environments = new List<PlatformEnvironment>
                    {
                        new PlatformEnvironment
                        {
                            Name = "Local",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.Local,
                            BaseUri = new Uri("http://localhost:1111"),
                            AuthenticationUri = new Uri("https://localhost:8262/"),
                            PoEditorProjectName = "SPRINT_FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "ONE Feature",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiFeature,
                            BaseUri = new Uri("https://api-feature-us.aquaticinformatics.net/"),
                            AuthenticationUri = new Uri("https://api-feature-us.aquaticinformatics.net/"),
                            PoEditorProjectName = "FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "ONE Integration",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiIntegration,
                            BaseUri = new Uri("https://api-integration-us.aquaticinformatics.net/"),
                            AuthenticationUri = new Uri("https://api-integration-us.aquaticinformatics.net/"),
                            PoEditorProjectName = "FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "ONE Stage",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiStage,
                            BaseUri = new Uri("https://api-stage-us.aquaticinformatics.net/"),
                            AuthenticationUri = new Uri("https://api-stage-us.aquaticinformatics.net/"),
                            PoEditorProjectName = "FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "ONE US Production",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiUSProduction,
                            BaseUri = new Uri("https://api-us.aquaticinformatics.net/"),
                            AuthenticationUri = new Uri("https://api-us.aquaticinformatics.net/"),
                            PoEditorProjectName = "FOUNDATION_LIBRARY"

                        }
                    };
                }
                return _environments;
            }
        }


    }
}
