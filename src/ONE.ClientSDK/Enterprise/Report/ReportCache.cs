using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ONE.Models.CSharp;

namespace ONE.ClientSDK.Enterprise.Report
{
	public class ReportCache
	{
		private readonly string _operationId;
		private OneApi ClientSdk { get; }

		public ReportCache(OneApi clientSdk, string operationId)
		{
			ClientSdk = clientSdk;
			_operationId = operationId;
		}

		public ReportCache() { }

		public List<ReportDefinition> ReportDefinitions { get; set; }
		public bool IsCached { get; set; }

		public ReportDefinition GetReportDefinition(string guid)
		{
			if (string.IsNullOrEmpty(guid) || ReportDefinitions == null || ReportDefinitions.Count == 0)
				return null;

			return ReportDefinitions.FirstOrDefault(c => string.Equals(c.Id, guid, StringComparison.CurrentCultureIgnoreCase));
		}

		private ReportDefinition _currentReportDefinition;

		public ReportDefinition CurrentReportDefinition
		{
			get => _currentReportDefinition;
			set
			{
				_currentReportDefinition = value;
				_currentReportDataDefinition = null;
			}
		}

		private ReportDataDefinition _currentReportDataDefinition;
		public ReportDataDefinition CurrentReportDataDefinition
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
				ReportDefinitions = await ClientSdk.Report.GetDefinitionsAsync(_operationId);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
