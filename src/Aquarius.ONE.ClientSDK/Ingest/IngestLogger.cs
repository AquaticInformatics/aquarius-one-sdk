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
        
        public TimeSeriesDatas Logs { get; set; }
        public DigitalTwin DigitalTwin { get; set; }

        public void Log(string message, object detail)
        {
            string propertybag = "";
            if (detail == null)
                propertybag = "";
            else if (detail is Type String)
            {
                propertybag = detail.ToString();
            }
            Logs.Items.Add(new TimeSeriesData
            {
                DateTimeUTC = DateTimeHelper.ToJsonTicksDateTime(DateTime.Now),
                Id = Guid.NewGuid().ToString(),
                TelemetryTwinRefId = DigitalTwin.TwinReferenceId,
                PropertyBag = propertybag,
                StringValue = message

            });
            
        }
        public async Task<bool> UploadAsync()
        {
            var result = await _dataApi.SaveDataAsync(DigitalTwin.TwinReferenceId, Logs);
            if (result == null)
                return false;
            else
            {
                Logs = new TimeSeriesDatas();
                return true;
            }
        }
        public IngestLogger(AuthenticationApi authentificationApi, DigitalTwinApi digitalTwinApi, DataApi dataApi, DigitalTwin loggerTwin)
        {
            _authentificationApi = authentificationApi;
            _digitalTwinApi = digitalTwinApi;
            _dataApi = dataApi;
            DigitalTwin = loggerTwin;
            Logs = new TimeSeriesDatas();
            
        }
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
                IngestLogger ingestAgent = new IngestLogger(authentificationApi, digitalTwinApi, dataApi, loggerTwin);
                return ingestAgent;
            }
            return null;
        }
    }
}
