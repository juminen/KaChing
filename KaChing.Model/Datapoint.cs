namespace KaChing.Model
{
	/// <summary>
	/// Record to hold values for data containing unix timestamp.
	/// </summary>
	public record Datapoint
	{
		public long UnixTimestamp { get; init; }
		public DateTime Date
		{
			get
			{
				return UnixTimeConverter.ConvertUnixTimeStampToDate(UnixTimestamp);
			}
		}
		public decimal Value { get; init; }
		public override string ToString()
		{
			return $"Date: {Date:yyyy-MM-dd HH:mm:ss}, Value: {Value}";
		}
	}
}