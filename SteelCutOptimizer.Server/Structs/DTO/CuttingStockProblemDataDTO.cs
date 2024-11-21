using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.DTO
{

    public class CuttingStockProblemDataDTO
    {
        public AlgorithmSettings AlgorithmSettings { get; set; } = new AlgorithmSettings();
        public List<StockItem>? StockList { get; set; }
        public List<OrderItem>? OrderList { get; set; }
        public List<Pattern>? Patterns {get; set;}
        public bool? AreBasicPatternsAllowed { get; set; } //used only in auto mode
        public List<double>? OrderPrices { get; set; } //used only in singleStep mode
        public List<double>? StockLimits { get; set; } //used only in singleStep mode
    }
}
