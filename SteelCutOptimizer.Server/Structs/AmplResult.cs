namespace SteelCutOptimizer.Server.Structs
{
    public class AmplResult
    {
        public bool IsFeasible { get; set; } = true;
        public int? TotalCost { get; set; }
        public int? TotalWaste { get; set; }
        public Dictionary<Tuple<int, int>, Pattern> Patterns { get; set; } = [];
        public List<double> OrderPrices { get; set; } = [];
        public List<double> StockLimits { get; set; } = [];
    }
}
