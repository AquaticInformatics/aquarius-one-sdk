using System;

namespace ONE.Utilities
{
    public static class DateTimeExtensions
    {
		public static string ToStringEx(this DateTime input, string format)
		{
			string dateString = string.Empty;

			string firstFormatChar = format.Substring(0, 1);

			switch (firstFormatChar)
			{
				case "q":
				case "Q":
					string remainingFormatChar = format.Substring(1);

					int firstQuarterMonth = 1;

					if (remainingFormatChar.Length > 0)
					{
						firstQuarterMonth = int.Parse(remainingFormatChar);
						if (firstQuarterMonth < 1 || firstQuarterMonth > 12)
							throw new FormatException
							("Expecting number between 1 & 12!");
					}

					//shift the reference to the starting of first quarter
					int monthNo = input.Month - firstQuarterMonth + 1;
					if (monthNo <= 0)
						monthNo += 12; //adding 12 when we are crossing the year

					//forcing 'double' input and type casting the return value
					int quarter = (int)Math.Ceiling(monthNo / 3.0);
					dateString = quarter.ToString();

					break;

				default:
					dateString = input.ToString
						(format, System.Globalization.CultureInfo.InvariantCulture);
					break;
			}
			return dateString;
		}
	}
}
