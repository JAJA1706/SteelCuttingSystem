using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.DTO
{

    public class CuttingStockProblemDataDTO
    {
        public AlgorithmSettings AlgorithmSettings { get; set; } = new AlgorithmSettings();
        public List<StockItem>? StockList { get; set; }
        public List<OrderItem>? OrderList { get; set; }
        public List<Pattern>? Patterns {get; set; }         //used only in singleStep mode
        public bool? AreBasicPatternsAllowed { get; set; }  //used only in auto mode
        public List<double>? OrderPrices { get; set; }      //used only in singleStep mode
        public List<double>? StockLimits { get; set; }      //used only in singleStep mode
        public double? RelaxCostMultiplier { get; set; }    //used in manual and auto mode

        public CuttingStockProblemDataDTO(){}
        public CuttingStockProblemDataDTO(CuttingStockProblemDataDTO copyObj)
        {
            if (copyObj == null)
                throw new ArgumentNullException();

            AlgorithmSettings = new AlgorithmSettings(copyObj.AlgorithmSettings);
            StockList = copyObj.StockList?.Select(item => new StockItem(item)).ToList();
            OrderList = copyObj.OrderList?.Select(item => new OrderItem(item)).ToList();
            Patterns = copyObj.Patterns?.Select(pattern => new Pattern(pattern)).ToList();
            AreBasicPatternsAllowed = copyObj.AreBasicPatternsAllowed;
            OrderPrices = copyObj.OrderPrices?.ToList();
            StockLimits = copyObj.StockLimits?.ToList();
            RelaxCostMultiplier = copyObj.RelaxCostMultiplier;
        }
    }
}
