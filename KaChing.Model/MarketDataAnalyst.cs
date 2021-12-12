using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KaChing.Test")]
namespace KaChing.Model
{
	internal static class MarketDataAnalyst
	{
		/// <summary>
		/// Finds the highest trading volume and day from given datapoints.
		/// </summary>
		/// <param name="data">Collection of datapoints</param>
		/// <returns>Object containing highest volume and day</returns>
		public static TradingVolumeDay GetHighestTradingVolumeAndDay(IList<Datapoint> data)
		{
			TradingVolumeDay result = new();

			if (data.Count < 1)
			{
				result.SetFailAndMessage("There is no data to run analysis.");
				return result;
			}

			//TODO: What if there are multiple days with same trading volume?
			//Not very likely situation but possible.
			Datapoint highestVolume = data[0];

			for (int i = 0; i < data.Count; i++)
			{
				if (data[i].Value > highestVolume.Value)
				{
					highestVolume = data[i];
				}
			}
			return new()
			{
				Volume = highestVolume.Value,
				Date = highestVolume.Date
			};
		}
		/// <summary>
		/// Finds the longest downward (bear) trend length from given datapoints.
		/// </summary>
		/// <param name="data">Collection of datapoints</param>
		/// <returns>Integer representing trend length</returns>
		public static int GetLongestDownwardTrendLength(IList<Datapoint> data)
		{
			int longestBearTrendLength = 0;
			int currentBearTrendLength = 0;

			for (int i = 1; i < data.Count; i++)
			{
				if (data[i].Value < data[i - 1].Value)
				{
					//Current coin value was smaller than previous values so adding up
					currentBearTrendLength++;
					//Update longest trend length if neccessary
					if (currentBearTrendLength > longestBearTrendLength)
					{
						longestBearTrendLength = currentBearTrendLength;
					}
				}
				else
				{
					currentBearTrendLength = 0;
				}
			}
			return longestBearTrendLength;
		}
		/// <summary>
		/// Finds the best day to buy and sell (ie. maximum profit) from given datapoints.
		/// </summary>
		/// <param name="data">Collection of datapoints</param>
		/// <returns>Object containing two dates</returns>
		public static BuySellDates GetBestDaysToBuyAndSell(IList<Datapoint> data)
		{
			BuySellDates result = new();

			//Check that there are atleast two datapoints to compare
			if (data.Count < 2)
			{
				string s = "Could not analyse best buy and sell date.";
				s += " There are not enough datapoints to run analysis.";
				result.SetFailAndMessage(s);
				return result;
			}

			//In order to find best day to buy and sell, 
			//we need to find maximum profit

			Datapoint bestMaxProfitPoint = null;
			Datapoint bestMinProfitPoint = null;
			decimal bestMaxProfit = 0;

			Datapoint currentMinProfitPoint = data[0];

			for (int i = 1; i < data.Count; i++)
			{
				decimal currentProfit = data[i].Value - currentMinProfitPoint.Value;
				if (currentProfit > bestMaxProfit)
				{
					//Profit was bigger so update current maximum profit
					bestMaxProfitPoint = data[i];
					bestMinProfitPoint = currentMinProfitPoint;
					bestMaxProfit = currentProfit;
				}
				//Check for min value
				if (data[i].Value < currentMinProfitPoint.Value)
				{
					currentMinProfitPoint = data[i];
				}
			}

			if (bestMinProfitPoint != null && bestMaxProfitPoint != null)
			{
				result.Buy = bestMinProfitPoint.Date;
				result.Sell = bestMaxProfitPoint.Date;
			}
			else
			{
				result.SetFailAndMessage("There were no good days to buy and sell.");
			}
			return result;

			//TODO: What if there are multiple periods with same maximum profits?
			//E.g. values are [1, 2, 1, 2, 1, 2, 1, 2]
		}
	}
}