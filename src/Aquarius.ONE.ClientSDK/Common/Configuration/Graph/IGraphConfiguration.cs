using System.Collections.Generic;

namespace ONE.Common.Configuration.Graph
{
    /*
    "name": "Flow ",
    "defaultDateRange": 2,
    "columns": [231100, 231099, 231138],
    "series": [{
            "twinId": 231100,
            "color": "#23409A",
            "showLimits": true,
            "type": "line",
            "axis": 0
        }, {
            "twinId": 231099,
            "color": "#A614AD",
            "showLimits": false,
            "type": "line",
            "axis": 0
        }, {
            "twinId": 231138,
            "color": "#520408",
            "showLimits": false,
            "type": "bar",
            "axis": "1"
        }
    ],
    "axisMode": "2"
     */
    public interface IGraphConfiguration
    {
        string name { get; set; }
        EnumRelativeDateRange defaultDateRange { get; set; }
        List<uint> columns { get; set; }
        List<ISavedSeriesInfo> series {get; set;}
        IGraphAxisMode axisMode { get; set; }
    }
    
}
