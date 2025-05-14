using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Constants.TwinCategory;
using ONE.Shared.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Enterprise.Twin
{
	public class DigitalTwinHelper
	{
		public DigitalTwinHelper (DigitalTwinApi digitalTwinApi)
		{
			ItemDictionaryByGuid = new Dictionary<string, DigitalTwinItem>();
			ItemDictionaryByLong = new Dictionary<long, DigitalTwinItem>();
			_digitalTwinApi = digitalTwinApi;
		}

		private readonly DigitalTwinApi _digitalTwinApi;

		public DigitalTwinItem OperationDigitalTwinItem { get; set; }
		public string Delimiter { get; set; } = "\\";

		public Dictionary<string, DigitalTwinItem> ItemDictionaryByGuid { get; set; }
		public Dictionary<long, DigitalTwinItem> ItemDictionaryByLong { get; set; }

		public async Task<bool> LoadOperationAsync(string operationId)
		{
			var operationDigitalTwin = await _digitalTwinApi.GetAsync(operationId);
			if (operationDigitalTwin != null)
			{
				OperationDigitalTwinItem = new DigitalTwinItem(operationDigitalTwin);

				//Load Location Structure
				var locationDigitalTwins = await _digitalTwinApi.GetDescendantsByCategoryAsync(operationDigitalTwin.TwinReferenceId, SpaceConstants.Id);

				//Load Column Telemetry Twins
				var columnDigitalTwins = await _digitalTwinApi.GetDescendantsAsync(operationDigitalTwin.TwinReferenceId, TelemetryConstants.ColumnType.RefId);

				//Merge the Twins
				var allChildTwins = locationDigitalTwins.Union(columnDigitalTwins).ToList();

				//Create Twin Hierarchy
				AddChildren(OperationDigitalTwinItem, allChildTwins);
				return true;
			}

			return false;
		}

		private void AddChildren(DigitalTwinItem digitalTwinTreeItem, List<DigitalTwin> digitalTwins)
		{
			var childDigitalTwins = digitalTwins.Where(p => p.ParentId == digitalTwinTreeItem.DigitalTwin.Id);
			foreach (var digitalTwin in childDigitalTwins)
			{
				var childDigitalTwinItem = new DigitalTwinItem(digitalTwin);
				childDigitalTwinItem.Path = string.IsNullOrEmpty(digitalTwinTreeItem.Path)
					? childDigitalTwinItem.DigitalTwin.Name
					: $"{digitalTwinTreeItem.Path}{Delimiter}{childDigitalTwinItem.DigitalTwin.Name}";
				digitalTwinTreeItem.ChildDigitalTwinItems.Add(childDigitalTwinItem);
				if (!string.IsNullOrEmpty(digitalTwin.TwinReferenceId))
					ItemDictionaryByGuid.Add(digitalTwin.TwinReferenceId, childDigitalTwinItem);
				ItemDictionaryByLong.Add(digitalTwin.Id, childDigitalTwinItem);
				AddChildren(childDigitalTwinItem, digitalTwins);
			}
		}

		public string GetTelemetryPath(string digitalTwinReferenceId, bool includeItem)
		{
			if (ItemDictionaryByGuid.ContainsKey(digitalTwinReferenceId))
			{
				if (includeItem)
					return ItemDictionaryByGuid[digitalTwinReferenceId].Path;

				if (ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId != null &&
				    ItemDictionaryByGuid.ContainsKey(ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId))
				{
					return ItemDictionaryByGuid[ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId].Path;
				}

				if (ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentId.HasValue &&
				    ItemDictionaryByLong.ContainsKey((long)ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentId))
				{
					return ItemDictionaryByLong[(long)ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentId].Path;
				}
			}

			return "";
		}

		public static DigitalTwin GetByRef(List<DigitalTwin> digitalTwins, string twinRefId)
		{
			if (digitalTwins == null || string.IsNullOrEmpty(twinRefId))
				return null;

			return digitalTwins.FirstOrDefault(p => p.TwinReferenceId != null && string.Equals(p.TwinReferenceId.ToUpper(), twinRefId.ToUpper(), StringComparison.CurrentCulture));
		}

		public static string AddUpdateRootValue(string key, JObject value, string json)
		{
			dynamic jsonObj = json == null ? new JObject() : JsonConvert.DeserializeObject(json) ?? new JObject();
			jsonObj[key] = value;

			return JsonConvert.SerializeObject(jsonObj, Formatting.None);
		}

		public static string AddUpdateRootValue(string key, string value, string json)
		{
			dynamic jsonObj = json == null ? new JObject() : JsonConvert.DeserializeObject(json) ?? new JObject();
			jsonObj[key] = value;

			return JsonConvert.SerializeObject(jsonObj, Formatting.None);
		}

		public static List<DigitalTwin> GetByParentRef(List<DigitalTwin> digitalTwins, string twinRefId)
		{
			if (digitalTwins == null || string.IsNullOrEmpty(twinRefId))
				return new List<DigitalTwin>();

			return digitalTwins.Where(p => p.ParentTwinReferenceId != null && string.Equals(p.ParentTwinReferenceId.ToUpper(), twinRefId.ToUpper(), StringComparison.CurrentCulture)).ToList();
		}

		public static long GetLongTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
		{
			long.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out var propertyValue);
			return propertyValue;
		}

		public static bool GetBoolTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
		{
			bool.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out var propertyValue);
			return propertyValue;
		}

		public static DateTime GetDateTimeTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
		{
			DateTimeHelper.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out var propertyValue);
			return propertyValue;
		}

		public static double GetDoubleTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
		{
			var value = GetTwinDataProperty(digitalTwin, path, propertyName);
			double.TryParse(value, out var propertyValue);
			return propertyValue;
		}

		public static int GetIntTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
		{
			var value = GetTwinDataProperty(digitalTwin, path, propertyName);
			int.TryParse(value, out var propertyValue);
			return propertyValue;
		}

		public static float? GetFloatTwinDataProperty(DigitalTwin digitalTwin, string path, string propertyName)
		{
			float? propertyValue = null;
			if (!string.IsNullOrEmpty(GetTwinDataProperty(digitalTwin, path, propertyName)))
			{
				float.TryParse(GetTwinDataProperty(digitalTwin, path, propertyName), out var value);
				propertyValue = value;
			}
			return propertyValue;
		}

		public static DigitalTwin GetByDataProperty(List<DigitalTwin> digitalTwins, string path, string propertyName, string value)
		{
			foreach (var digitalTwin in digitalTwins)
			{
				var prop = GetTwinDataProperty(digitalTwin, path, propertyName);
				if (prop != null && string.Equals(prop, value, StringComparison.CurrentCultureIgnoreCase))
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
				var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
				var parentObject = JsonConvert.DeserializeObject<JObject>(digitalTwin.TwinData, settings);

				if (parentObject == null)
					return null;

				var pathItems = path.Split('\\');
				foreach (var pathItem in pathItems)
				{
					if (!string.IsNullOrEmpty(pathItem))
					{
						if (parentObject[pathItem] == null || !parentObject[pathItem].HasValues)
							return "";

						parentObject = (JObject)parentObject[pathItem];
						if (parentObject == null)
							return "";
					}
				}

				var propertyValue = parentObject[propertyName];

				return propertyValue == null ? "" : propertyValue.ToString();
			}

			return "";
		}
		
		public static List<string> GetTwinDataPropertyAsList(DigitalTwin digitalTwin, string path, string propertyName)
		{
			if (digitalTwin != null && !string.IsNullOrEmpty(digitalTwin.TwinData))
			{
				var twinData = JObject.Parse(digitalTwin.TwinData);
				if (twinData == null)
					return null;

				var pathItems = path.Split('\\');
				var parentObject = twinData;

				foreach (var pathItem in pathItems)
				{
					if (!string.IsNullOrEmpty(pathItem))
					{
						if (parentObject[pathItem] == null || !parentObject[pathItem].HasValues)
							return new List<string>();
						
						parentObject = (JObject)parentObject[pathItem];

						if (parentObject == null)
							return new List<string>();
					}
				}

				if (parentObject[propertyName] == null)
					return new List<string>();

				if (parentObject[propertyName] is JArray)
					return parentObject[propertyName].ToObject<List<string>>();
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
				var currentValue = GetTwinDataProperty(digitalTwin, "", key);
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
					var twinData = JObject.Parse(digitalTwin.TwinData);
					if (twinData == null)
						return false;

					if (!string.IsNullOrEmpty(path))
					{
						var array = (JArray)twinData[path];
						if (array == null)
							return false;

						foreach (var t in array)
						{
							if (t[key] != null && string.Equals(t[key].ToString(), propertyName, StringComparison.CurrentCultureIgnoreCase))
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
			
		
