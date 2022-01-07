using ONE.Operations.Spreadsheet;
using System;

namespace ONE.Utilities
{
    public class GroupDates
    {
        public DateTime GroupStartDate { get; set; }
        public DateTime GroupEndDate { get; set; }

        public GroupDates(string strGrouping, int? Offset, DateTime StartDate, DateTime EndDate)
        {
            // strGrouping - "M" - Month, "D" - Day,"R" - Report date range,
            // Offset - number of groups from startdate of report, if = -8888 means get grouping for date in enddate
            // IE GAVG(2,1,"M") - Offset 1, grouping M, get avg for 1st month
            // GAVG(2,B3,"D") - Get avg for Day in B3, Offset will be set to -8888, startDate to B3 and strGrouping to D
            // GAVG(2,B3,"M") - Get avg for Month in B3, Offset = -8888
            // GAVG(2,B3,B4) - Get avg for dates B3 to B4, strGrouping will be set to "R" and StartDate and End Date will
            // set to dates in B3 and B4.


            DateTime SummerStart;
            DateTime FallStart;
            DateTime WinterStart;
            DateTime SpringStart;
            DateTime CurrDate;
            var strDesc = default(string);
            int intWeekDef;
            int dYear;
            string Quarter;
            int ColonPos;
            string StartHour;
            string EndHour;
            DateTime TempDate;
            if (string.IsNullOrEmpty(strGrouping))
                strGrouping = "R";

            StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day);
            EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day);




