using System.Text.Json;

namespace KaChing.Model
{
	public class AnalysisService
	{
		#region properties
		static readonly HttpClient client = new();
		private long startDate;
		private long endDate;
		private string coin = string.Empty;
		private string currency = string.Empty;
		private string url = string.Empty;
		private string jsonResult = string.Empty;
		private MarketData marketData;
		private MarketDataAnalysisResults results;
		#endregion

		/// <summary>
		/// Returns avaible currencies.
		/// </summary>
		/// <returns>List of currencies</returns>
		public static async Task<IList<string>> ListCurrenciesAsync()
		{
			//For future improvements: get currencies from Coingecko
			//For now, return list only with euro
			return await Task.Run(() => new List<string>() { "eur" });
		}

		/// <summary>
		/// Returns avaible coins.
		/// </summary>
		/// <returns>List of currencies</returns>
		public static async Task<IList<string>> ListCoinsAsync()
		{
			//For future improvements: get coins from Coingecko
			//For now, return list only with bitcoin
			return await Task.Run(() => new List<string>() { "bitcoin" });
		}

		/// <summary>
		/// Fetches data for given crypto coin for given date range from internets and does some analysis to that data.</br>
		/// </summary>
		/// <param name="fromDate">Start date for data search</param>
		/// <param name="toDate">End date for data search</param>
		/// <param name="coin">Crypto coin name</param>
		/// <param name="currency">Currency name</param>
		/// <returns></returns>
		public async Task<IMarketDataAnalysisResults> AnalyseMarkets(DateTime fromDate, DateTime toDate, string coin, string currency)
		{
			results = new();

			//Check toDate is not before or equal to fromDate
			if (toDate == fromDate)
			{
				results.SetFailAndMessage("End date can not be same as start date.");
				return results;
			}
			else if (toDate < fromDate)
			{
				results.SetFailAndMessage("End date can not be earlier than start date.");
				return results;
			}
			//Convert DateTimes to unix timestamps (Coingecko uses UTC-time)
			startDate = UnixTimeConverter.ConvertDateToUnixTimeStamp(fromDate);
			//Adjust end time so it is +1 hour 
			endDate = UnixTimeConverter.ConvertDateToUnixTimeStamp(toDate.AddHours(1));
			this.coin = coin;
			this.currency = currency;

			/* for debugging purposes
			string format = "yyyy-MM-dd HH:mm:ss.fff";
			System.Console.WriteLine($"fromDate: {fromDate.ToString(format)}");
			System.Console.WriteLine($"toDate: {toDate.ToString(format)}");
			System.Console.WriteLine($"toDate +1h: {toDate.AddHours(1).ToString(format)}");
			System.Console.WriteLine($"startDate: {startDate}");
			System.Console.WriteLine($"endDate: {endDate}");
			
			System.Console.WriteLine($"startDate (from unix): {UnixTimeConverter.ConvertUnixTimeStampToDate(startDate).ToString(format)}");
			System.Console.WriteLine($"endDate (from unix): {UnixTimeConverter.ConvertUnixTimeStampToDate(endDate).ToString(format)}");
			*/

			//Why use easy and simple .NET wrapper for CoinGecko API like this:
			//data = await geckoClient.CoinsClient.GetMarketChartRangeByCoinId(coin, currency, startDate, endDate);
			//when you can make your life harder like this?

			//Create url, download data and convert it JSON objects
			BuildUrl();
			await DownloadData();
			if (results.AnalysisSuccesfull == false)
			{
				return results;
			}

			ConvertJsonToObjects();
			//Answer to question: so we can do more consulting for customer 
			//=> more billable hours 
			//=> cash register goes KaChing!
			//Of course in real world scenario we would use the CoinGecko API
			//and bill the hours by not using it.

			if (marketData.Prices.Count == 0 && marketData.TotalVolumes.Count == 0)
			{
				results.SetFailAndMessage("There is no data to run analysis.");
			}

			AnalyseMarketData();
			// Dump results for debugging
			//DumpData();

			return results;
		}

		private void BuildUrl()
		{
			//Example url
			//https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from=1577836800&to=1609376400

			UriBuilder ub = new UriBuilder();
			ub.Scheme = "https:";
			ub.Host = $"api.coingecko.com/api/v3/coins/{coin}/market_chart/range";

			//construct parameters to url
			var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
			queryString.Add("vs_currency", currency);
			queryString.Add("from", startDate.ToString());
			queryString.Add("to", endDate.ToString());
			ub.Query = queryString.ToString();
			url = ub.Uri.ToString();
		}

		private async Task DownloadData()
		{
			try
			{
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				jsonResult = await response.Content.ReadAsStringAsync();
			}
			catch (HttpRequestException e)
			{
				results.AnalysisSuccesfull = false;
				results.Message = $"Downloading data failed: {e.Message}";
			}
		}

		private void ConvertJsonToObjects()
		{
			if (string.IsNullOrEmpty(jsonResult))
			{
				return;
			}
			//Convert first to root object
			RootObject root = JsonSerializer.Deserialize<RootObject>(jsonResult);
			marketData = new MarketData();
			//Convert root object arrays to datapoints and take only daily values from/near 00:00
			ConvertAndFilterData(root.prices, marketData.Prices);
			ConvertAndFilterData(root.total_volumes, marketData.TotalVolumes);
		}

		private void ConvertAndFilterData(decimal[][] fromCollection, List<Datapoint> toCollection)
		{
			// Notes from Coingecko:
			// Data granularity is automatic (cannot be adjusted)
			// 1 day from query time = 5 minute interval data
			// 1 - 90 days from query time = hourly data
			// above 90 days from query time = daily data (00:00 UTC)

			//Data from Coingecko is sorted from oldest to newest values.

			//Create dictionary for checking purposes
			Dictionary<string, DateTime> dates = new();

			for (int i = 0; i < fromCollection.Length; i++)
			{
				Datapoint dp = new()
				{
					UnixTimestamp = (long)fromCollection[i][0],
					Value = fromCollection[i][1]
				};
				string currentDate = dp.Date.ToShortDateString();
				//If date is not in dictionary 
				//=> Days first value nearest to midnight, add this datapoint to collection,
				//otherwise omit this datapoint.
				if (!dates.ContainsKey(currentDate))
				{
					dates.Add(currentDate, dp.Date);
					toCollection.Add(dp);
				}
			}
		}

		private void AnalyseMarketData()
		{
			//Bear trend results
			results.LongestDownwardTrendLength = MarketDataAnalyst.GetLongestDownwardTrendLength(marketData.Prices);
			//Highest volume results
			TradingVolumeDay tvd = MarketDataAnalyst.GetHighestTradingVolumeAndDay(marketData.TotalVolumes);
			tvd.Currency = currency;
			results.HighestTradingVolume = tvd;
			//Best day to buy and sell results.
			results.BestDaysToBuyAndSell = MarketDataAnalyst.GetBestDaysToBuyAndSell(marketData.Prices);
		}

		//For debugging purposes
		private void DumpData()
		{
			foreach (var item in marketData.Prices)
			{
				System.Console.WriteLine($"{item.Date}\t{item.Value}");
			}
		}

	}
}