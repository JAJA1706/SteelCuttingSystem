namespace SteelCutOptimizer.Server.Structs
{
    public class StockItem
    {
        public int Length { get; set; }
        public int? Count { get; set; }
        public int? Cost { get; set; }
        public bool? NextStepGeneration { get; set; }
    }
}
