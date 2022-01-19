using ONE.Enterprise.Authentication;
using ONE.Enterprise.Twin;
using ONE.Common.Historian;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Enterprise.Twin.Protobuf.Models;
using TimeSeries.Data.Protobuf.Models;
using ONE.Utilities;

namespace ONE.Ingest
{
    public class IngestLogger
    {
        private AuthenticationApi _authentificationApi;
        private DigitalTwinApi _digitalTwinApi;
        private DataApi _dataApi;

        /// <summary>
        /// Instantiates the Ingest Logger Class
        /// </summary>
        /// <param name="authentificationApi">The Authentication Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        /// <param name="loggerTwinId">Twin Reference Id of the Telemetry Log Twin</param>
        public IngestLogger(AuthenticationApi authentificationApi, DigitalTwinApi digitalTwinApi, DataApi dataApi, string loggerTwinId)
        {
            _authentificationApi = authentificationApi;
            _digitalTwinApi = digitalTwinApi;
            _dataApi = dataApi;
            Id = loggerTwinId;
            Log = new TimeSeriesDatas();

        }
        /// <summary>
        /// Intiates a new Logger
        /// </summary>
        /// <param name="authentificationApi">The Authentication Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        /// <param name="parentId">The Reference Id of the Instrument Twin the logger belongs</param>
        /// <param name="loggerName">Display name of the Log Telemetry Dataset</param>
        /// <returns></returns>
        public static async Task<IngestLogger> InitializeAsync(AuthenticationApi authentificationApi, DigitalTwinApi digitalTwinApi, DataApi dataApi, string parentId, string loggerName)
        {
            DigitalTwin loggerTwin = new DigitalTwin
            {
                CategoryId = Enterprise.Twin.Constants.TelemetryCategory.Id,
                TwinTypeId = Enterprise.Twin.Constants.TelemetryCategory.HistorianType.RefId,
                TwinSubTypeId = Enterprise.Twin.Constants.TelemetryCategory.HistorianType.Logger.RefId,
                TwinReferenceId = Guid.NewGuid().ToString(),
                Name = loggerName,
                ParentTwinReferenceId = parentId
            };
            loggerTwin = await digitalTwinApi.CreateAsync(loggerTwin);
            if (loggerTwin != null)
            {
                IngestLogger ingestAgent = new IngestLogger(authentificationApi, digitalTwinApi, dataApi, loggerTwin.TwinReferenceId);
                return ingestAgent;
            }
            return null;
        }

        /// <summary>
        /// Log data is a memory cache for the data to be stored by Telemetry GUID (Id)
        /// </summary>
        public TimeSeriesDatas Log { get; set; }
        
        /// <summary>
        /// Twin Reference Id of the Telemetry Log Twin
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Store Log data in local cache until upload
        /// </summary>
        /// <param name="message">Short Log Message</param>
        /// <param name="detail">Longer log message</param>
        public void IngestLogData(string message, object detail)
        {
            string propertybag = "";
            if (detail == null)
                propertybag = "";
            else if (detail is Type String)
            {
                propertybag = detail.ToString();
            }
            Log.Items.Add(new TimeSeriesData
            {
                DateTimeUTC = DateTimeHelper.ToJsonTicksDateTime(DateTime.Now),
                Id = Guid.NewGuid().ToString(),
                TelemetryTwinRefId = Id,
                PropertyBag = propertybag,
                StringValue = message

            });
            
        }
        /// <summary>
        /// Uploads the log data to the Historical Log Dataset in the cloud
        /// </summary>
        /// <returns>Whether the upload was successful</returns>
        public async Task<bool> UploadAsync()
        {
            var result = await _dataApi.SaveDataAsync(Id, Log);
            if (result == null)
                return false;
            else
            {
                Log = new TimeSeriesDatas();
                return true;
            }
        }
        
    }
}
