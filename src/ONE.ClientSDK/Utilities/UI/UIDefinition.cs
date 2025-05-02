using System.Collections.Generic;
using Newtonsoft.Json;
using ONE.Models.CSharp.Imposed.TwinUiDefinition;

namespace ONE.ClientSDK.Utilities.UI
{
	public class UIDefinition
	{
		public UIDefinition(string json)
		{
			try
			{
				var result = JsonConvert.DeserializeObject<UIDefinition>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				Attributes = result.Attributes;
			}
			catch
			{
				Attributes = new List<UiDefinitionAttribute>();
			}
		}
	   
		public List<UiDefinitionAttribute> Attributes { get; set; }
		public override string ToString()
		{
			try
			{
				return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			}
			catch
			{
				return base.ToString();
			}
		}
	}
}
