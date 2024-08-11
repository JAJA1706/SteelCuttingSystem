namespace SteelCutOptimizer.Server.Structs
{
    public class AmplResult
    {
        public int TotalCost { get; set; }
        public Dictionary<Tuple<int, int>, Pattern> Patterns { get; set; } = [];
    }
}
