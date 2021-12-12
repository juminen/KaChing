namespace KaChing.Model
{
	/// <summary>
	/// Interface for analysis results.
	/// </summary>
	public interface IAnalysisResult
	{
		/// <summary>
		/// For determining if analysis was succesfull or not. Initial value is true.
		/// </summary>
		bool AnalysisSuccesfull { get; }
		/// <summary>
		/// General purpose message, eg. for failed analysis result.
		/// </summary>
		string Message { get; }
	}

	/// <summary>
	/// Interface for trading volume.
	/// </summary>
	public interface ITradingVolumeDay : IAnalysisResult
	{
		decimal Volume { get; }
		string Currency { get; }
		DateTime Date { get; }
	}

	/// <summary>
	/// Interface for buy and sell dates.
	/// </summary>
	public interface IBuySellDates : IAnalysisResult
	{
		DateTime Buy { get; }
		DateTime Sell { get; }
	}

	/// <summary>
	/// Interface for market data analysis result.
	/// </summary>
	public interface IMarketDataAnalysisResults : IAnalysisResult
	{
		int LongestDownwardTrendLength { get; }
		ITradingVolumeDay HighestTradingVolume { get; }
		IBuySellDates BestDaysToBuyAndSell { get; }
	}

}