            switch (strGrouping.ToUpper() ?? "")
            {
                case "D":
                    {
                        if (Offset != null)
                        {
                            GroupStartDate = StartDate.AddDays((int)Offset - 1);
                            GroupEndDate = GroupStartDate.AddDays(1);
                            if (GroupStartDate > EndDate)
                            {
                                GroupStartDate = DateTime.MinValue;
                                GroupEndDate = DateTime.MinValue;
                            }
                        }
                        else
                        {
                            GroupStartDate = StartDate;
                            GroupEndDate = GroupStartDate.AddDays(1);
                        }

                        break;
                    }

                case "H":
                    {
                        if (Offset != null)
                        {
                            GroupStartDate = StartDate.AddHours((int)Offset - 1);
                            GroupEndDate = GroupStartDate.AddHours(1);    // - OneSec '0.00001
                            if (GroupStartDate >= EndDate.AddDays(1))
                            {
                                GroupStartDate = DateTime.MinValue;
                                GroupEndDate = DateTime.MinValue;
                            }
                        }
                        else
                        {
                            GroupStartDate = StartDate;
                            GroupEndDate = GroupStartDate.AddHours(1);  // - OneSec '0.00001
                            if (GroupStartDate >= EndDate.AddDays(1))
                            {
                                GroupStartDate = DateTime.MinValue;
                                GroupEndDate = DateTime.MinValue;
                            }
                        }

                        break;
                    }

                case "15M":
                    {
                        if (Offset != null)
                        {
                            GroupStartDate = StartDate.AddMinutes(((int)Offset - 1));
                            GroupEndDate = GroupStartDate.AddMinutes(15);    // - 1 / 24 / 3600
                            if (GroupStartDate >= EndDate.AddDays(1))
                            {
                                GroupStartDate = DateTime.MinValue;
                                GroupEndDate = DateTime.MinValue;
                            }
                        }
                        else
                        {
                            GroupStartDate = StartDate;
                            GroupEndDate = GroupStartDate.AddMinutes(15);   // - OneSec '0.00001
                            if (GroupStartDate >= EndDate.AddDays(1))
                            {
                                GroupStartDate = DateTime.MinValue;
                                GroupEndDate = DateTime.MinValue;
                            }
                        }

                        break;
                    }

                case "M":
                case "MM":
                    {
                        if (Offset != null)
                        {
                            GroupStartDate = StartDate.AddMonths((int)Offset - 1);
                            GroupStartDate = new DateTime(GroupStartDate.Year, GroupStartDate.Month, 1);
                            GroupEndDate = GroupStartDate.AddMonths(1);
                        }
                        else
                        {
                            GroupStartDate = StartDate;
                            GroupStartDate = new DateTime(GroupStartDate.Year, GroupStartDate.Month, 1);
                            GroupEndDate = GroupStartDate.AddMonths(1);
                        }

                        break;
                    }

                case "R":
                case "REPORT":
                    {
                        GroupStartDate = StartDate;
                        GroupEndDate = EndDate.AddDays(1);
                        break;
                    }

                case "XX":
                    {
                        GroupStartDate = StartDate;
                        GroupEndDate = EndDate;
                        break;
                    }

                case "A":
                    {
                        GroupStartDate = new DateTime(1970, 1, 1);
                        GroupEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
                        break;
                    }

                case "Y":
                case "YYYY":
                    {
                        if (Offset != null)
                        {
                            dYear = StartDate.Year;
                            dYear = dYear + (int)Offset - 1;
                            GroupStartDate = new DateTime(dYear, 1, 1);
                            GroupEndDate = new DateTime(dYear, 12, 31).AddDays(1);
                        }
                        else
                        {
                            dYear = StartDate.Year;
                            GroupStartDate = new DateTime(dYear, 1, 1);
                            GroupEndDate = new DateTime(dYear, 12, 31).AddDays(1);
                        }

                        break;
                    }

                case "YTED":
                case "YTD":
                    {
                        dYear = StartDate.Year;
                        dYear = dYear + (int)Offset - 1;
                        GroupStartDate = new DateTime(dYear, 1, 1);
                        GroupEndDate = EndDate.AddYears((int)Offset - 1).AddDays(1); // dYear, Month(EndDate), Day(EndDate))
                                                                            // checks for leap year.
                                                                            // if the EndDate of the Report is 2/28/01 and we subtract one year
                                                                            // we get 2/28/00 but we want 2/29/02, this corrects
                        if (EndDate.Month == 2)
                        {
                            if (EndDate == Helper.LastDOMValue(EndDate))
                            {
                                GroupEndDate = new DateTime(GroupEndDate.Year, 3, 1);
                            }
                        }

                        break;
                    }

                case "YTBSD":
                case "YTSD":      // Year To Before Start Date (one day before Start Date)
                    {
                        dYear = StartDate.Year;
                        dYear = dYear + (int)Offset - 1;
                        GroupStartDate = new DateTime(dYear, 1, 1);
                        GroupEndDate = StartDate.AddYears((int)Offset - 1); // + 1 ' dYear, Month(EndDate), Day(EndDate))
                                                                   // checks for leap year.
                                                                   // if the StartDate of the Report is 2/28/01 and we subtract one year
                                                                   // we get 2/28/00 but we want 2/29/02, this corrects
                        if (StartDate.Month == 2)
                        {
                            if (StartDate == Helper.LastDOMValue(StartDate))
                            {
                                GroupEndDate = new DateTime(GroupEndDate.Year, 3, 1);
                            }
                        }

                        break;
                    }

                case "S":
                case "SEASON":
                    {
                        SummerStart = new DateTime(StartDate.Year, 6, 20);
                        FallStart = new DateTime(StartDate.Year, 9, 20);
                        SpringStart = new DateTime(StartDate.Year, 3, 21);
                        if (StartDate < SpringStart)
                        {
                            WinterStart = new DateTime(StartDate.Year - 1, 12, 20);
                        }
                        else
                        {
                            WinterStart = new DateTime(StartDate.Year, 12, 20);
                        }

                        GroupStartDate = StartDate;
                        if (GroupStartDate >= SpringStart && GroupStartDate < SummerStart)
                        {
                            GroupStartDate = SpringStart.AddDays(8);
                        }
                        else if (GroupStartDate >= SummerStart && GroupStartDate < FallStart)
                        {
                            GroupStartDate = SummerStart.AddDays(8);
                        }
                        else if (GroupStartDate >= FallStart && GroupStartDate < WinterStart)
                        {
                            GroupStartDate = FallStart.AddDays(8);
                        }
                        else
                        {
                            GroupStartDate = WinterStart.AddDays(8);
                        }

                        if (Offset != null)
                        {
                            CurrDate = GroupStartDate.AddDays(((int)Offset - 1) * 92);
                        }
                        else
                        {
                            CurrDate = GroupStartDate;
                        }
                        Season season = new Season(CurrDate);
                        GroupStartDate = season.StartDate;
                        GroupEndDate = season.EndDate;
                        strDesc = season.Description;
                        GroupEndDate = GroupEndDate.AddDays(1);
                        break;
                    }

                case "Q":
                    {
                        if (Offset != null)
                        {
                            GroupStartDate = StartDate.AddMonths(3 * ((int)Offset - 1));
                        }
                        else
                        {
                            GroupStartDate = StartDate;
                        }

                        Quarter = GroupStartDate.ToStringEx("q");
                        dYear = GroupStartDate.Year;
                        switch (Quarter ?? "")
                        {
                            case "1":
                                {
                                    GroupStartDate = new DateTime(dYear, 1, 1);
                                    GroupEndDate = new DateTime(dYear, 3, 31);
                                    break;
                                }

                            case "2":
                                {
                                    GroupStartDate = new DateTime(dYear, 4, 1);
                                    GroupEndDate = new DateTime(dYear, 6, 30);
                                    break;
                                }

                            case "3":
                                {
                                    GroupStartDate = new DateTime(dYear, 7, 1);
                                    GroupEndDate = new DateTime(dYear, 9, 30);
                                    break;
                                }

                            case "4":
                                {
                                    GroupStartDate = new DateTime(dYear, 10, 1);
                                    GroupEndDate = new DateTime(dYear, 12, 31);
                                    break;
                                }
                        }

                        GroupEndDate = GroupEndDate.AddDays(1);
                        break;
                    }

                case "SA":
                case "2Y":    // semi annual
                    {
                        dYear = StartDate.Year;
                        if (StartDate > new DateTime(dYear, 6, 30))
                        {
                            GroupStartDate = new DateTime(dYear, 7, 1);
                        }
                        else
                        {
                            GroupStartDate = new DateTime(dYear, 1, 1);
                        }

                        if (Offset != -8888)
                        {
                            GroupStartDate = GroupStartDate.AddMonths(6 * ((int)Offset - 1));
                            // gSD = DateAdd("d", 182 * (Offset - 1), gSD)
                        }

                        dYear = GroupStartDate.Year;
                        if (GroupStartDate > new DateTime(dYear, 6, 30))
                        {
                            GroupStartDate = new DateTime(dYear, 7, 1);
                            GroupEndDate = new DateTime(dYear, 12, 31);
                        }
                        else
                        {
                            GroupStartDate = new DateTime(dYear, 1, 1);
                            GroupEndDate = new DateTime(dYear, 6, 30);
                        }

                        GroupEndDate = GroupEndDate.AddDays(1);
                        break;
                    }

                default:
                    {
                        if (strGrouping.StartsWith("W"))
                        {
                            if (Offset == null)
                                Offset = 1;
                            if (strGrouping.Substring(1, 1).ToUpper() == "O")
                            {
                                int.TryParse(strGrouping.Substring(2, 1), out intWeekDef);
                                WeekDates weekDates = new WeekDates((int)Offset, intWeekDef, StartDate);

                                //GetWeekDates(Offset, gSD, ged, intWeekDef, true, StartDate, EndDate);
                                // If gSD > EndDate Then
                                // gSD = 0
                                // gED = 0
                                // End If
                                GroupStartDate = weekDates.StartDate;
                                GroupEndDate = weekDates.EndDate.AddDays(1);
                            }
                            else if (strGrouping.Substring(1, 1).ToUpper() == "S")
                            {
                                int.TryParse(strGrouping.Substring(2, 1), out intWeekDef);
                                WeekDates weekDates = new WeekDates((int)Offset, intWeekDef, StartDate);
                                GroupStartDate = weekDates.StartDate;
                                GroupEndDate = weekDates.EndDate.AddDays(1);
                                if (GroupStartDate > EndDate || GroupEndDate < StartDate)
                                {
                                    GroupStartDate = DateTime.MinValue;
                                    GroupEndDate = DateTime.MinValue;
                                }
                            }
                            else
                            {
                                int.TryParse(strGrouping.Substring(1, 1), out intWeekDef);
                                WeekDates weekDates = new WeekDates((int)Offset, intWeekDef, StartDate);
                                GroupStartDate = weekDates.StartDate;
                                GroupEndDate = weekDates.EndDate.AddDays(1);
                                // check if group end date is beyond enddate of report
                                // if it is set gED to less than gSD date so
                                // queries return no data
                                if (GroupEndDate > EndDate.AddDays(1))
                                {
                                    GroupStartDate = DateTime.MinValue;
                                    GroupEndDate = DateTime.MinValue;
                                    // gED = gSD - 1
                                }
                            }
                        }
                        else if (strGrouping.StartsWith("H"))
                        {
                            // Partial days H0:H23
                            ColonPos = strGrouping.IndexOf(':', 0);
                            if (ColonPos > 1)
                            {
                                StartHour = strGrouping.Substring(1, ColonPos - 1);
                                EndHour = strGrouping.Substring(ColonPos + 2);
                                if (Helper.IsNumeric(StartHour) & Helper.IsNumeric(EndHour))
                                {
                                    TempDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day);
                                    if (Offset != null)
                                    {
                                        TempDate = TempDate.AddDays(-1);
                                    }
                                    
                                    Double.TryParse(StartHour, out double dStartHour);
                                    GroupStartDate = TempDate.AddHours(dStartHour);
                                    Double.TryParse(EndHour, out double dEndHour);

                                    GroupEndDate = TempDate.AddHours(dEndHour + 1);
                                    if (GroupStartDate >= EndDate.AddDays(1))
                                    {
                                        GroupStartDate = DateTime.MinValue;
                                        GroupEndDate = DateTime.MinValue;
                                    }
                                }
                            }
                        }
                        else if (strGrouping.StartsWith("MOV"))
                        {
                            int.TryParse(strGrouping.Substring(3), out intWeekDef);
                            if (Offset != -8888)
                            {
                                GroupEndDate = StartDate.AddDays((int)Offset - 1);
                                GroupStartDate = GroupEndDate.AddDays(-intWeekDef + 1);
                                GroupEndDate = GroupEndDate.AddDays(1);
                            }
                            else
                            {
                                GroupStartDate = StartDate;
                                GroupEndDate = GroupStartDate.AddDays(-intWeekDef + 1);
                            }

                            if (GroupEndDate > EndDate.AddDays(1))
                            {
                                GroupStartDate = DateTime.MinValue;
                                GroupEndDate = DateTime.MinValue;
                            }
                        }
                        else if (strGrouping.StartsWith("MA"))
                        {
                            int.TryParse(strGrouping.Substring(2), out intWeekDef);
                            if (Offset != null)
                            {
                                GroupStartDate = StartDate.AddMonths(-1);
                            }
                            else
                            {
                                GroupStartDate = StartDate;
                            }

                            GroupStartDate = new DateTime(GroupStartDate.Year, GroupStartDate.Month, 1);
                            GroupEndDate = GroupStartDate.AddMonths(1);   // - 1
                            GroupStartDate = GroupStartDate.AddMonths(-(intWeekDef - 1));
                        }

                        break;
                    }
            }
        }

    }
}

