using ONE.Models.CSharp.Imposed.Internationalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ONE.ClientSDK.Common.Library
{
	[Serializable]
	public static class I18NKeyHelper
	{
		public static List<I18NKey> I18NKeyList { get; set; }
	  
		public static List<I18NKey> Load(List<I18NKey> translations, Dictionary<string, string> values, string module, string type)
		{
			foreach (var dictionaryItem in values)
			{
				var translation = new I18NKey(module, type, dictionaryItem.Key, dictionaryItem.Value);
				translations.Add(translation);
			}

			return translations;
		}

		public static string GetValue(string type, string key)
		{
			if (I18NKeyList == null)
				return null;

			return I18NKeyList.FirstOrDefault(p => string.Equals(p.Key, key, StringComparison.CurrentCulture) && string.Equals(p.Type, type, StringComparison.CurrentCulture))?.Value ?? string.Empty;
		}

		public static string GetShortValue(string key) => GetValue("SHORT", key);

		public static string GetLongValue(string key) => GetValue("LONG", key);

		public static DataTable GetDataTable()
		{
			var dataTable = new DataTable("Translations");

			//Create Columns
			var parameterProps = typeof(I18NKey).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			//Setting column names as Property names
			foreach (var prop in parameterProps)
				dataTable.Columns.Add(prop.Name);

			//Load Data
			if (I18NKeyList != null)
			{
				foreach (var item in I18NKeyList)
				{
					var row = dataTable.NewRow();

					foreach (var t in parameterProps)
						row[t.Name] = t.GetValue(item, null);

					dataTable.Rows.Add(row);
				}
			}

			return dataTable;
		}
	}
}
