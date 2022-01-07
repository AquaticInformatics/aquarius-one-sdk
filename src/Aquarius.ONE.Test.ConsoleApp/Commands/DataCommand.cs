using CommandLine;
using ONE.Utilities;
using ONE;
using System;
using System.Threading.Tasks;
using TimeSeries.Data.Protobuf.Models;

namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("data", HelpText = "Manage Data.")]
    public class DataCommand: ICommand
    {
       
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            string telemetryTwinRefId = "45550715-8734-4c1a-8526-d0857c3f1c63";
            TimeSeriesDatas timeSeriesDatas = new TimeSeriesDatas();
            timeSeriesDatas.Items.Add(new TimeSeriesData
            {
                DateTimeUTC = DateTimeHelper.ToJsonTicksDateTime(DateTime.Now),
                Id = Guid.NewGuid().ToString(),
                TelemetryTwinRefId = telemetryTwinRefId,
                PropertyBag = "",
                Value = 5,
                StringValue = "<10"

            });
            var result = await clientSDK.Data.SaveData(telemetryTwinRefId, timeSeriesDatas);
            if (result == null)
                return 0;
            else
            {
                Console.WriteLine(result);
                return 1;
            }
        }
    }
}
