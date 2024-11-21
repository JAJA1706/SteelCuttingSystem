using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.DTO
{
    public class CuttingStockResultsDTO
    {
        public int? TotalCost { get; set; }
        public int TotalWaste { get; set; }
        public int? TotalRelax { get; set; }
        public List<ResultPattern> Patterns { get; set; } = [];
        public List<double> OrderPrices { get; set; } = [];
        public List<double> StockLimits { get; set; } = [];
    }
}
