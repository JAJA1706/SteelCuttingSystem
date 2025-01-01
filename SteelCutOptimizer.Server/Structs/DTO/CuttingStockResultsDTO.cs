using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.DTO
{
    public class CuttingStockResultsDTO
    {
        public long? TotalCost { get; set; }
        public long TotalWaste { get; set; }
        public long? TotalRelax { get; set; }
        public List<ResultPattern> Patterns { get; set; } = [];
        public List<double> OrderPrices { get; set; } = [];
        public List<double> StockLimits { get; set; } = [];
    }
}
