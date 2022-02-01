using ONE.Models.CSharp;
using System.Collections.Generic;

namespace ONE.Enterprise.Twin
{
    public class DigitalTwinItem
    {
        public DigitalTwinItem(DigitalTwin digitalTwin)
        {
            this.DigitalTwin = digitalTwin;
            ChildDigitalTwinItems = new List<DigitalTwinItem>();
        }
        public DigitalTwin DigitalTwin { get; set; }
        public List<DigitalTwinItem> ChildDigitalTwinItems { get; set; }
        public string Path { get; set; }
      
    }
}
