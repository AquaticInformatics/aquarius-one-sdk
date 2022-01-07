using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Utilities
{ 
    public enum EnumCascadeRule
    {
        DisplayNoQualifers = 0,
        DisplayIfOneValueHasQualifier = 1,
        DisplayIfHalfContainsQualifiers = 2,
        DisplayIfAllContainQualifiers = 3,
        DisplayAllDataQualifier = 4,
        Invalid= 5
    }
}
