namespace SteelCutOptimizer.Server.DTO
{
    public class ResultItem
    {
        public int PatternId { get; set; }
        public int StockLength { get; set; }
        public int Count { get; set; }
        public List<int> UsedOrderLengths { get; set; } = [];
    }

    public class CuttingStockResultsDTO
    {
        public List<ResultItem> ResultItems { get; set; } = [];
    }
}
