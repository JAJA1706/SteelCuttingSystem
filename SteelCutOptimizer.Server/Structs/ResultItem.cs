namespace SteelCutOptimizer.Server.Structs
{
    public class ResultItem
    {
        public int PatternId { get; set; }
        public int StockLength { get; set; }
        public int Count { get; set; }
        public List<RelaxableLength> UsedOrderLengths { get; set; } = [];
    }
}
