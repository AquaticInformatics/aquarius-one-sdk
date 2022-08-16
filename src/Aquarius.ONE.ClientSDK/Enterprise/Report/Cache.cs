using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Enterprise.Report
{
    public class Cache
    {
        private string _operationId;
        private ClientSDK _clientSDK { get; set; }
        public Cache(ClientSDK clientSDK, string operationId)
        {
            _clientSDK = clientSDK;
            _operationId = operationId;
        }
        public List<ReportDefinition> ReportDefinitions { get; set; }
        public bool IsCached { get; set; }

        private ReportDefinition _currentReportDefinition;
        public ReportDefinition CurrentReportDefinition {
            get
            { 
                return _currentReportDefinition; 
            }
            set
            {
                _currentReportDefinition = value;
                _currentReportDataDefinition = null;
            }
                }

        private ReportDataDefinition _currentReportDataDefinition;
        private ReportDataDefinition CurrentReportDataDefinition
        {
            get
            {
                if (_currentReportDataDefinition == null && _currentReportDefinition != null)
                    _currentReportDataDefinition = new ReportDataDefinition(_currentReportDefinition.ReportDefinitionJson);
                return _currentReportDataDefinition;
            }

        }
        
        public async Task<bool> LoadAsync()
        {
            if (IsCached)
                return true;

            try
            {
                ReportDefinitions = await _clientSDK.Report.GetDefinitionsAsync(_operationId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
