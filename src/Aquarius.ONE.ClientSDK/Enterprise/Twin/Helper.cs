using ONE.Models.CSharp;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Enterprise.Twin
{
    public class Helper
    {
        public Helper (DigitalTwinApi digitalTwinApi)
        {
            ItemDictionarybyGuid = new Dictionary<string, DigitalTwinItem>();
            ItemDictionarybyLong = new Dictionary<long, DigitalTwinItem>();
            _digitalTwinApi = digitalTwinApi;
        }
        private DigitalTwinApi _digitalTwinApi;
        public DigitalTwinItem OperationDigitalTwinItem { get; set; }
        private string _delimiter = "\\";
        public string Delimiter 
        {
            get
            {
                return _delimiter;
            }
            set
            {
                _delimiter = value;
            }
        }
        public Dictionary<string, DigitalTwinItem> ItemDictionarybyGuid { get; set; }
        public Dictionary<long, DigitalTwinItem> ItemDictionarybyLong { get; set; }
        public async Task<bool> LoadOperationAsync(string operationId)
        {
            var operationDigitalTwin = await _digitalTwinApi.GetAsync(operationId);
            if (operationDigitalTwin != null)
            {
                OperationDigitalTwinItem = new DigitalTwinItem(operationDigitalTwin);
                //Load Location Structure
                var locationDigitalTwins = await _digitalTwinApi.GetDescendantsAsync(operationDigitalTwin.TwinReferenceId, Constants.SpaceCategory.LocationType.RefId);

                //Load Column Telemetry Twins
                var columnDigitalTwins = await _digitalTwinApi.GetDescendantsAsync(operationDigitalTwin.TwinReferenceId, Constants.TelemetryCategory.ColumnType.RefId);

                //Merge the Twins
                var allChildTwins = locationDigitalTwins.Union(columnDigitalTwins).ToList();
                //Create Twin Hierarchy
                AddChildren(OperationDigitalTwinItem, allChildTwins);
                return true;
            }
            else
                return false;
        }
        private void AddChildren(DigitalTwinItem digitalTwinTreeItem, List<DigitalTwin> digitalTwins)
        {
            var childDigitalTwins = digitalTwins.Where(p => p.ParentId == digitalTwinTreeItem.DigitalTwin.Id);
            foreach (DigitalTwin digitalTwin in childDigitalTwins)
            {
                var childDigitalTwinItem = new DigitalTwinItem(digitalTwin);
                if (string.IsNullOrEmpty(digitalTwinTreeItem.Path))
                    childDigitalTwinItem.Path = childDigitalTwinItem.DigitalTwin.Name;
                else
                    childDigitalTwinItem.Path = $"{digitalTwinTreeItem.Path}{Delimiter}{childDigitalTwinItem.DigitalTwin.Name}";
                digitalTwinTreeItem.ChildDigitalTwinItems.Add(childDigitalTwinItem);
                if (!string.IsNullOrEmpty(digitalTwin.TwinReferenceId))
                    ItemDictionarybyGuid.Add(digitalTwin.TwinReferenceId, childDigitalTwinItem);
                ItemDictionarybyLong.Add(digitalTwin.Id, childDigitalTwinItem);
                AddChildren(childDigitalTwinItem, digitalTwins);
            }
        }
        public string GetTelemetryPath(string digitalTwinReferenceId, bool includeItem)
        {
            if (ItemDictionarybyGuid.ContainsKey(digitalTwinReferenceId))
            {
                if (includeItem)
                {
                    return ItemDictionarybyGuid[digitalTwinReferenceId].Path;
                }
                else
                {
                    if (ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId != null &&
                        ItemDictionarybyGuid.ContainsKey(ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId))
                    {
                        return ItemDictionarybyGuid[ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId].Path;
                    }
                    else if (ItemDictionarybyLong.ContainsKey((long)ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentId))
                    {
                        return ItemDictionarybyLong[(long)ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentId].Path;
                    }
                }
            }
            return "";
        }
        public static DigitalTwin GetByRef(List<DigitalTwin> digitalTwins, string twinRefId)
        {
            if (digitalTwins == null || string.IsNullOrEmpty(twinRefId))
                return null;
            var matches = digitalTwins.Where(p => p.TwinReferenceId != null && String.Equals(p.TwinReferenceId.ToUpper(), twinRefId.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public static string AddUpdateRootValue(string key, JObject value, string json)
        {
            dynamic jsonObj;
            if (json == null)
                jsonObj = new JObject();
            else
                jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj[key] = value;
            return JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.None);
        }
        public static string AddUpdateRootValue(string key, string value, string json)
        {
            dynamic jsonObj;
            if (json == null)
                jsonObj = new JObject();
            else
                jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj[key] = value;
            return JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.None);
        }
        public static List<DigitalTwin> GetByParentRef(List<DigitalTwin> digitalTwins, string twinRefId)
        {
            if (digitalTwins == null || string.IsNullOrEmpty(twinRefId))
                return new List<DigitalTwin>();
            var matches = digitalTwins.Where(p => p.ParentTwinReferenceId != null && String.Equals(p.ParentTwinReferenceId.ToUpper(), twinRefId.ToUpper(), StringComparison.CurrentCulture));
            return matches.ToList();
        }
        public static long GetLongTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
        {
            long.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out long propertyValue);
            return propertyValue;
        }
        public static bool GetBoolTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
        {
            bool.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out bool propertyValue);
            return propertyValue;
        }
        public static DateTime GetDateTimeTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
        {
            DateTime.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out DateTime propertyValue);
            return propertyValue;
        }
        public static double GetDoubleTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
        {
            string value = GetTwinDataProperty(digitalTwin, path, propertyName);
            double.TryParse(value, out double propertyValue);
            return propertyValue;
        }
        public static float? GetFloatTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
        {
            float? propertyValue = null;
            if (!string.IsNullOrEmpty(GetTwinDataProperty(digitalTwin, path, propertyName)))
            {
                float.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out float value);
                propertyValue = value;
            }
            return propertyValue;
        }
        public static DigitalTwin GetByDataProperty(List<DigitalTwin> digitalTwins, string path, string propertyName, string value)
        {
            foreach (DigitalTwin digitalTwin in digitalTwins)
            {
                string prop = GetTwinDataProperty(digitalTwin, path, propertyName);
                if (prop != null && prop.ToUpper() == value.ToUpper())
                {
                    return digitalTwin;
                }
            }
            return null;
        }
        public static string GetTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
        {
            if (digitalTwin != null && !string.IsNullOrEmpty(digitalTwin.TwinData))
            {
                JObject twinData = JObject.Parse(digitalTwin.TwinData);
                if (twinData == null)
                    return null;
                var pathItems = path.Split('\\');
                JObject parentObject = twinData;
                foreach (var pathItem in pathItems)
                {
                    if (!string.IsNullOrEmpty(pathItem))
                    {
                        if (parentObject[pathItem] == null || !parentObject[pathItem].HasValues)
                        {
                            return "";
                        }
                        parentObject = (JObject)parentObject[pathItem];
                        if (parentObject == null)
                            return "";
                    }
                }
                if (parentObject[propertyName] == null)
                    return "";
                return parentObject[propertyName].ToString();

            }
            return "";
        }
        public static List<string> GetTwinDataPropertyAslist(DigitalTwin digitalTwin, string path, string propertyName)
        {
            if (digitalTwin != null && !string.IsNullOrEmpty(digitalTwin.TwinData))
            {
                JObject twinData = JObject.Parse(digitalTwin.TwinData);
                if (twinData == null)
                    return null;
                var pathItems = path.Split('\\');
                JObject parentObject = twinData;
                foreach (var pathItem in pathItems)
                {
                    if (!string.IsNullOrEmpty(pathItem))
                    {
                        if (parentObject[pathItem] == null || !parentObject[pathItem].HasValues)
                        {
                            return new List<string>();
                        }
                        parentObject = (JObject)parentObject[pathItem];
                        if (parentObject == null)
                            return new List<string>();
                    }
                }
                if (parentObject[propertyName] == null)
                    return new List<string>();
                if (parentObject[propertyName] is JArray)
                {
                    return parentObject[propertyName].ToObject<List<string>>();
                }
            }
            return new List<string>();
        }

        public static JsonPatchDocument UpdateJsonDataField(DigitalTwin digitalTwin, JsonPatchDocument jsonPatchDocument, JObject existingTwinData, string key, string value)
        {
            if (existingTwinData.ContainsKey(key) && string.IsNullOrEmpty(value))
            {
                jsonPatchDocument.Remove($"/{key}");
            }
            else if (!string.IsNullOrEmpty(value))
            {
                string currentValue = GetTwinDataProperty(digitalTwin, "", key);
                if (currentValue != value)
                    jsonPatchDocument.Add($"/{key}", value);
            }
            return jsonPatchDocument;
        }
        public static bool DoesTwinArrayPropertyExist(DigitalTwin digitalTwin, string path, string key, string propertyName)
        {
            try
            {
                if (digitalTwin != null && !string.IsNullOrEmpty(digitalTwin.TwinData))
                {
                    JObject twinData = JObject.Parse(digitalTwin.TwinData);
                    if (twinData == null)
                        return false;
                    if (!string.IsNullOrEmpty(path))
                    {

                        var Array = (JArray)twinData[path];
                        if (Array == null)
                            return false;
                        for (int i = 0; i < Array.Count; i++)
                        {
                            if (Array[i][key] != null && Array[i][key].ToString().ToUpper() == propertyName.ToUpper())
                                return true;
                        }
                    }


                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
            
        