/*
Sub GetGroupDates(strGrouping As String, Offset As Integer, StartDate As Date, EndDate As Date, gSD As Date, ged As Date)
'strGrouping - "M" - Month, "D" - Day,"R" - Report date range,
'Offset - number of groups from startdate of report, if = -8888 means get grouping for date in enddate
'IE GAVG(2,1,"M") - Offset 1, grouping M, get avg for 1st month
'   GAVG(2,B3,"D") - Get avg for Day in B3, Offset will be set to -8888, startDate to B3 and strGrouping to D
'   GAVG(2,B3,"M") - Get avg for Month in B3, Offset = -8888
'   GAVG(2,B3,B4) - Get avg for dates B3 to B4, strGrouping will be set to "R" and StartDate and End Date will
'                   set to dates in B3 and B4.


Dim SummerStart As Date
Dim FallStart As Date
Dim WinterStart As Date
Dim SpringStart As Date
Dim CurrDate As Date
Dim strDesc As String
Dim intWeekDef As Integer
Dim OneSec As Double
Dim dYear As Integer
Dim Quarter As String
Dim ColonPos As Long
Dim SH As String
Dim EH As String
Dim TempDate As Date

OneSec = 1 / 24 / 3600
If strGrouping = "" Then strGrouping = "R"

Select Case UCase$(strGrouping)


Case "D"
If Offset <> -8888 Then
    gSD = DateAdd("d", Offset - 1, StartDate)
    ged = gSD + 1
    If gSD > EndDate Then
        gSD = 0
        ged = 0
    End If
Else
    gSD = StartDate
    ged = gSD + 1
End If
Case "H"
If Offset <> -8888 Then
    gSD = DateAdd("h", Offset - 1, StartDate)
    ged = DateAdd("h", 1, gSD)    '- OneSec '0.00001
    If gSD >= EndDate + 1 Then
        gSD = 0
        ged = 0
    End If
Else
    gSD = StartDate
    ged = DateAdd("h", 1, gSD)    '- OneSec '0.00001
    If gSD >= EndDate + 1 Then
        gSD = 0
        ged = 0
    End If
End If

Case "15M"
If Offset <> -8888 Then
    gSD = DateAdd("n", (Offset - 1) * 15, StartDate)
    ged = DateAdd("n", 15, gSD)    ' - 1 / 24 / 3600
    If gSD >= EndDate + 1 Then
        gSD = 0
        ged = 0
    End If
Else
    gSD = StartDate
    ged = DateAdd("n", 15, gSD)    '- OneSec '0.00001
    If gSD >= EndDate + 1 Then
        gSD = 0
        ged = 0
    End If
End If

Case "M", "MM"
If Offset <> -8888 Then
    gSD = DateAdd("m", Offset - 1, StartDate)
    gSD = DateSerial(year(gSD), Month(gSD), 1)
    ged = DateAdd("m", 1, gSD)
Else
    gSD = StartDate
    gSD = DateSerial(year(gSD), Month(gSD), 1)
    ged = DateAdd("m", 1, gSD)
End If
Case "R", "REPORT"
gSD = StartDate
ged = EndDate + 1
Case "XX"
gSD = StartDate
ged = EndDate
Case "A"
gSD = DateSerial(1970, 1, 1)
ged = DateSerial(year(Date), Month(Date), Day(Date) + 1)
Case "Y", "YYYY"
If Offset <> -8888 Then
    dYear = year(StartDate)
    dYear = dYear + Offset - 1
    gSD = DateSerial(dYear, 1, 1)
    ged = DateSerial(dYear, 12, 31) + 1
Else
    dYear = year(StartDate)
    gSD = DateSerial(dYear, 1, 1)
    ged = DateSerial(dYear, 12, 31) + 1
End If
Case "YTED", "YTD"
dYear = year(StartDate)
dYear = dYear + Offset - 1
gSD = DateSerial(dYear, 1, 1)
ged = DateAdd("yyyy", Offset - 1, EndDate) + 1 ' dYear, Month(EndDate), Day(EndDate))
'checks for leap year.
'if the EndDate of the Report is 2/28/01 and we subtract one year
'we get 2/28/00 but we want 2/29/02, this corrects
If Month(EndDate) = 2 Then
    If EndDate = LastDOMValue(EndDate) Then
        ged = DateSerial(year(ged), 3, 1)
        'gED = LastDOMValue(gED)
    End If
End If


Case "YTBSD", "YTSD"      'Year To Before Start Date (one day before Start Date)
dYear = year(StartDate)
dYear = dYear + Offset - 1
gSD = DateSerial(dYear, 1, 1)
ged = DateAdd("yyyy", Offset - 1, StartDate) '+ 1 ' dYear, Month(EndDate), Day(EndDate))
'checks for leap year.
'if the StartDate of the Report is 2/28/01 and we subtract one year
'we get 2/28/00 but we want 2/29/02, this corrects
If Month(StartDate) = 2 Then
    If StartDate = LastDOMValue(StartDate) Then
        ged = DateSerial(year(ged), 3, 1)
        'gED = LastDOMValue(gED)
    End If
End If




Case "S", "SEASON"
SummerStart = DateSerial(year(StartDate), 6, 20)
FallStart = DateSerial(year(StartDate), 9, 20)
SpringStart = DateSerial(year(StartDate), 3, 21)
If StartDate < SpringStart Then
    WinterStart = DateSerial(year(StartDate) - 1, 12, 20)
Else
    WinterStart = DateSerial(year(StartDate), 12, 20)
End If
gSD = StartDate
If gSD >= SpringStart And gSD < SummerStart Then
    gSD = SpringStart + 8
ElseIf gSD >= SummerStart And gSD < FallStart Then
    gSD = SummerStart + 8
ElseIf gSD >= FallStart And gSD < WinterStart Then
    gSD = FallStart + 8
Else
    gSD = WinterStart + 8
End If

If Offset <> -8888 Then
    CurrDate = DateAdd("d", (Offset - 1) * 92, gSD)
Else
    CurrDate = gSD
End If
Call GetSeasonDates(CurrDate, gSD, ged, strDesc)
ged = ged + 1

Case "Q"
If Offset <> -8888 Then
    gSD = DateAdd("q", Offset - 1, StartDate)
Else
    gSD = StartDate
End If
Quarter = format(gSD, "q")
dYear = year(gSD)
Select Case Quarter
    Case 1
        gSD = DateSerial(dYear, 1, 1)
        ged = DateSerial(dYear, 3, 31)

    Case 2
        gSD = DateSerial(dYear, 4, 1)
        ged = DateSerial(dYear, 6, 30)

    Case 3
        gSD = DateSerial(dYear, 7, 1)
        ged = DateSerial(dYear, 9, 30)

    Case 4
        gSD = DateSerial(dYear, 10, 1)
        ged = DateSerial(dYear, 12, 31)

End Select
ged = ged + 1

Case "SA", "2Y"    'semi annual
dYear = year(StartDate)
If StartDate > DateSerial(dYear, 6, 30) Then
    gSD = DateSerial(dYear, 7, 1)
Else
    gSD = DateSerial(dYear, 1, 1)
End If
If Offset <> -8888 Then
    gSD = DateAdd("m", 6 * (Offset - 1), gSD)
    'gSD = DateAdd("d", 182 * (Offset - 1), gSD)
End If
dYear = year(gSD)
If gSD > DateSerial(dYear, 6, 30) Then
    gSD = DateSerial(dYear, 7, 1)
    ged = DateSerial(dYear, 12, 31)
Else
    gSD = DateSerial(dYear, 1, 1)
    ged = DateSerial(dYear, 6, 30)
End If
ged = ged + 1

Case Else
If strGrouping Like "W*" Then
    If Offset = -8888 Then Offset = 1

    If UCase(Mid$(strGrouping, 2, 1)) = "O" Then
        intWeekDef = Mid$(strGrouping, 3)
        Call GetWeekDates(Offset, gSD, ged, intWeekDef, True, StartDate, EndDate)
'                If gSD > EndDate Then
'                    gSD = 0
'                    gED = 0
'                End If
        ged = ged + 1
    ElseIf UCase(Mid$(strGrouping, 2, 1)) = "S" Then
        intWeekDef = Mid$(strGrouping, 3)
        Call GetWeekDates(Offset, gSD, ged, intWeekDef, True, StartDate, EndDate)
        ged = ged + 1
        If gSD > EndDate Or ged < StartDate Then
            gSD = 0
            ged = 0
        End If
    Else
        intWeekDef = Mid$(strGrouping, 2)
        Call GetWeekDates(Offset, gSD, ged, intWeekDef, False, StartDate, EndDate)
        ged = ged + 1
        'check if group end date is beyond enddate of report
        'if it is set gED to less than gSD date so
        'queries return no data
        If ged > EndDate + 1 Then
            gSD = 0
            ged = 0
            'gED = gSD - 1
        End If
    End If
ElseIf strGrouping Like "H*" Then
    'Partial days H0:H23
    ColonPos = InStr(strGrouping, ":")
    If ColonPos > 2 Then
        SH = Mid$(strGrouping, 2, ColonPos - 2)
        EH = Mid$(strGrouping, ColonPos + 1)
        If IsNumeric(SH) And IsNumeric(EH) Then
            If Offset <> -8888 Then
                TempDate = DateAdd("d", Offset - 1, StartDate)
            Else
                TempDate = StartDate
            End If
            gSD = DateAdd("h", CInt(SH), TempDate)
            ged = DateAdd("h", CInt(EH) + 1, TempDate)
            If gSD >= EndDate + 1 Then
                gSD = 0
                ged = 0
            End If
        End If
    End If


ElseIf strGrouping Like "MOV*" Then
    intWeekDef = Mid$(strGrouping, 4)
    If Offset <> -8888 Then
        ged = DateAdd("d", Offset - 1, StartDate)
        gSD = ged - intWeekDef + 1
        ged = ged + 1
    Else
        gSD = StartDate
        ged = gSD + intWeekDef - 1
    End If
    If ged > EndDate + 1 Then
        gSD = 0
        ged = 0
    End If


ElseIf strGrouping Like "MA*" Then
    intWeekDef = Mid$(strGrouping, 3)
    If Offset <> -8888 Then
        gSD = DateAdd("m", Offset - 1, StartDate)
    Else
        gSD = StartDate
    End If
    gSD = DateSerial(year(gSD), Month(gSD), 1)
    ged = DateAdd("m", 1, gSD)   '- 1
    gSD = DateAdd("m", 0 - (intWeekDef - 1), gSD)
End If

End Select


End Sub
 */

