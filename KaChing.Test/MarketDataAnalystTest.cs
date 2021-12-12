using KaChing.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace KaChing.Test
{
	[TestClass]
	public class MarketDataAnalystTest
	{
		#region Longest downward trend length
		[TestMethod]
		public void LongestBearTrendLengthForRisingTrend()
		{
			//Arrange
			List<Datapoint> datapoints = new();
			for (int i = 0; i < 5; i++)
			{
				DateTime dt = DateTime.Now.AddDays(i);
				Datapoint dp = new() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(dt), Value = i };
				datapoints.Add(dp);
			}
			//Act
			int length = MarketDataAnalyst.GetLongestDownwardTrendLength(datapoints);

			//Assert
			Assert.AreEqual(0, length);
		}

		[TestMethod]
		public void LongestBearTrendLengthForDescendingTrend()
		{
			//Arrange
			int startValue = 5;
			List<Datapoint> datapoints = new();
			for (int i = 0; i < startValue; i++)
			{
				DateTime dt = DateTime.Now.AddDays(i);
				Datapoint dp = new() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(dt), Value = startValue - i };
				datapoints.Add(dp);
			}
			//Act
			int length = MarketDataAnalyst.GetLongestDownwardTrendLength(datapoints);

			//Assert			
			Assert.AreEqual(datapoints.Count - 1, length);
		}

		[TestMethod]
		public void LongestBearTrendLengthForRsiginAndDescendingTrend()
		{
			//Arrange
			List<Datapoint> datapoints = new()
			{
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(1)), Value = 1 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(2)), Value = 2 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(3)), Value = 3 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(4)), Value = 4 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(5)), Value = 3 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(6)), Value = 2 }
			};
			//Act
			int length = MarketDataAnalyst.GetLongestDownwardTrendLength(datapoints);

			//Assert			
			Assert.AreEqual(2, length);
		}
		#endregion

		#region Highest trading volume
		[TestMethod]
		public void NoHighestTradingVolume()
		{
			//Arrange
			List<Datapoint> datapoints = new() { };
			//Act
			var result = MarketDataAnalyst.GetHighestTradingVolumeAndDay(datapoints);
			//Assert
			Assert.AreEqual(false, result.AnalysisSuccesfull);
		}

		[TestMethod]
		public void HighestTradingVolume()
		{
			//Arrange
			Datapoint highest = new() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now), Value = 100 };
			List<Datapoint> datapoints = new()
			{
				highest,
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(1)), Value = 1 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(2)), Value = 2 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(3)), Value = 3 }
			};

			//Act
			var result = MarketDataAnalyst.GetHighestTradingVolumeAndDay(datapoints);

			//Assert
			Assert.AreEqual(highest.Value, result.Volume);
			Assert.AreEqual(true, result.AnalysisSuccesfull);
		}
		#endregion

		#region Best days to buy and sell
		[TestMethod]
		public void NoBestDaysToBuyAndSellEmptyList()
		{
			//Arrange
			List<Datapoint> datapoints = new() { };
			//Act
			var result = MarketDataAnalyst.GetBestDaysToBuyAndSell(datapoints);
			//Assert
			Assert.AreEqual(false, result.AnalysisSuccesfull);
		}

		[TestMethod]
		public void NoBestDaysToBuyAndSellDownwardTrend()
		{
			//Arrange
			List<Datapoint> datapoints = new()
			{
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(1)), Value = 3 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(2)), Value = 2 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(3)), Value = 1 }
			};

			//Act
			var result = MarketDataAnalyst.GetBestDaysToBuyAndSell(datapoints);

			//Assert
			Assert.AreEqual(false, result.AnalysisSuccesfull);
		}

		[TestMethod]
		public void BestDaysToBuyAndSellUpwardTrend()
		{
			//Arrange
			DateTime minDate = DateTime.UtcNow;
			Datapoint min = new() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(minDate), Value = 1 };
			DateTime maxDate = DateTime.UtcNow.AddDays(4);
			Datapoint max = new() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(maxDate), Value = 100 };
			List<Datapoint> datapoints = new()
			{
				min,
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(1)), Value = 1 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(2)), Value = 2 },
				new Datapoint() { UnixTimestamp = UnixTimeConverter.ConvertDateToUnixTimeStamp(DateTime.Now.AddDays(3)), Value = 3 },
				max
			};

			//Act
			var result = MarketDataAnalyst.GetBestDaysToBuyAndSell(datapoints);

			//Assert
			string format = "dd.MM.yyyy HH:mm:ss";
			Assert.AreEqual(minDate.ToString(format), result.Buy.ToString(format));
			Assert.AreEqual(maxDate.ToString(format), result.Sell.ToString(format));
			Assert.AreEqual(true, result.AnalysisSuccesfull);
		}
		#endregion
	}
}
