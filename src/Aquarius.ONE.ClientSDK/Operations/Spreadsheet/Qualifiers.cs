using ONE.Models.CSharp;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ONE.Operations.Spreadsheet
{
    public static class Qualifiers
    {
        public const string QualifierList = "<,>,ND,E,DNQ,TNTC,P,A";
        public static string[] QualifierArray
        {
            get { return QualifierList.Split(','); }
        }

        public static int[] GetQualifierCount(List<Measurement> measurements)
        {
            int[] intDQCount = new int[QualifierArray.Length];
            
            for (int i = 0; i < QualifierArray.Length; i++)
            {
                intDQCount[i] = measurements.Select(x => x.StringValue).Where(x => x.StartsWith(QualifierArray[i])).Count();

            }
            return intDQCount;
        }
        public static SymbolResult GetMaxSymbol(int[] intDQCount)
        {
            int maxCount = 0;
            int maxIndex = 0;
            
            for (int i = 0; i < QualifierArray.Length; i++)
            {
                if (intDQCount[i] > maxCount)
                {
                    maxCount = intDQCount[i];
                    maxIndex = i;
                }

            }
            if (maxCount > 0)
                return new SymbolResult { Count = maxCount, Symbol = QualifierArray[maxIndex] };
            else
                return new SymbolResult { Count = maxCount };

        }
        public static string GetSymbol(string value)
        {
            string nonqualifiers = " -.0123456789";
            bool Found = false;
            string result = "";
            if (string.IsNullOrEmpty(value))
                return value;

            for (int i = 0; i < value.Length; i++)
            {
                if (value.Substring(i, 1) == "-" && i < value.Length)
                {
                    if (nonqualifiers.Contains(value[i]))
                    {
                        if (i > 0)
                            result = value.Substring(0, i).Trim();
                        Found = true;
                        break;
                    }
                }
                else if (nonqualifiers.Contains(value[i]))
                {
                    if (i > 0)
                        result = value.Substring(0, i);
                    Found = true;
                    break;
                }
            }
            if (!Found)
                return value.Trim();
            if (!QualifierList.Contains(result))
                return "";
            return result;
        }
        public static string GetAggregateQualifier(List<Measurement> measurements, int cascadeRule)
        {
            var dqCount = GetQualifierCount(measurements);
            var maxSymbol = GetMaxSymbol(dqCount);
            if (measurements == null || measurements.Count == 0 || maxSymbol.Count == 0)
            switch (cascadeRule)
            {
                case 1:     //if one value < or > place
                    if (maxSymbol.Count > 0)
                        return maxSymbol.Symbol;
                    else
                        return "";
                case 2:     //if all are < or > place
                    if (maxSymbol.Count == measurements.Count)
                        return maxSymbol.Symbol;
                    else
                        return "";
                case 3:     //if more than half
                    if (maxSymbol.Count == measurements.Count / 2)
                        return maxSymbol.Symbol;
                    else
                        return "";
                //Cascade Rules 4-7 Deprecated per Scott Dorner
            }


            return "";
        }

        public static string Format(double? value, int[] qualifierCount, long numberOfSamples, EnumCascadeRule cascadeRule, string strFormat)
        {
            if (value == null)
                return "";
            string formattedValue = ((double)value).ToString(strFormat);
            int? FoundIndex = null;
            switch (cascadeRule)
            {
                case EnumCascadeRule.DisplayNoQualifers:
                    return formattedValue;
                case EnumCascadeRule.DisplayIfOneValueHasQualifier:
                    double MaxCount = 0.5;
                    for (int i = 0; i < qualifierCount.Length; i++)
                    {
                        if (qualifierCount[i] > MaxCount)
                        {
                            FoundIndex = i;
                            MaxCount = qualifierCount[i];
                        }
                        if (MaxCount > 0.5 && FoundIndex != null)
                            return QualifierArray[(int)FoundIndex] + formattedValue;
                        else
                            return formattedValue;
                    }
                    break;
                case EnumCascadeRule.DisplayIfHalfContainsQualifiers:
                    for (int i = 0; i < qualifierCount.Length; i++)
                    {
                        if (qualifierCount[i] == numberOfSamples)
                            return QualifierArray[i] + formattedValue;
                    }
                    break;
                case EnumCascadeRule.DisplayIfAllContainQualifiers:
                    for (int i = 0; i < qualifierCount.Length; i++)
                    {
                        if (qualifierCount[i] >= (0.5 * numberOfSamples))
                            return QualifierArray[i] + formattedValue;
                    }
                    break;
                case EnumCascadeRule.DisplayAllDataQualifier:
                    for (int i = 0; i < qualifierCount.Length; i++)
                    {
                        if (qualifierCount[i] > 0)
                            return QualifierArray[i] + formattedValue;
                    }
                    break;
            }
            return formattedValue;

        }
        public static EnumQualifierRule GetQualifierRule(string rule)
        {
            if (string.IsNullOrEmpty(rule))
                return EnumQualifierRule.Invalid;
            switch (rule)
            {
                case "0":
                    return EnumQualifierRule.Zero;
                case "1":
                    return EnumQualifierRule.EnteredValue;
                case "2":
                    return EnumQualifierRule.HalfOfOrDouble;
                case "3":
                    return EnumQualifierRule.ZeroOrEnteredValue;
            }
            return EnumQualifierRule.Invalid;

        }
        public static EnumCascadeRule GetCascadeRule(string rule)
        {
            if (string.IsNullOrEmpty(rule))
                return EnumCascadeRule.Invalid;
            switch (rule)
            {
                case "0":
                    return EnumCascadeRule.DisplayNoQualifers;
                case "1":
                    return EnumCascadeRule.DisplayIfOneValueHasQualifier;
                case "2":
                    return EnumCascadeRule.DisplayIfHalfContainsQualifiers;
                case "3":
                    return EnumCascadeRule.DisplayIfAllContainQualifiers;
                case "4":
                    return EnumCascadeRule.DisplayAllDataQualifier;
            }
            return EnumCascadeRule.Invalid;

        }
        public static object GetValue(string enteredValue, EnumQualifierRule qualifierRule)
        {
            string symbol = "";
            string numericPart = "";
            double? value;

            if (qualifierRule == EnumQualifierRule.Invalid)
                return EnumErrors.ERR_INVALID_QUALIFIER_RULE;
            symbol = GetSymbol(enteredValue);
            numericPart = enteredValue.Substring(symbol.Length).Trim();
            if (Helper.IsNumeric(numericPart))
                value = Helper.TryParseDouble(numericPart);
            else
            {
                if (string.IsNullOrEmpty(symbol))
                    return "";
                else
                    value = null;
            }
            switch (symbol.ToUpper())
            {
                case "<":
                    switch (qualifierRule)
                    {
                        case  EnumQualifierRule.Zero:
                            return 0;
                        default:
                        case EnumQualifierRule.EnteredValue:
                            return Helper.TryParseDouble(numericPart);
                        case EnumQualifierRule.HalfOfOrDouble:
                            return Helper.TryParseDouble(numericPart) / 2;
                    }
                case ">":
                    switch (qualifierRule)
                    {

                        case EnumQualifierRule.Zero:
                            return value;
                        default:
                        case EnumQualifierRule.EnteredValue:
                            return value;
                        case EnumQualifierRule.HalfOfOrDouble:
                            return value * 2;
                    }
                case "E":
                case "DNQ":
                    switch (qualifierRule)
                    {
                        case EnumQualifierRule.Zero:
                            return value;
                        default:
                        case EnumQualifierRule.EnteredValue:
                            return value;
                        case EnumQualifierRule.HalfOfOrDouble:
                            return value / 2;
                    }
                case "TNTC":
                    if (value == null)
                        return 10000;
                    else return value;
                case "A":
                case "ND":
                    return 0;
                case "P":
                    return 1;
            }
            if (value == null)
                return "";
            return value;
        }
        public static object Statistics(object range, string statistic, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string strFormat, string strSortRule = "")
        {
            if (statistic == null)
                return EnumErrors.ERR_INVALID_VALUE;

            switch (statistic.Trim().ToUpper())
            {
                case "AVG":
                case "AVE":
                case "AVERAGE":
                    return Average(range, cascadeRule, qualifierRule, strFormat);
                case "SUM":
                case "TOTAL":
                case "TOT":
                    return Sum(range, cascadeRule, qualifierRule, strFormat);
                case "MAX":
                case "MAXIMUM":
                    return Max(range, cascadeRule, qualifierRule, strFormat);
                case "MIN":
                case "MINIMUM":
                    return Min(range, cascadeRule, qualifierRule, strFormat);
                case "COUNT":
                case "NOS":
                    return Count(range, qualifierRule, strFormat);
                case "FIRST":
                    return First(range, qualifierRule, strFormat);
                case "LAST":
                    return Last(range, qualifierRule, strFormat);
                case "GM0":
                case "GM1":
                case "GM2":
                    return GeometricMeans(range, statistic, cascadeRule, qualifierRule, strFormat);
                case "MEDIAN":
                    return Median(range, cascadeRule, qualifierRule, strFormat, strSortRule);
            }
            if (statistic.Substring(0, 4) == "PERC")
                return Percentile(range, statistic, cascadeRule, qualifierRule, strFormat, strSortRule);
            else if (statistic.Substring(0, 4) == "CCGT")
                return CCGT(range, statistic, cascadeRule, qualifierRule, strFormat);
            return EnumErrors.ERR_INVALID_VALUE;
        }
        public static object Sort(object range, EnumQualifierRule qualifierRule, string sortRule, string sortOrder = "")
        {
            var array = Helper.ConvertToArray(range);
            List<QualifierItem> qualifierItems = new List<QualifierItem>();
            string temp;
            double? CurrValue = null;


            foreach (var arrayItem in array)
            {
                temp = arrayItem.ToString();
                if (!string.IsNullOrEmpty(temp))
                {
                    if (Helper.IsNumeric(temp))
                    {
                        CurrValue = Helper.TryParseDouble(temp);
                        if (CurrValue != null)
                        {
                            qualifierItems.Add(new QualifierItem { StringValue = CurrValue.ToString(), Value = (double)CurrValue, Qualifier = "" });
                        }
                    }
                    else
                    {
                        string qualifier = GetSymbol(temp);
                        if (!string.IsNullOrEmpty(qualifier))
                        {
                            CurrValue = Helper.TryParseDouble(GetValue(temp, qualifierRule));
                            if (CurrValue != null)
                                qualifierItems.Add(new QualifierItem { StringValue = temp, Value = (double)CurrValue, Qualifier = qualifier });
                        }
                    }
                }
            }
            switch (sortRule.ToUpper())
            {
                default:
                case "CALIFORNIA":
                case "CALI":
                case "CAL":
                case "CA":
                    qualifierItems.Sort();
                    break;
                case "VALUE":
                    qualifierItems = qualifierItems.OrderBy(q => q.Value).ToList();
                    break;
            }
            // qualifierItems = qualifierItems.OrderBy(q => q.Value).ToList();

            qualifierItems.ToArray();
            object[,] arr = new object[qualifierItems.Count, 3];
            for (int i = 0; i < qualifierItems.Count; i++)
            {
                arr[i, 0] = qualifierItems[i].StringValue;
                arr[i, 1] = qualifierItems[i].Value;
                arr[i, 2] = qualifierItems[i].Qualifier;
            }
            return arr;

        }
        public static object Average(object range, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string strFormat)
        {
            var array = Helper.ConvertToArray(range);
            double? CurrValue = null;
            string strCurrSymbol = "";
            string strTemp = "";

            long numberOfSymbols = 0;
            double total = 0;
            int[] intDQCount = new int[QualifierArray.Length];

            foreach (var C in array)
            {
                strTemp = C.ToString();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    if (Helper.IsNumeric(strTemp))
                    {
                        CurrValue = Helper.TryParseDouble(strTemp);
                        total += (double)CurrValue;
                        numberOfSymbols++;
                    }
                    else
                    {
                        strCurrSymbol = GetSymbol(strTemp);
                        CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                        if (!string.IsNullOrEmpty(strCurrSymbol))
                        {
                            for (int i = 0; i < QualifierArray.Length; i++)
                            {
                                if (strCurrSymbol == QualifierArray[i])
                                {
                                    intDQCount[i] = intDQCount[i] + 1;
                                    break;
                                }
                            }
                        }
                        if (CurrValue != null)
                        {
                            total = total + (double)CurrValue;
                            numberOfSymbols++;
                        }
                    }
                }
            }
            if (numberOfSymbols > 0)
                return Format(total / numberOfSymbols, intDQCount, numberOfSymbols, cascadeRule, strFormat);
            return "";

        }
        public static object Sum(object CR, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string format)
        {
            var array = Helper.ConvertToArray(CR);

            double? CurrValue = null;
            long numberOfSymbols = 0;
            double total = 0;
            string strCurrSymbol = "";
            int NumOfDQ = 3;
            int[] intDQCount = new int[QualifierArray.Length];


            foreach (var C in array)
            {
                string strTemp = C.ToString();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    if (Helper.IsNumeric(strTemp))
                    {
                        CurrValue = Helper.TryParseDouble(strTemp);
                        total += (double)CurrValue;
                        numberOfSymbols++;
                    }
                    else
                    {
                        strCurrSymbol = GetSymbol(strTemp);
                        CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                        if (CurrValue != null)
                        {
                            for (int i = 0; i < NumOfDQ; i++)
                            {
                                if (strCurrSymbol == Qualifiers.QualifierArray[i])
                                {
                                    intDQCount[i] = intDQCount[i] + 1;
                                    break;
                                }
                            }
                        }
                        if (CurrValue != null)
                        {
                            total = total + (double)CurrValue;
                            numberOfSymbols++;
                        }
                    }
                }
            }
            if (numberOfSymbols > 0)
                return Qualifiers.Format(total, intDQCount, numberOfSymbols, cascadeRule, format);
            return "";
        }
        public static object Max(object range, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string format)
        {
            // Cast CR to Array
            var array = Helper.ConvertToArray(range);
            double? dblMax = null;
            double? CurrValue = null;
            string strMax = "";
            string strCurrSymbol = "";
            string MaxSymbolStatus = "";
            bool MaxFound = false;

            foreach (var C in array)
            {
                string strTemp = C.ToString();
                if (Helper.IsNumeric(strTemp))
                {
                    CurrValue = Helper.TryParseDouble(strTemp);
                    if (dblMax == null || CurrValue > dblMax)
                    {
                        dblMax = CurrValue;
                        strMax = ((Double)CurrValue).ToString(format);
                    }
                }
                else
                {
                    strCurrSymbol = GetSymbol(strTemp);
                    CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                    if (CurrValue != null)
                    {
                        if (dblMax == null || CurrValue > dblMax)
                        {
                            dblMax = CurrValue;
                            if (cascadeRule > 0)
                            {
                                if (strCurrSymbol == "ND")
                                    strMax = "ND";
                                else
                                    strMax = strCurrSymbol + ((Double)CurrValue).ToString(format);
                                MaxSymbolStatus = strCurrSymbol;
                            }
                            else
                                strMax = ((Double)CurrValue).ToString(format);
                        }
                        else if (CurrValue == dblMax)
                        {
                            switch (strCurrSymbol)
                            {
                                default:
                                    MaxFound = false;
                                    break;
                                case "<":
                                    if (MaxSymbolStatus == "ND")
                                        MaxFound = true;
                                    break;
                                case "ND":
                                    MaxFound = false;
                                    break;
                                case ">":
                                    if (MaxSymbolStatus != ">")
                                        MaxFound = true;
                                    break;
                                case "":
                                    if (MaxSymbolStatus == "<")
                                        MaxFound = true;
                                    break;
                                case "TNTC":
                                    if (MaxSymbolStatus == ">")
                                        MaxFound = true;
                                    break;
                            }
                            if (MaxFound)
                            {
                                if (strCurrSymbol == "ND")
                                    strMax = "ND";
                                else
                                    strMax = strCurrSymbol + ((Double)CurrValue).ToString(format);
                                MaxSymbolStatus = strCurrSymbol;
                                dblMax = CurrValue;
                            }
                        }
                    }
                }

            }
            if (dblMax != null)
                return strMax;
            return "";
        }
        public static object Min(object range, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string strFormat)
        {
            // Cast CR to Array
            var array = Helper.ConvertToArray(range);
            double? dblMin = null;
            double? CurrValue = null;
            string strMin = "";
            string strCurrSymbol = "";
            string MinSymbolStatus = "";
            double temp;
            bool MinFound = false;


            foreach (var C in array)
            {
                string strTemp = C.ToString();
                if (Helper.IsNumeric(strTemp))
                {
                    CurrValue = Helper.TryParseDouble(strTemp);
                    if (dblMin == null || CurrValue < dblMin)
                    {
                        dblMin = CurrValue;
                        strMin = ((Double)CurrValue).ToString(strFormat);
                    }
                }
                else
                {
                    strCurrSymbol = GetSymbol(strTemp);
                    CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                    if (CurrValue != null)
                    {
                        if (dblMin == null || CurrValue < dblMin)
                        {
                            dblMin = CurrValue;
                            if (cascadeRule > 0)
                            {
                                if (strCurrSymbol == "ND")
                                    strMin = "ND";
                                else
                                    strMin = strCurrSymbol + ((Double)CurrValue).ToString(strFormat);
                                MinSymbolStatus = strCurrSymbol;
                            }
                            else
                                strMin = ((Double)CurrValue).ToString(strFormat);
                        }
                        else if (CurrValue == dblMin)
                        {
                            switch (strCurrSymbol)
                            {
                                default:
                                    MinFound = false;
                                    break;
                                case "<":
                                    if (MinSymbolStatus != "ND")
                                        MinFound = true;
                                    break;
                                case "ND":
                                    MinFound = true;
                                    break;
                                case ">":
                                    MinFound = false;
                                    break;
                                case "":
                                    if (MinSymbolStatus == ">")
                                        MinFound = true;
                                    break;
                                case "TNTC":
                                    if (MinSymbolStatus == ">" && strCurrSymbol != "TNTC")
                                        MinFound = true;
                                    break;
                            }
                            if (MinFound)
                            {
                                if (strCurrSymbol == "ND")
                                    strMin = "ND";
                                else
                                    strMin = strCurrSymbol + ((Double)CurrValue).ToString(strFormat);
                                MinSymbolStatus = strCurrSymbol;
                                dblMin = CurrValue;
                            }
                        }
                    }
                }

            }
            if (dblMin != null)
                return strMin;
            return "";
        }
        public static object Count(object range, EnumQualifierRule qualifierRule, string strFormat)
        {
            long NOS = 0;
            string strTemp;
            var array = Helper.ConvertToArray(range);

            foreach (var C in array)
            {
                strTemp = C.ToString();

                if (!string.IsNullOrEmpty(strTemp) && Helper.TryParseDouble(GetValue(strTemp, qualifierRule)) != null)
                    NOS++;
            }
            if (NOS > 0)
                return NOS.ToString(strFormat);
            return "";
           
        }
        public static object First(object range, EnumQualifierRule qualifierRule, string strFormat)
        {
            var array = Helper.ConvertToArray(range);
            string strTemp;
            double? CurrValue = null;
            string strCurrSymbol = "";


            foreach (var C in array)
            {
                strTemp = C.ToString();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    if (Helper.IsNumeric(strTemp))
                    {
                        CurrValue = Helper.TryParseDouble(strTemp);
                        if (CurrValue != null)
                            return ((double)CurrValue).ToString(strFormat);
                    }
                    else
                    {
                        strCurrSymbol = GetSymbol(strTemp);
                        CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                        if (CurrValue != null)
                            return strTemp;
                    }
                }
            }
            return "";
          
        }

        public static object Last(object range, EnumQualifierRule qualifierRule, string strFormat)
        {
            var array = Helper.ConvertToArray(range);
            string strTemp;
            double? CurrValue = null;
            string strCurrSymbol = "";
            string strLast = "";

            foreach (var C in array)
            {
                strTemp = C.ToString();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    if (Helper.IsNumeric(strTemp))
                    {
                        CurrValue = Helper.TryParseDouble(strTemp);
                        if (CurrValue != null)
                            strLast = ((double)CurrValue).ToString(strFormat);
                    }
                    else
                    {
                        strCurrSymbol = GetSymbol(strTemp);
                        CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                        if (CurrValue != null)
                            strLast = strTemp;
                    }
                }
            }
            return strLast;
        }
        public static object GeometricMeans(object CR, string strStat, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string strFormat)
        {
            var array = Helper.ConvertToArray(CR);
            double? CurrValue = null;
            string strCurrSymbol = "";

            long NOS = 0;
            double dblTemp = 0;
            double dblGeoSum = 0;
            double dblValPlusOne = 0;
            double dblGeoSumPlusOne = 0;
            bool boolZeroFound = false;
            int intGeoCnt = 0;
            int NumOfDQ = 3;
            int[] intDQCount = new int[QualifierArray.Length];
            int intGType = 0;


            switch (strStat.ToUpper())
            {
                case "GM0":
                    intGType = 0;
                    break;
                case "GM1":
                    intGType = 1;
                    break;
                case "GM2":
                    intGType = 2;
                    break;
                case "GM3":
                    intGType = 3;
                    break;
            }
            foreach (var C in array)
            {
                string strTemp = C.ToString();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    if (Helper.IsNumeric(strTemp))
                        CurrValue = Helper.TryParseDouble(strTemp);
                    else
                    {
                        strCurrSymbol = GetSymbol(strTemp);
                        CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                        if (CurrValue != null)
                        {
                            if (!string.IsNullOrEmpty(strCurrSymbol))
                            {
                                for (int i = 0; i < NumOfDQ; i++)
                                {
                                    if (strCurrSymbol == Qualifiers.QualifierArray[i])
                                    {
                                        intDQCount[i] = intDQCount[i] + 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (CurrValue != null)
                    {
                        dblTemp = (double)CurrValue;
                        switch (intGType)
                        {
                            default: //    'Ignore zeros and negatives
                            case 0:
                                if (dblTemp > 0)
                                {
                                    dblTemp = Math.Log(dblTemp) / Math.Log(10);
                                    dblGeoSum += dblTemp;
                                    intGeoCnt++;
                                }
                                break;
                            case 1: //      'Convert zeros (or negative numbers) to 1
                                if (dblTemp <= 0)
                                    dblTemp = Math.Log(1) / Math.Log(10);
                                else
                                    dblTemp = Math.Log(dblTemp) / Math.Log(10);
                                dblGeoSum += dblTemp;
                                intGeoCnt++;
                                break;
                            case 2://       'add 1 to all, and subtract 1 from result.
                                dblValPlusOne = dblTemp + 1;
                                if (dblTemp <= 0)
                                {
                                    dblTemp = Math.Log(1) / Math.Log(10);
                                    dblValPlusOne = dblTemp;
                                    boolZeroFound = true;
                                }
                                else
                                {
                                    dblTemp = Math.Log((double)CurrValue) / Math.Log(10);
                                    dblValPlusOne = Math.Log(dblValPlusOne) / Math.Log(10);
                                }
                                dblGeoSum += dblTemp;
                                dblGeoSumPlusOne += dblValPlusOne;
                                intGeoCnt++;
                                break;
                        }
                    }


                }
            }
            if (intGeoCnt > 0)
            {
                if (intGType == 2 && boolZeroFound)
                    strStat = Math.Pow(10, (dblGeoSumPlusOne / intGeoCnt)).ToString(strFormat);
                else
                    strStat = Math.Pow(10, (dblGeoSum / intGeoCnt)).ToString(strFormat);
                return Qualifiers.Format(Helper.TryParseDouble(strStat), intDQCount, intGeoCnt, cascadeRule, strFormat);
            }
            return "";
            
        }
        public static object Percentile(object CR, string strStat, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string strFormat, string strSortRule)
        {
            object[,] array = (object[,])Sort(CR, qualifierRule, strSortRule);
            double index = 0;
            if (array == null)
                return null;
            int Count = (array.GetUpperBound(0) + 1);
            int SlotToGet = 0;
            if (strStat.Length <= 8)
                return EnumErrors.ERR_INVALID_VALUE.ToString();
            int.TryParse(strStat.Substring(4, 1), out int intPercType);
            double? dblPercent = Helper.TryParseDouble(strStat.Substring(6, 2));

            if (Count > 0 && dblPercent != null)
            {
                double dblTemp = (Count * (double)dblPercent / 100);
                if (dblTemp == (int)dblTemp)
                    return array[(int)index, 0];
                //else
                //    return DQIntAvg(OutArray(SlotToGet), OutArray(SlotToGet + 1), CascadeRule, MDLRule, strFormat);
            }

            return "";


            /*
            Dim intCellCount As Long
            Dim Count As Long
            Dim C As Range
            Dim strTemp As String
            Dim Divisor As Long
            Dim boolNeedToAvg As Boolean
            Dim dblPercent As Double
            Dim intPercType As Integer
            Dim SlotToGet As Integer
            Dim dblTemp As Double



            intCellCount = CR.Rows.Count * CR.Columns.Count

            ReDim SourceArray(intCellCount) As String
            ReDim OutArray(intCellCount) As String


            intPercType = Mid(strStat, 5, 1)
            dblPercent = Mid(strStat, 7, 2)




            For Each C In CR
                strTemp = C.Value
                If strTemp <> "" Then
                    Count = Count + 1
                    SourceArray(Count) = strTemp
                End If
            Next C
            Call SortArray(SourceArray, OutArray, 1)


             Divisor = Count

             If Count > 0 Then
                    dblTemp = (Count * dblPercent / 100)
                    boolNeedToAvg = False
                    If dblTemp = CLng(dblTemp) Then
                        SlotToGet = CLng(dblTemp)
                        DQPERCENTILE = OutArray(SlotToGet)
                        DQPERCENTILE = DQ(DQPERCENTILE) & Format(DQPERCENTILE, strFormat)
                    Else
                        If intPercType = 1 Then
                            SlotToGet = CLng((Count * dblPercent / 100) + 0.5)
                            DQPERCENTILE = OutArray(SlotToGet)
                            DQPERCENTILE = DQ(DQPERCENTILE) & Format(DQPERCENTILE, strFormat)
                        Else
                            SlotToGet = Fix(dblTemp)
                            boolNeedToAvg = True
                            DQPERCENTILE = DQIntAvg(OutArray(SlotToGet), OutArray(SlotToGet + 1), CascadeRule, MDLRule, strFormat)
                        End If
                    End If
            Else
                DQPERCENTILE = ""
            End If

             */
        }
        public static object CCGT(object CR, string strStat, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string strFormat)
        {
            var array = Helper.ConvertToArray(CR);
            double? CurrValue = null;
            double dblTestVal = 0;
            bool boolCounted = false;
            bool boolLastTrue = false;
            string strTemp = "";
            long NOS = 0;

            foreach (var C in array)
            {
                strTemp = C.ToString();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    if (Helper.IsNumeric(strTemp))
                        CurrValue = Helper.TryParseDouble(strTemp);
                    else
                        CurrValue = Helper.TryParseDouble(GetValue(strTemp, qualifierRule));
                }
                if (CurrValue != null)
                {
                    if (CurrValue > dblTestVal)
                    {
                        if (boolLastTrue)
                        {
                            if (boolCounted)
                                NOS++;
                            boolCounted = true;
                        }
                        else
                            boolLastTrue = true;
                    }
                    else
                        boolLastTrue = false;
                }
            }
            if (NOS > 0)
                return NOS.ToString(strFormat);
            return "";

        }
        public static object Median(object CR, EnumCascadeRule cascadeRule, EnumQualifierRule qualifierRule, string strFormat, string strSortRule)
        {
            object[,] array = (object[,])Sort(CR, qualifierRule, strSortRule);
            double index = 0;
            if (array == null)
                return null;
            int numValues = (array.GetUpperBound(0) + 1);
            if (numValues % 2 != 0) // odd
                index = Math.Round((double)numValues / 2, 0) + 1;
            else
                index = Math.Round((double)numValues / 2, 0);
            return array[(int)index, 1];  //Since it is all ints the number will be rounded down if Length is odd.

        }
        

    }
}
