namespace KaChing.Model
{
	internal static class UnixTimeConverter
	{
		/// <summary>
		/// Converts given datetime to unix timestamp.
		/// </summary>
		/// <param name="date">Datetime to convert</param>
		/// <returns>Unix timestamp in seconds</returns>
		public static long ConvertDateToUnixTimeStamp(DateTime date)
		{
			DateTime d = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);
			DateTimeOffset offset = new DateTimeOffset(d);
			return offset.ToUnixTimeSeconds();
		}

		/// <summary>
		/// Converts given unix timestamp to UTC datetime. 
		/// Timestamp can be 10 (=seconds) or 13 digit (=milliseconds) timestamp
		/// </summary>
		/// <param name="timeStamp"></param>
		/// <returns></returns>
		public static DateTime ConvertUnixTimeStampToDate(long timeStamp)
		{
			//Unix time stamp length:
			//seconds = 10-digit
			//milliseconds = 13-digit
			//microseconds = 16-digit
			int len = timeStamp.ToString().Length;
			if (len == 10)
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp);
			}
			else if (len == 13)
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timeStamp);
			}
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			//return DateTimeOffset.FromUnixTimeMilliseconds(timeStamp).UtcDateTime;

		}
	}
}