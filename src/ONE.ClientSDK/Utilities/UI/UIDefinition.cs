using Newtonsoft.Json;
using ONE.Models.CSharp.Imposed.TwinUiDefinition;
using System.Collections.Generic;

namespace ONE.ClientSDK.Utilities.UI
{
	public class UIDefinition
	{
		public UIDefinition(string json)
		{
			try
			{
				var result = JsonConvert.DeserializeObject<UIDefinition>(json, JsonExtensions.IgnoreNullSerializerSettings);
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
				return JsonConvert.SerializeObject(this, JsonExtensions.IgnoreNullSerializerSettings);
			}
			catch
			{
				return base.ToString();
			}
		}
	}
}
