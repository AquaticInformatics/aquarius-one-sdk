using CsvHelper;
using CsvHelper.Configuration;
using Enterprise.Twin.Protobuf.Models;
using Newtonsoft.Json;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Twin;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Agents.CSV
{
    public class AgentService : IngestAgent
    {
        public AgentService(ClientSDK clientSDK)
            : base(clientSDK.Authentication, clientSDK.Core, clientSDK.DigitalTwin, clientSDK.Configuration, clientSDK.Data, new DigitalTwin())
        {
            TwinSubTypeId = ONE.Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentCsv.RefId;
        }
        public override Task<bool> InitializeAsync(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, string ingestClientId, string ingestAgentName, string agentSubTypeId)
        {
            // Initialize Telemetry
            Telemetry.Add(Guid.NewGuid().ToString());
            Telemetry.Add(Guid.NewGuid().ToString());

            // Initialize Configururation
            var agentConfiguration = new AgentConfiguration();
            FileConfig file1 = new FileConfig
            {
                FileName = "*.csv",
                HasHeaderRow = true,
                Path = "C:\\CSV\\LIMS",
                DateColumnNumber = 1
            };

            file1.Telemetry.Add(new TelemetryConfig
            {
                Id = Telemetry[0],
                Name = "pH",
                ValueColumnNumber = 5,
                StringValueColumnNumber = 0,
                Filter = "Ph",
                FilterByColumnNumber = 4,
                 PutAllColumnsIntoPropertyBag = true
            }
            );
            file1.Telemetry.Add(new TelemetryConfig
            {
                Id = Telemetry[1],
                Name = "BOD",
                ValueColumnNumber = 5,
                StringValueColumnNumber = 0,
                Filter = "BOD",
                FilterByColumnNumber = 4,
                PutAllColumnsIntoPropertyBag = true
            }
            );
            agentConfiguration.Files.Add(file1);
            agentConfiguration.ProcessedPath = "c:\\Processed\\CSV";

            agentConfiguration.RunFrequency = new TimeSpan(0, 1, 0);
            agentConfiguration.UploadFrequency = new TimeSpan(0, 5, 0);

            Configuration = agentConfiguration;

            return base.InitializeAsync(authentificationApi, coreApi, digitalTwinApi, configurationApi, dataApi, ingestClientId, ingestAgentName, agentSubTypeId);
        }
        public override async Task<bool> RunAsync()
        {
            var success = true;
            DateTime dateTime = DateTime.Now;
            await UpdateTelemetryTwinInfoAsync();
            var agencyConfiguration = ((AgentConfiguration)Configuration);
            if (agencyConfiguration == null)
                return false;
            try
            {
                if (!Directory.Exists(agencyConfiguration.ProcessedPath))
                    Directory.CreateDirectory(agencyConfiguration.ProcessedPath);
            }
            catch (Exception ex)
            {
                Logger.IngestLogData(ex.Message, ex.StackTrace);
                return false;
            }
            foreach (var fileConfig in ((AgentConfiguration)Configuration).Files)
            {
                try
                {
                    if (!Directory.Exists(fileConfig.Path))
                        Directory.CreateDirectory(fileConfig.Path);


                    string[] files = Directory.GetFiles(fileConfig.Path, fileConfig.FileName);

                    foreach (string file in files)
                    {
                        try
                        {
                            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                            {
                                HasHeaderRecord = false,
                            };
                            using (var reader = new StreamReader(file))
                            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                            {
                                // Do any configuration to `CsvReader` before creating CsvDataReader.
                                using (var dr = new CsvDataReader(csv))
                                {
                                    var dt = new DataTable();
                                    dt.Load(dr);
                                    LoadDataFromDatatable(dt, fileConfig);
                                    File.Move(file, Path.Combine(agencyConfiguration.ProcessedPath, Path.GetFileNameWithoutExtension(file) + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-MM.") + Path.GetExtension(file)));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.IngestLogData(ex.Message, ex.StackTrace);
                            success = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.IngestLogData(ex.Message, ex.StackTrace);
                    return false;
                }
            }
            return success;
        }
        private void LoadDataFromDatatable(DataTable dt, FileConfig fileConfig)
        {
            try
            {
                foreach (string telemetryId in Telemetry)
                {
                    var telemetryConfig = fileConfig.GetById(telemetryId);
                    if (telemetryConfig != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (fileConfig.DateColumnNumber <= dt.Columns.Count && 
                                    (   
                                        (telemetryConfig.ValueColumnNumber > 0 && telemetryConfig.ValueColumnNumber <= dt.Columns.Count) || 
                                        (telemetryConfig.StringValueColumnNumber > 0 && telemetryConfig.StringValueColumnNumber <= dt.Columns.Count)
                                    )
                                )
                            {
                                DateTime.TryParse(row[fileConfig.DateColumnNumber - 1].ToString(), out DateTime dateTime);
                                if (dateTime > DateTime.MinValue)
                                {
                                    if (telemetryConfig.FilterByColumnNumber > 0 &&
                                        telemetryConfig.FilterByColumnNumber <= dt.Columns.Count &&
                                        !string.IsNullOrEmpty(telemetryConfig.Filter) &&
                                        telemetryConfig.Filter != row[telemetryConfig.FilterByColumnNumber - 1].ToString())
                                        continue;
                                    double? value = null;
                                    if (telemetryConfig.ValueColumnNumber > 0 && telemetryConfig.ValueColumnNumber <= dt.Columns.Count && row[telemetryConfig.ValueColumnNumber - 1] != null)
                                        if (double.TryParse(row[telemetryConfig.ValueColumnNumber - 1].ToString(), out double dvalue))
                                            value = dvalue;
                                    string? stringValue = "";
                                    if (telemetryConfig.StringValueColumnNumber > 0 && telemetryConfig.StringValueColumnNumber <= dt.Columns.Count)
                                        stringValue = row[telemetryConfig.StringValueColumnNumber - 1].ToString();

                                    IngestData(telemetryConfig.Id, dateTime, value, stringValue);

                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.IngestLogData(ex.Message, ex.StackTrace);
            }
        }
        private async Task<bool> UpdateTelemetryTwinInfoAsync()
        {
            if (TelemetryTwins != null)
            {
                for (int i = 0; i < TelemetryTwins.Count; i++)
                {
                    var telemetryConfig = GetById(TelemetryTwins[i].TwinReferenceId);
                    if (telemetryConfig == null)
                        return false;
                    if (!string.IsNullOrEmpty(telemetryConfig.Name) && telemetryConfig.Name != TelemetryTwins[i].Name)
                        await UpdateTelemetryTwinName(TelemetryTwins[i], telemetryConfig.Name);
                }
            }
            return true;
        }
        public TelemetryConfig? GetById(string Id)
        {
            AgentConfiguration configuration = (AgentConfiguration)Configuration;
            foreach (var file in configuration.Files)
            {
                var matches = file.Telemetry.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), Id.ToUpper(), StringComparison.CurrentCulture));
                if (matches.Count() > 0)
                    return matches.First();
            }
            return null;
        }
        public override bool LoadConfiguration(string json)
        {
            try
            {
                var configuration = JsonConvert.DeserializeObject<AgentConfiguration>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                if (configuration == null)
                    return false;
                configuration.Files = configuration.Files;
                configuration.RunFrequency = configuration.RunFrequency;
                configuration.UploadFrequency = configuration.UploadFrequency;
                Configuration = configuration;

            }
            catch
            {
                return false;
            }
            return true;
        }
    }

}
