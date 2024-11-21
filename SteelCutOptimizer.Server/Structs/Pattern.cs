namespace SteelCutOptimizer.Server.Structs
{
    public class Pattern
    {
        public int StockId { get; set; } //we need the stockId in case there is multiple StockItems with the same length
        public int StockLength { get; set; }
        public int UseCount { get; set; }
        public List<Segment> SegmentList { get; set; } = [];

        public Pattern()
        {
            StockId = 0;
            StockLength = 0;
            UseCount = 0;
            SegmentList = new List<Segment>();
        }

        public Pattern(int stockId, int stockLength, int useCount, List<Segment> listOfSegments)
        {
            StockId = stockId;
            StockLength = stockLength;
            UseCount = useCount;
            SegmentList.AddRange(listOfSegments);
        }
    }
}
