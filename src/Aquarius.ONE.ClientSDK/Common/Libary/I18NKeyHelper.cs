using ONE.Models.CSharp.Imposed.Internationalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ONE.Common.Library
{
    [Serializable]
    public static class I18NKeyHelper
    {
        public static List<I18NKey> I18NKeyList { get; set; }
      
        public static List<I18NKey> Load(List<I18NKey> translations, Dictionary<string, string> values, string module, string type)
        {
            foreach (var dictionaryItem in values)
            {
                I18NKey translation = new I18NKey(module, type, dictionaryItem.Key, dictionaryItem.Value);
                translations.Add(translation);
            }
            return translations;
        }
        public static string GetValue(string type, string key)
        {
            if (I18NKeyList == null)
                return null;
            var matches = I18NKeyList.Where(p => (String.Equals(p.Key, key, StringComparison.CurrentCulture) && String.Equals(p.Type, type, StringComparison.CurrentCulture)));
            if (matches.Count() > 0)
            {
                return matches.First().Value;
            }
            else
            {
                return "";
            }
        }
        public static string GetShortValue(string key)
        {
            return GetValue("SHORT", key);
        }
        public static string GetLongValue(string key)
        {
            return GetValue("LONG", key);
        }
        public static DataTable GetDataTable()
        {
            DataTable dataTable = new DataTable("Translations");


            //Create Columns
            PropertyInfo[] parameterProps = typeof(I18NKey).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in parameterProps)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }

            //Load Data
            if (I18NKeyHelper.I18NKeyList != null)
            {
                foreach (I18NKey item in I18NKeyHelper.I18NKeyList)
                {
                    DataRow row = dataTable.NewRow();
                    for (int i = 0; i < parameterProps.Length; i++)
                    {
                        row[parameterProps[i].Name] = parameterProps[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }
    }
}
