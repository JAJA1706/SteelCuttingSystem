namespace SteelCutOptimizer.Server.Structs
{
    public class Pattern
    {
        public int StockLength { get; set; }
        public int UseCount { get; set; }
        public List<RelaxableLength> UsedOrderLengths { get; set; } = [];

        public Pattern()
        {
            StockLength = 0;
            UseCount = 0;
            UsedOrderLengths = new List<RelaxableLength>();
        }

        public Pattern(int stockLength, int useCount, List<RelaxableLength> usedOrderLengths)
        {
            StockLength = stockLength;
            UseCount = useCount;
            UsedOrderLengths.AddRange(usedOrderLengths);
        }
    }
}
