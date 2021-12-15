using Operations.Spreadsheet.Protobuf.Models;
using System;

namespace ONE.Operations.Spreadsheet
{
    /* AK: 
     * This class is from Geoff Voss and what is used to calculate the row numbers based on time stamp and
     * vice versa. This may be a good candidate to refactor into the C# protocol buffer package. I
     * Was not able to find reference documentation describing the details on how the row numbers should be calculated
     */
    public static class RowNumber
    {
        // All row number math is done from the date of 1-1-1900 00:00:00.000
        // This date should never ever change. 
        private static readonly DateTime BaseTime = new DateTime(1900, 01, 01, 0, 0, 0);


        /// <summary>
        /// The <see cref="DateTime"/> returned is computed from the row number and worksheet type
        /// </summary>
        /// <param name="rowNumber">Row number to convert to <see cref="DateTime"/></param>
        /// <param name="worksheetType">Worksheet type to compute the </param>
        /// <returns></returns>
        public static DateTime? GetDateTimeFromRowNumberAndWorksheetType(uint rowNumber, EnumWorksheet worksheetType)
        {
            if (worksheetType == EnumWorksheet.WorksheetUnknown)
                return null;

            if (rowNumber == 0)
                return null;

            var baseTime = BaseTime;
            uint offsetRowNumber = rowNumber - 1;

            var windowSize = TimeSpanOfWorksheetType(worksheetType);
            return baseTime.AddMinutes(windowSize.TotalMinutes * offsetRowNumber);
        }

        /// <summary>
        /// Computes the row number of the <see cref="DateTime"/> for the given known <see cref="EnumWorksheet"/>.
        /// </summary>
        /// <param name="localDateTime"><see cref="DateTime"/> to convert to worksheet row number</param>
        /// <param name="worksheetType">Worksheet type to compute the row number for6</param>
        /// <returns></returns>
        public static uint GetRowNumberFromDateTimeAndWorksheetType(DateTime localDateTime, EnumWorksheet worksheetType)
        {
            var baseTime = BaseTime;
            var diffTime = localDateTime - baseTime;

            var windowSize = TimeSpanOfWorksheetType(worksheetType);

            return (uint)(diffTime.TotalMinutes / windowSize.TotalMinutes) + 1;
        }

        private static TimeSpan TimeSpanOfWorksheetType(EnumWorksheet worksheetType)
        {
            switch (worksheetType)
            {
                case EnumWorksheet.WorksheetFifteenMinute:
                    return TimeSpan.FromMinutes(15);
                case EnumWorksheet.WorksheetHour:
                    return TimeSpan.FromHours(1);
                case EnumWorksheet.WorksheetFourHour:
                    return TimeSpan.FromHours(4);
                case EnumWorksheet.WorksheetDaily:
                    return TimeSpan.FromDays(1);
                default:
                    return TimeSpan.FromTicks(1);
            }
        }
    }
}
