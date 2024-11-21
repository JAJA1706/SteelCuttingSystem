namespace SteelCutOptimizer.Server.Structs
{
    public class OrderItem
    {
        public int Length { get; set; }
        public int Count { get; set; }
        public int? MaxRelax { get; set; }
        public bool? CanBeRelaxed { get; set; }
    }
}
