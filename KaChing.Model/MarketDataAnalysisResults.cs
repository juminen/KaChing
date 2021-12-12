namespace KaChing.Model
{
	/// <summary>
	/// Abstract base for analysis results.
	/// </summary>
	abstract class ResultBase: IAnalysisResult
	{
		/// <summary>
		/// For determining if analysis was succesfull or not. Initial value is true.
		/// </summary>
		public bool AnalysisSuccesfull { get; set; } = true;
		/// <summary>
		/// General purpose message, eg. for failed analysis result.
		/// </summary>
		public string Message { get; set; } = string.Empty;

		/// <summary>
		/// Sets AnalysisSuccesfull to false and given message to Message.
		/// </summary>
		/// <param name="message">Reason why analysis was not succesfull</param>
		public void SetFailAndMessage(string message)
		{
			AnalysisSuccesfull = false;
			Message = message;
		}
	}

	class TradingVolumeDay : ResultBase, ITradingVolumeDay
	{
		public decimal Volume { get; set; }
		public string Currency { get; set; } =	string.Empty;
		public DateTime Date { get; set; }
	}

	class BuySellDates : ResultBase, IBuySellDates
	{
		public DateTime Buy { get; set; }
		public DateTime Sell { get; set; }
	}

	class MarketDataAnalysisResults : ResultBase, IMarketDataAnalysisResults
	{
		public int LongestDownwardTrendLength { get; set; }
		public ITradingVolumeDay HighestTradingVolume { get; set; } 
		public IBuySellDates BestDaysToBuyAndSell { get; set; } 
	}
}