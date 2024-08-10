namespace SteelCutOptimizer.Server.AmplApiServices
{
    public class Pattern
    {
        public int StockLength { get; set; }
        public int UseCount { get; set; }
        public List<int> UsedOrderLengths { get; set; } = [];

        public Pattern()
        {
            StockLength = 0;
            UseCount = 0;
            UsedOrderLengths = new List<int>();
        }

        public Pattern(int stockLength, int useCount, List<int> usedOrderLengths)
        {
            StockLength = stockLength;
            UseCount = useCount;
            UsedOrderLengths.AddRange(usedOrderLengths);
        }
    }
    public class AmplResult
    {
        public int TotalCost { get; set; }
        public Dictionary<Tuple<int, int>, Pattern> Patterns { get; set; } = [];
    }

    public interface IAmplApiService
    {
        public AmplResult SolveCuttingStockProblem(string dataFilePath);
    }
}
