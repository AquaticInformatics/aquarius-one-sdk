using System;
using ONE.Models.CSharp;
using Newtonsoft.Json;
using NodaTime;

namespace ONE.Utilities
{
    
        /// <summary>
        /// Helpers for managing DateTime, ClarosDateTime with respect to 'WIMS Local Time'.
        /// 'WIMS Local Time' is the time coordinate system used in spreadsheets.
        /// <remarks>
        /// For more infomation about time handling in spreadsheets:
        /// <see cref="http://confluence.hach.ewqg.com/pages/viewpage.action?pageId=111176358"/>.
        /// </remarks>
        /// </summary>
        public static class DateTimeHelper
        {
            /// <summary>
            /// The Base time for spreadsheets.
            /// This is the 'WIMS Local Time' for the first spreadsheet row.
            /// </summary>
            public static readonly DateTime BaseTime = new DateTime(1900, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            #region Extension Methods

            /// <summary>
            /// Gets the spreadsheet row number.
            /// </summary>
            /// <param name="wimsLocalTime">DateTime in 'WIMS Local Time' coordinates.</param>
            /// <param name="worksheetType">Worksheet type</param>
            /// <returns>Row number.</returns>
            public static uint GetRowNumber(this DateTime wimsLocalTime, EnumWorksheet worksheetType)
            {
                var diffTime = wimsLocalTime - BaseTime;

                var windowSize = TimeSpanOfWorksheetType(worksheetType);

                return (uint)(diffTime.TotalMinutes / windowSize.TotalMinutes) + 1;
            }

            /// <summary>
            /// Gets the ClarosDateTime.
            /// </summary>
            /// <param name="dateTime">DateTime.</param>
            /// <returns>ClarosDateTime without converting time coordinates.</returns>
            public static JsonTicksDateTime ToJsonTicksDateTime(this DateTime dateTime)
            {
                // This line ensures that the result will have the trailing 'Z' (zulu time)
                var dt = new DateTime(dateTime.Ticks, DateTimeKind.Utc);

                return new JsonTicksDateTime
                {
                    JsonDateTime = JsonConvert.SerializeObject(dt).Replace("\"", "")
                };
            }

            /// <summary>
            /// Gets the DateTime.
            /// </summary>
            /// <param name="jsonTicksDateTime">ClarosDateTime.</param>
            /// <returns>DateTime without converting time coordinates.</returns>
            public static DateTime ToDateTime(this JsonTicksDateTime jsonTicksDateTime)
            {
                if (jsonTicksDateTime.Ticks.HasValue)
                    return new DateTime((long)jsonTicksDateTime.Ticks, DateTimeKind.Utc);

                if (jsonTicksDateTime.JsonDateTime != null && DateTimeOffset.TryParse(jsonTicksDateTime.JsonDateTime, out var time))
                    return time.UtcDateTime;

                throw new ArgumentException("Invalid ClarosDateTime.", nameof(jsonTicksDateTime));
            }

            #endregion Extension Methods

            #region Helper Methods

            /// <summary>
            /// Gets the TimeSpan of the specified worksheet type.
            /// </summary>
            public static TimeSpan TimeSpanOfWorksheetType(EnumWorksheet worksheetType)
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
                        throw new ArgumentException($"Invalid worksheet type: {worksheetType}", nameof(worksheetType));
                }
            }

            /// <summary>
            /// Get the TimeWindow for a spreadsheet row.
            /// </summary>
            /// <param name="rowNumber">Row number.</param>
            /// <param name="worksheetType">Worksheet type.</param>
            /// <returns>TimeWindow for the row in 'WIMS Local Time' coordinates.</returns>
            public static TimeWindow GetTimeWindow(uint rowNumber, EnumWorksheet worksheetType)
            {
                return new TimeWindow
                {
                    StartTime = GetWimsLocalTime(rowNumber, worksheetType).ToJsonTicksDateTime(),
                    EndTime = GetWimsLocalTime(rowNumber + 1, worksheetType).ToJsonTicksDateTime(),
                    DurationInSeconds = (uint)TimeSpanOfWorksheetType(worksheetType).TotalSeconds
                };
            }

            /// <summary>
            /// Gets a DateTime for a row.
            /// </summary>
            /// <param name="rowNumber">Row number.</param>
            /// <param name="worksheetType">Worksheet type.</param>
            /// <returns>DateTime in 'WIMS Local Time' coordinates.</returns>
            public static DateTime GetWimsLocalTime(uint rowNumber, EnumWorksheet worksheetType)
            {
                if (rowNumber == 0)
                    throw new ArgumentException($"Invalid row number: {rowNumber}");

                var offsetRowNumber = rowNumber - 1;

                var windowSize = TimeSpanOfWorksheetType(worksheetType);

                return BaseTime.AddMinutes(windowSize.TotalMinutes * offsetRowNumber);
            }

            /// <summary>
            /// Convert a DateTime in UTC to a DateTime in 'WIMS Local Time'.
            /// <remarks>
            /// Used to convert a time received from a client in UTC.
            /// </remarks>
            /// </summary>
            /// <param name="utcTime">UTC time.</param>
            /// <param name="spreadsheetTimeZone">The time zone of the spreadsheet.</param>
            /// <returns>DateTime in 'WIMS Local Time' coordinates.</returns>
            public static DateTime ConvertUtcToWimsLocalTime(DateTime utcTime, EnumTimeZone spreadsheetTimeZone)
            {
                var timeZoneString = TimezoneUtility.Timezones[(int)spreadsheetTimeZone];
                var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneString);
                if (timeZone == null)
                    throw new ArgumentException($"Invalid spreadsheet time zone: {spreadsheetTimeZone}", nameof(spreadsheetTimeZone));

                var instant = Instant.FromDateTimeUtc(utcTime);
                var localDateTime = instant.InZone(timeZone);

                return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day,
                    localDateTime.Hour, localDateTime.Minute, localDateTime.Second, DateTimeKind.Utc);
            }

            /// <summary>
            /// Convert a ClarosDateTime in UTC to a ClarosDateTime in 'WIMS Local Time'.
            /// <remarks>
            /// Used to convert a time received from a client in UTC.  If client doesn't provide a timestamp, then
            /// we will use UTCNow instead.  Please refer http://jira.hach.ewqg.com/browse/CLAROS-25493 for more information.
            /// </remarks>
            /// </summary>
            /// <param name="spreadsheetTimeZone">The time zone of the spreadsheet.</param>
            /// <param name="utcTime">UTC time.</param>
            /// <returns>ClarosDateTime in 'WIMS Local Time' coordinates.</returns>
            public static JsonTicksDateTime ConvertUtcToWimsLocalTime(EnumTimeZone spreadsheetTimeZone, JsonTicksDateTime utcTime)
            {
                utcTime = utcTime ?? DateTime.UtcNow.ToJsonTicksDateTime();

                return ConvertUtcToWimsLocalTime(utcTime.ToDateTime(), spreadsheetTimeZone).ToJsonTicksDateTime();
            }

            #endregion Helper Methods
        }
    }
