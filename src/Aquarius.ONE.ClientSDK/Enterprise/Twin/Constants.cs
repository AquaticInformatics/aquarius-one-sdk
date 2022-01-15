using Enterprise.Twin.Protobuf.Models;

namespace ONE.Enterprise.Twin
{
    public static class Constants
    {
        public static class OrganizationCategory
        {
            public const int Id = 1;

            public static class TenantType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "0a1f9e35-fb6c-4976-9768-aeac93c84b96";
                public static class DemoTenantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "10f3b028-12ab-4947-bcb0-b0f612efdaad";
                }
                public static class TestTenantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "6b9dc107-7dbe-44b4-b95f-aa99d0882cfa";
                }
                public static class CustomerTenantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "3c30f0fc-f780-482d-8b9a-671166bc11ca";
                }
                public static class PrivateWaterCompanyTenantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "bcc753cd-369d-4f6a-8661-252b8063fadd";
                }
                public static class RegionTenantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "53b82041-6dd5-4dc7-9c02-e9c76ab430aa";
                }
                public static class RemoteSupportTenantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "b25542d2-4e47-4646-a174-84dcc3d09b56";
                }
                public static class TrojanTenantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "bb335a49-d027-4208-9bd9-7b4529b60f25";
                }
            }
            public static class UserType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "fbd28ae8-8c21-4187-b558-f027612d2e46";

            }

        }
        public static class SpaceCategory
        {
            public const int Id = 2;

            public static class OperationType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "e5d5ea04-64da-49c7-8887-7c6039ba239b";
                public static class WasterWaterTreatmentPlantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "5adb3a4c-0b8b-43a8-8e68-e2de5f5a5430";
                }
                public static class SurfaceWaterTreatmentPlantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "307f753e-a67c-480a-976b-7b0eba270e27";
                }
                public static class DrinkingWaterTreatmentPlantSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "36f8befe-1a75-4c0d-a618-e4885ca6beb2";
                }
            }
            public static class LocationType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "07bc39dc-9c17-494a-8873-65a85f380269";
                public static class OtherSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "3d8cbb58-4af8-4986-83ab-c7cdd70ec895";
                }
            }
            public static class EnvironmentalMonitoringLocationType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "94c566f3-3b11-42a7-bf32-48bc05f4c98a";
                public static class LakeSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "72ae7995-3ac3-471d-acb7-bd5fe1a1c959";
                }
                public static class UnknownSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "d4c5662d-0fb7-4103-8c04-68a2d614e43f";
                }
            }
            public static class GeographyLocationType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "ed63ff2c-3f13-4584-8caa-c10f43af528a";
                public static class OceansSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "880ad30e-ac02-4ecb-bcdf-ef9046c8ec2c";
                }
                public static class ContinentSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "c2499cf5-4214-4e25-b033-a505017f345b";
                }
                public static class CountrySubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "bdd0cc6a-6317-43c4-9d34-cc405f0606ce";
                }
                public static class StateSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "64592ee5-88d8-48da-aa93-eecb34ef7a2f";
                }
                public static class CitySubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "efc7ad46-d15e-4daa-a8be-5b31752966e8";
                }
            }
        }
        public static class IntrumentCategory
        {
            public const int Id = 3;
            public static class ClientIngestType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "9c77b2dc-1c6b-4f02-88e1-eac43692d1c0";

                public static class ClientIngestSubType
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "02d6fd74-f610-428f-8dd6-2789960e7fd7";
                }
            }
            public static class ClientIngestAgentType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "3209ac8f-640b-4f68-a74b-08b670187182";
                public static class ClientIngestAgentTest
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "cafd1356-7e06-44d9-9f28-3a897fbc4cf8";
                }
            }

        }
        public static class TelemetryCategory
        {
            public const int Id = 4;

            public static class InstrumentType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "2a471801-f462-4248-8567-b128ae261fe7";
            }
            public static class TimeSeriesType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "76b517dd-4d94-4b9d-8be3-50f8ffaee731";
                public static class FSData
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "15a5be60-ce3b-419d-9827-ba30cfd9b882";
                }
                public static class InstrumentMeasurements
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "c66fa8ca-57e6-48bd-bde7-7244ebff80cb";
                }
                public static class InstrumentLogs
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "c11e4895-1de4-4964-ae56-95aa94ec8f90";
                }
                public static class AquariusSamples
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "829004fc-1093-4278-827d-609614ad72d5";
                }
                public static class WaterTraxSamples
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "32dec483-6cf4-4924-a972-2cf45766e3c1";
                }
            }
            public static class ColumnType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public static class WorksheetDaily
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "18201416-4ebb-4c33-8881-57eace7cb27d";
                }
                public static class WorksheetFourHour
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "b8f79f8e-fa92-4cdc-9b9b-75589095e20a";
                }
                public static class WorksheetHour
                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "858e695a-5bf7-4f4c-96fc-f9959a2ac01b";
                }
                public static class WorksheetFifteenMinute

                {
                    public static bool Is(DigitalTwin digitalTwin)
                    {
                        return digitalTwin.TwinSubTypeId != null && digitalTwin.TwinSubTypeId.ToUpper() == RefId.ToUpper();
                    }
                    public const string RefId = "2501b9d4-979c-49ea-9bbd-7089ab091747";
                }
                public const string RefId = "ae018857-5f27-4fe4-8117-d0cbaecb9c1e";
            }
            public static class EnterpriseType
            {
                public static bool Is(DigitalTwin digitalTwin)
                {
                    return digitalTwin.TwinTypeId != null && digitalTwin.TwinTypeId.ToUpper() == RefId.ToUpper();
                }
                public const string RefId = "a6c5443d-3726-4a6b-9725-2709094eaf5d";
            }
        }
    }
}
