using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.DTO
{

    public class CuttingStockProblemDataDTO
    {
        public AlgorithmSettings AlgorithmSettings { get; set; } = new AlgorithmSettings();
        public List<StockItem>? StockList { get; set; }
        public List<OrderItem>? OrderList { get; set; }
        public List<Pattern>? Patterns {get; set; }   //used only in singleStep mode
        public bool? AreBasicPatternsAllowed { get; set; } //used only in auto mode
        public List<double>? OrderPrices { get; set; } //used only in singleStep mode
        public List<double>? StockLimits { get; set; } //used only in singleStep mode

        public CuttingStockProblemDataDTO(CuttingStockProblemDataDTO copyObj)
        {
            AlgorithmSettings = new AlgorithmSettings(copyObj.AlgorithmSettings);
            StockList = new List<StockItem>(copyObj.StockList ?? []);
            OrderList = new List<OrderItem>(copyObj.OrderList ?? []);
            Patterns = new List<Pattern>(copyObj.Patterns ?? []);
            AreBasicPatternsAllowed = copyObj.AreBasicPatternsAllowed;
            OrderPrices = new List<double>(copyObj.OrderPrices ?? []);
            StockLimits = new List<double>(copyObj.StockLimits ?? []);
        }
    }
}
