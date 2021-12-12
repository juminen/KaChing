namespace KaChing.Model
{
	public class MarketData
	{
		public List<Datapoint> Prices { get; set; } = new();
		public List<Datapoint> TotalVolumes { get; set; } = new();
		//For possible future use: public List<Datapoint> MarketCaps { get; set; } = new();
	}
}