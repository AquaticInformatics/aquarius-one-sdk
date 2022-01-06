using ONE.Operations.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Utilities
{
    public class WeekDates
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CalcDate { get; set; }

        public WeekDates(int offset, int weekDef, DateTime startDate)
        {
                // 1   Monday - Sunday
                // 2   Tuesday - Monday
                // 3   Wednesday - Tuesday
                // 4   Thursday - Wednesday
                // 5   Friday - Thursday
                // 6   Saturday - Friday
                // 7   Sunday - Saturday
                // 
                // 8   1st 7th
                // 8th 14th
                // 15th    21st
                // 22nd    Last Day of Month
                // 
                // 9   1st 7th
                // 8th 14th
                // 15th    21st
                // 22nd    28th
                // (Days greater than the 28th are ignored.)
                // 
                // 10  1st 7th
                // 8th 14th
                // 15th    21st
                // 22nd    28th
                // 29th    Last Day of Month
                // 
                // 11  Monday (or first of the month)  Sunday
                // 12  Tuesday (or first of the month) Monday
                // 13  Wednesday (or first of the month)   Tuesday
                // 14  Thursday (or first of the month)    Wednesday
                // 15  Friday (or first of the month)  Thursday
                // 16  Saturday (or first of the month)    Friday
                // 17  Sunday (or first of the month)  Saturday
                // 
                // 18  Sunday Saturday
                // Always calculates Sunday to Saturday.  The data value is placed on Saturday unless the last week of the month spans into the next month and the Wednesday is in the current month.  In which case the data value will be placed on the last day of the current month.
                // 
                // 19  Sunday Saturday
                // Always calculates Sunday to Saturday. Data is considered to be part of the month that the most data falls into. If there is equal data from each month in a single week, the data is placed on the Saturday.  Click Here for examples of week 19.
                // 
                // 20     Sunday Saturday
                // Calculates only complete weeks within the month.
                // 
                // 24 Sun-Sat Calc in month with SUNDAY
                // 99 - "Start Date of Report"

                DateTime CurrDate;
                DateTime cd;
                bool Done;
                int inc;

                int CurrWeek;




            var StartOfWeek = (int)startDate.DayOfWeek;
                switch (weekDef)
                {
                    case var @case when 1 <= @case && @case <= 7:
                        {
                            cd = startDate.AddDays((offset - 1) * 7);
                            while ((int)cd.DayOfWeek != weekDef - 1)
                                cd = cd.AddDays(1);
                            EndDate = cd;   // SD + 6
                            StartDate = cd.AddDays(- 6);
                            break;
                        }

                    case var case1 when 11 <= case1 && case1 <= 17:
                    {
                        cd = startDate.AddDays((offset - 1) * 7);
                        while ((int)cd.DayOfWeek != (weekDef - 11))
                            cd = cd.AddDays(1);
                        EndDate = cd;   // SD + 6
                        StartDate = cd.AddDays(-6);
                        if (EndDate.Month != StartDate.Month)
                            StartDate = new DateTime(EndDate.Year, EndDate.Month, 1);
                        break;
                    }

                    case 99:
                        {
                            //FirstWeekStart = StartDate;
                            StartDate = startDate.AddDays(7 * (offset - 1));
                            EndDate = StartDate.AddDays(6);
                            break;
                        }

                    case 8:  // 8 - "Month Weeks (1-7, 8-14, 15-21,22-end of month)"
                        {
                            CurrDate = startDate;
                            CurrWeek = 1;
                            if (offset < 1)
                            {
                                inc = -1;
                            }
                            else
                            {
                                inc = 1;
                            }

                            do
                            {
                                StartOfWeek = CurrDate.Day;
                                switch (StartOfWeek)
                                {
                                    case var case2 when 1 <= case2 && case2 <= 7:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 1);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 7);
                                            break;
                                        }
                                    // CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                                    case var case3 when 8 <= case3 && case3 <= 14:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 8);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 14);
                                            break;
                                        }
                                    // CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                                    case var case4 when 15 <= case4 && case4 <= 21:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 15);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 21);
                                            break;
                                        }
                                    // CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                                    case var case5 when 22 <= case5 && case5 <= 31:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 22);
                                            EndDate = Helper.LastDOMValue(CurrDate);
                                            break;
                                        }
                                        // CurrDate = ED + 1


                                }

                                if (CurrWeek == offset || inc == 1 && CurrWeek > offset || inc == -1 && CurrWeek < offset)
                                {
                                    Done = true;
                                }
                                else
                                {
                                    Done = false;
                                }

                                if (inc == -1)
                                {
                                    CurrDate = StartDate.AddDays(-1);
                                }
                                else
                                {
                                    CurrDate = EndDate.AddDays(1);
                                }

                                CurrWeek += inc;
                            }
                            while (!Done);
                            break;
                        }

                    case 9: // 9 -  "Month Weeks (1-7, 8-14, 15-21,22-28)"
                        {
                            CurrDate = startDate;
                            CurrWeek = 1;
                            if (offset < 1)
                            {
                                inc = -1;
                            }
                            else
                            {
                                inc = 1;
                            }

                            do
                            {
                                StartOfWeek = CurrDate.Day;
                                switch (StartOfWeek)
                                {
                                    case var case6 when 1 <= case6 && case6 <= 7:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 1);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 7);
                                            if (inc == 1)
                                            {
                                                CurrDate = new DateTime(CurrDate.Year, CurrDate.Month, 8);
                                            }
                                            else
                                            {
                                                CurrDate = StartDate.AddDays(-1);
                                                CurrDate = new DateTime(CurrDate.Year, CurrDate.Month, 28);
                                            }

                                            break;
                                        }

                                    case var case7 when 8 <= case7 && case7 <= 14:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 8);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 14);
                                            if (inc == 1)
                                            {
                                                CurrDate = new DateTime(CurrDate.Year, CurrDate.Month, 15);
                                            }
                                            else
                                            {
                                                CurrDate = new DateTime(CurrDate.Year, CurrDate.Month, 7);
                                            }

                                            break;
                                        }

                                    case var case8 when 15 <= case8 && case8 <= 21:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 15);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 21);
                                            if (inc == 1)
                                            {
                                                CurrDate = new DateTime(CurrDate.Year, CurrDate.Month, 22);
                                            }
                                            else
                                            {
                                                CurrDate = new DateTime(CurrDate.Year, CurrDate.Month, 14);
                                            }

                                            break;
                                        }

                                    case var case9 when 22 <= case9 && case9 <= 28:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 22);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 28);
                                            if (inc == 1)
                                            {
                                                CurrDate = Helper.LastDOMValue(CurrDate).AddDays(1);
                                            }
                                            else
                                            {
                                                CurrDate = new DateTime(CurrDate.Year, CurrDate.Month, 21);
                                            }

                                            break;
                                        }
                                }

                                if (CurrWeek == offset || (inc == 1 && CurrWeek > offset) || inc == -1 && CurrWeek < offset)
                                {
                                    Done = true;
                                }
                                else
                                {
                                    Done = false;
                                }

                                CurrWeek += inc;
                            }
                            while (!Done);
                            break;
                        }

                    case 10:  // 20 - "Month Weeks (1-7, 8-14, 15-21,22-28,29-end of month)"
                        {
                            CurrDate = startDate;
                            CurrWeek = 1;
                            if (offset < 1)
                            {
                                inc = -1;
                            }
                            else
                            {
                                inc = 1;
                            }

                            do
                            {
                                StartOfWeek = CurrDate.Day;
                                switch (StartOfWeek)
                                {
                                    case var case10 when 1 <= case10 && case10 <= 7:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 1);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 7);
                                            break;
                                        }
                                    // CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                                    case var case11 when 8 <= case11 && case11 <= 14:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 8);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 14);
                                            break;
                                        }
                                    // CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                                    case var case12 when 15 <= case12 && case12 <= 21:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 15);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 21);
                                            break;
                                        }
                                    // CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                                    case var case13 when 22 <= case13 && case13 <= 28:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 22);
                                            EndDate = new DateTime(CurrDate.Year, CurrDate.Month, 28);
                                            break;
                                        }
                                    // CurrDate = ED + 1
                                    case var case14 when 29 <= case14 && case14 <= 31:
                                        {
                                            StartDate = new DateTime(CurrDate.Year, CurrDate.Month, 29);
                                            EndDate = Helper.LastDOMValue(CurrDate);
                                            break;
                                        }
                                        // CurrDate = ED + 1

                                }

                                if (CurrWeek == offset || (inc == 1 & CurrWeek > offset) || (inc == -1 & CurrWeek < offset))
                                {
                                    Done = true;
                                }
                                else
                                {
                                    Done = false;
                                }

                                if (inc == -1)
                                {
                                    CurrDate = StartDate.AddDays(-1);
                                }
                                else
                                {
                                    CurrDate = EndDate.AddDays(1);
                                }

                                CurrWeek = CurrWeek + inc;
                            }
                            while (!Done);
                            break;
                        }

                    case 18:  // '18  Sunday Saturday
                    {
                        // Always calculates Sunday to Saturday.
                        // The data value is placed on Saturday unless the last week of the month spans into the next month and the Wednesday
                        // is in the current month.  In which case the data value will be placed on the last day of the current month.


                        cd = startDate.AddDays((offset - 1) * 7);
                        while (cd.DayOfWeek != DayOfWeek.Saturday)
                            cd = cd.AddDays(1);
                        EndDate = cd;   // SD + 6
                        StartDate = cd.AddDays(-6);
                        if (EndDate.Month != StartDate.Month)
                        {
                            if (EndDate.Day < 4)
                            {
                                CalcDate = Helper.LastDOMValue(StartDate);
                            }
                            else
                            {
                                CalcDate = EndDate;
                            }
                        }

                        break;
                    }

                    case 19:  // '19  Sunday Saturday
                        {
                            // Put stat in week with more data
                            cd = startDate.AddDays((offset - 1) * 7);
                            while (cd.DayOfWeek != DayOfWeek.Saturday)
                                cd = cd.AddDays(1);
                            EndDate = cd;   // SD + 6
                            StartDate = cd.AddDays(-6);
                            if (EndDate.Month != StartDate.Month)
                            {
                                if (EndDate.Day < 4)
                                {
                                    CalcDate = Helper.LastDOMValue(StartDate);
                                }
                                else
                                {
                                    CalcDate = EndDate;
                                }
                            }

                            break;
                        }

                    case 20:  // '20 Sun - Sat Complete Weeks
                        {
                            if (offset < 1)
                            {
                                inc = -1;
                            }
                            else
                            {
                                inc = 1;
                            }

                            cd = startDate;   // DateAdd("d", StartDate, (Offset - 1) * 7)
                            while (cd.DayOfWeek != DayOfWeek.Sunday)
                                cd = cd.AddDays(1);
                            StartDate = cd;
                            EndDate = StartDate.AddDays(6);
                            CurrWeek = 1;
                            // Offset = 1
                            bool Found = false;
                            while (!Found)  // SD <> Offset
                            {
                                if (StartDate.Month == EndDate.Month)
                                {
                                    if (offset == CurrWeek)
                                    {
                                        Found = true;
                                        break;
                                    }

                                    CurrWeek = CurrWeek + inc;
                                }

                                if (inc == 1)
                                {
                                    StartDate = EndDate.AddDays(1);
                                    EndDate = EndDate.AddDays(7);
                                }
                                else
                                {
                                    EndDate = StartDate.AddDays(-1);
                                    StartDate = StartDate.AddDays(-7);
                                }

                                if (inc == 1 & CurrWeek > offset | inc == -1 & CurrWeek < offset)
                                {
                                    break;
                                }
                            }

                            if (!Found)
                            {
                                StartDate = DateTime.MinValue;
                                EndDate = DateTime.MinValue;
                            }

                            break;
                        }

                    case 23:
                        {
                            cd = startDate;   // DateAdd("d", lStartDate, (Offset - 1) * 7)
                            while (cd.DayOfWeek != DayOfWeek.Saturday)
                                cd = cd.AddDays(1);
                            StartDate = cd.AddDays(-6);
                            if (StartDate.Month != cd.Month)
                            {
                                StartDate = new DateTime(cd.Year, cd.Month, 1);
                                EndDate = StartDate;
                                while (EndDate.DayOfWeek != DayOfWeek.Saturday)
                                    EndDate = EndDate.AddDays(1);
                            }
                            else
                            {
                                EndDate = StartDate.AddDays(6);
                            }

                            CurrWeek = 1;
                            // Offset = 1
                            bool Found = false;
                            while (!Found)  // SD <> Offset
                            {
                                if (StartDate.Month == EndDate.Month)
                                {
                                    if (offset == CurrWeek)
                                {
                                        Found = true;
                                        break;
                                    }

                                    CurrWeek = CurrWeek + 1;
                                }
                                else
                                {
                                    EndDate = Helper.LastDOMValue(StartDate);
                                    if (offset == CurrWeek)
                                    {
                                        Found = true;
                                        break;
                                    }
                                }

                                StartDate = EndDate.AddDays(1);
                                EndDate = EndDate.AddDays(7);
                                if (CurrWeek > offset)
                                {
                                    break;
                                }
                            }

                            if (!Found)
                            {
                                StartDate = DateTime.MinValue;
                                EndDate = DateTime.MinValue;
                            }

                            break;
                        }

                    case 24:
                        {
                            cd = startDate;
                            while (cd.DayOfWeek != DayOfWeek.Sunday)
                                cd = cd.AddDays(1);
                            cd = cd.AddDays((offset - 1) * 7);
                            StartDate = cd;
                            EndDate = cd.AddDays(6);   // default to week type 7
                            break;
                        }

                    default:
                        {
                            cd = startDate.AddDays((offset - 1) * 7);
                            while (cd.DayOfWeek != DayOfWeek.Saturday)
                                cd = cd.AddDays(1);
                            EndDate = cd;   // SD + 6
                            StartDate = cd.AddDays(-6);
                            break;
                        }
                }

            // If Not DataOut Then
            // If SD < StartDate Then
            // SD = StartDate
            // End If
            // End If

            if (CalcDate == DateTime.MinValue)
                CalcDate = EndDate;
        }
    }
}
/*
 Sub GetWeekDates(Offset, SD As Date, ED As Date, WeekDef As Integer, DataOut As Boolean, StartDate As Date, Optional EndDate As Date)
'1   Monday - Sunday
'2   Tuesday - Monday
'3   Wednesday - Tuesday
'4   Thursday - Wednesday
'5   Friday - Thursday
'6   Saturday - Friday
'7   Sunday - Saturday
'
'8   1st 7th
'    8th 14th
'    15th    21st
'    22nd    Last Day of Month
'
'9   1st 7th
'    8th 14th
'    15th    21st
'    22nd    28th
'    (Days greater than the 28th are ignored.)
'
'10  1st 7th
'    8th 14th
'    15th    21st
'    22nd    28th
'    29th    Last Day of Month
'
'11  Monday (or first of the month)  Sunday
'12  Tuesday (or first of the month) Monday
'13  Wednesday (or first of the month)   Tuesday
'14  Thursday (or first of the month)    Wednesday
'15  Friday (or first of the month)  Thursday
'16  Saturday (or first of the month)    Friday
'17  Sunday (or first of the month)  Saturday
'
'18  Sunday Saturday
'Always calculates Sunday to Saturday.  The data value is placed on Saturday unless the last week of the month spans into the next month and the Wednesday is in the current month.  In which case the data value will be placed on the last day of the current month.
'
'19  Sunday Saturday
'Always calculates Sunday to Saturday. Data is considered to be part of the month that the most data falls into. If there is equal data from each month in a single week, the data is placed on the Saturday.  Click Here for examples of week 19.
'
'20     Sunday Saturday
'       Calculates only complete weeks within the month.
'
'24 Sun-Sat Calc in month with SUNDAY
'99 - "Start Date of Report"

Dim CurrDate As Date
Dim cd As Date
Dim Done As Boolean
Dim inc As Integer


StartOfWeek = format(StartDate, "w")
Select Case WeekDef
    Case 1 To 7
        cd = DateAdd("d", StartDate, (Offset - 1) * 7)
        Do While Weekday(cd) <> WeekDef
            cd = cd + 1
        Loop
        ED = cd   'SD + 6
        SD = cd - 6
        
    Case 11 To 17
        cd = DateAdd("d", StartDate, (Offset - 1) * 7)
        Do While Weekday(cd) <> (WeekDef - 10)
            cd = cd + 1
        Loop
        ED = cd   'SD + 6
        SD = cd - 6
        If Month(ED) <> Month(SD) Then
            SD = DateSerial(year(ED), Month(ED), 1)
        End If
        
    Case 99
        FirstWeekStart = StartDate
        SD = DateAdd("d", StartDate, 7 * (Offset - 1))
        ED = SD + 6
        
    Case 8  '8 - "Month Weeks (1-7, 8-14, 15-21,22-end of month)"
        CurrDate = StartDate
        CurrWeek = 1
        If Offset < 1 Then
            inc = -1
        Else
            inc = 1
        End If
        Do
            StartOfWeek = format(CurrDate, "d")

            Select Case StartOfWeek
                Case 1 To 7:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 1)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 7)
                    'CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                Case 8 To 14:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 14)
                    'CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                Case 15 To 21:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 21)
                    'CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                Case 22 To 31:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                    ED = Helper.LastDOMValue(CurrDate)
                    'CurrDate = ED + 1

                    
            End Select
            If CurrWeek = Offset Or (inc = 1 And CurrWeek > Offset) Or (inc = -1 And CurrWeek < Offset) Then
                Done = True
            Else
                Done = False
            End If
            If inc = -1 Then
                CurrDate = DateAdd("d", -1, SD)
            Else
                CurrDate = DateAdd("d", 1, ED)
            End If
            CurrWeek = CurrWeek + inc
        Loop Until Done
        
        
    Case 9 '9 -  "Month Weeks (1-7, 8-14, 15-21,22-28)"
        CurrDate = StartDate
        CurrWeek = 1
        If Offset < 1 Then
            inc = -1
        Else
            inc = 1
        End If
        Do
            StartOfWeek = format(CurrDate, "d")

            Select Case StartOfWeek
                Case 1 To 7:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 1)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 7)
                    If inc = 1 Then
                        CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                    Else
                        CurrDate = DateAdd("d", -1, SD)
                        CurrDate = DateSerial(year(CurrDate), Month(CurrDate), 28)
                    End If
                Case 8 To 14:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 14)
                    If inc = 1 Then
                        CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                    Else
                        CurrDate = DateSerial(year(CurrDate), Month(CurrDate), 7)
                    End If
                Case 15 To 21:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 21)
                    If inc = 1 Then
                        CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                    Else
                        CurrDate = DateSerial(year(CurrDate), Month(CurrDate), 14)
                    End If
                Case 22 To 28:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 28)
                    If inc = 1 Then
                        CurrDate = Helper.LastDOMValue(CurrDate) + 1
                    Else
                        CurrDate = DateSerial(year(CurrDate), Month(CurrDate), 21)
                    End If
            End Select
            
            If CurrWeek = Offset Or (inc = 1 And CurrWeek > Offset) Or (inc = -1 And CurrWeek < Offset) Then
                Done = True
            Else
                Done = False
            End If
            CurrWeek = CurrWeek + inc
        Loop Until Done
        
        
   Case 10  '20 - "Month Weeks (1-7, 8-14, 15-21,22-28,29-end of month)"
        CurrDate = StartDate
        CurrWeek = 1
        If Offset < 1 Then
            inc = -1
        Else
            inc = 1
        End If
        Do
            StartOfWeek = format(CurrDate, "d")

            Select Case StartOfWeek
                Case 1 To 7:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 1)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 7)
                    'CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                Case 8 To 14:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 8)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 14)
                    'CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                Case 15 To 21:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 15)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 21)
                    'CurrDate = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                Case 22 To 28:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 22)
                    ED = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 28)
                    'CurrDate = ED + 1
                Case 29 To 31:
                    SD = DateSerial(format(CurrDate, "yyyy"), format(CurrDate, "m"), 29)
                    ED = Helper.LastDOMValue(CurrDate)
                    'CurrDate = ED + 1
                    
            End Select
            If CurrWeek = Offset Or (inc = 1 And CurrWeek > Offset) Or (inc = -1 And CurrWeek < Offset) Then
                Done = True
            Else
                Done = False
            End If
            If inc = -1 Then
                CurrDate = DateAdd("d", -1, SD)
            Else
                CurrDate = DateAdd("d", 1, ED)
            End If
            CurrWeek = CurrWeek + inc
        Loop Until Done
        
        
        
    Case 18  ''18  Sunday Saturday
            'Always calculates Sunday to Saturday.
            'The data value is placed on Saturday unless the last week of the month spans into the next month and the Wednesday
            'is in the current month.  In which case the data value will be placed on the last day of the current month.
            
        cd = DateAdd("d", StartDate, (Offset - 1) * 7)
        Do While Weekday(cd) <> vbSaturday
            cd = cd + 1
        Loop
        ED = cd   'SD + 6
        SD = cd - 6
        If Month(ED) <> Month(SD) Then
            If Day(ED) < 4 Then
                CalcDate = Helper.LastDOMValue(SD)
            Else
                CalcDate = ED
            End If
        End If
            
    Case 19  ''19  Sunday Saturday
            'Put stat in week with more data
        cd = DateAdd("d", StartDate, (Offset - 1) * 7)
        Do While Weekday(cd) <> vbSaturday
            cd = cd + 1
        Loop
        ED = cd   'SD + 6
        SD = cd - 6
        If Month(ED) <> Month(SD) Then
            If Day(ED) < 4 Then
                CalcDate = Helper.LastDOMValue(SD)
            Else
                CalcDate = ED
            End If
        End If
        
            
    Case 20  ''20 Sun - Sat Complete Weeks
        If Offset < 1 Then
            inc = -1
        Else
            inc = 1
        End If
        cd = StartDate   'DateAdd("d", StartDate, (Offset - 1) * 7)
        Do While Weekday(cd) <> vbSunday
            cd = cd + 1
        Loop
        SD = cd
        ED = SD + 6
        CurrWeek = 1
        'Offset = 1
        Found = False
        Do While Not Found  'SD <> Offset
            If Month(SD) = Month(ED) Then
                If Offset = CurrWeek Then
                    Found = True
                    Exit Do
                End If
                CurrWeek = CurrWeek + inc
            End If
            If inc = 1 Then
                SD = ED + 1
                ED = ED + 7
            Else
                ED = SD - 1
                SD = SD - 7
                
            End If
            If (inc = 1 And CurrWeek > Offset) Or (inc = -1 And CurrWeek < Offset) Then
                Exit Do
            End If
        Loop
        If Not Found Then
            SD = 0
            ED = 0
        End If
        
    Case 23:
            cd = StartDate   'DateAdd("d", lStartDate, (Offset - 1) * 7)
            Do While Weekday(cd) <> vbSaturday
                cd = cd + 1
            Loop
            SD = cd - 6
            If Month(SD) <> Month(cd) Then
                SD = DateSerial(year(cd), Month(cd), 1)
                ED = SD
                Do While Weekday(ED) <> vbSaturday
                    ED = ED + 1
                Loop
            Else
                ED = SD + 6
            End If
            CurrWeek = 1
            'Offset = 1
            Found = False
            Do While Not Found  'SD <> Offset
                If Month(SD) = Month(ED) Then
                    If Offset = CurrWeek Then
                        Found = True
                        Exit Do
                    End If
                    CurrWeek = CurrWeek + 1
                Else
                    ED = Helper.LastDOMValue(SD)
                    If Offset = CurrWeek Then
                        Found = True
                        Exit Do
                    End If
                End If
                
                SD = ED + 1
                ED = ED + 7
                If CurrWeek > Offset Then
                    Exit Do
                End If
            Loop
            If Not Found Then
                SD = 0
                ED = 0
            End If
        
    Case 24:
            cd = StartDate
            Do Until Weekday(cd) = vbSunday
                cd = cd + 1
            Loop
            cd = DateAdd("d", cd, (Offset - 1) * 7)
            SD = cd
            ED = cd + 6
    
    
    
    Case Else   'default to week type 7
        cd = DateAdd("d", StartDate, (Offset - 1) * 7)
        Do While Weekday(cd) <> vbSaturday
            cd = cd + 1
        Loop
        ED = cd   'SD + 6
        SD = cd - 6
End Select

'If Not DataOut Then
'    If SD < StartDate Then
'        SD = StartDate
'    End If
'End If
End Sub*/
