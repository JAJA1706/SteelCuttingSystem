namespace SteelCutOptimizer.Server.DTO
{
    public class Stock
    {
        public int Length { get; set; }
        public int? Count { get; set; }
        public int? Cost { get; set; }
    }
    public class Order
    {
        public int Length { get; set; }
        public int Count { get; set; }
        public int? MaxRelax { get; set; }
    }

    public class CuttingStockProblemDataDTO
    {
        public string? ProblemType { get; set; }
        public List<Stock>? StockList { get; set; }
        public List<Order>? OrderList { get; set; }
    }
}
