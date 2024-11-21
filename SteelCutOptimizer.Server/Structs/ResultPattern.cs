namespace SteelCutOptimizer.Server.Structs
{
    public class ResultPattern
    {
        public int PatternId { get; set; }
        public int StockId { get; set; } //we need the stockId in case there is multiple StockItems with the same length
        public int StockLength { get; set; }
        public int Count { get; set; }
        public List<Segment> SegmentList { get; set; } = [];
    }
}
