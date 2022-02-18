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
                switch (name)
                {
                    case "AqiIntegration":
                    case "2":
                        return PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiIntegration);
                    case "AqiStage":
                    case "3":
                        return PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);
                    case "AqiUSProduction":
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
                            Name = "Sprint",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.Sprint,
                            BaseUri = new Uri("https://api-eu-sprint.hachtest.com/"),
                            AuthenticationUri = new Uri("https://ffaa-eu-sprint.hachtest.com"),
                            PoEditorProjectName = "SPRINT_FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "Integration",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.Integration,
                            BaseUri = new Uri("https://api-eu-integration.hachtest.com/"),
                            AuthenticationUri = new Uri("https://ffaa-eu-integration.hachtest.com"),
                            PoEditorProjectName = "SPRINT_FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "Feature",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.Feature,
                            BaseUri = new Uri("https://api-eu-feature.hachtest.com/"),
                            AuthenticationUri = new Uri("https://ffaa-eu-feature.hachtest.com"),
                            PoEditorProjectName = "SPRINT_FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "US Production",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.USProduction,
                            BaseUri = new Uri("https://api-us.fsn.hach.com/"),
                            AuthenticationUri = new Uri("https://api-us.fsn.hach.com/"),
                            PoEditorProjectName = "FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "EU Production",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.EUProduction,
                            BaseUri = new Uri("https://api-eu.fsn.hach.com/"),
                            AuthenticationUri = new Uri("https://api-eu.fsn.hach.com/"),
                            PoEditorProjectName = "FOUNDATION_LIBRARY"

                        },
                        new PlatformEnvironment
                        {
                            Name = "Stage",
                            PlatformEnvironmentEnum = EnumPlatformEnvironment.Stage,
                            BaseUri = new Uri("https://api-eu-stage.hach-claros.com/"),
                            AuthenticationUri = new Uri("https://api-eu-stage.hach-claros.com/"),
                            PoEditorProjectName = "FOUNDATION_LIBRARY"

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
