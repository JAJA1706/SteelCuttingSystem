namespace SteelCutOptimizer.Server.Structs
{
    public class StockItem
    {
        public int Length { get; set; }
        public int? Count { get; set; }
        public int? Cost { get; set; }
        public bool? NextStepGeneration { get; set; }

        public StockItem() {}

        public StockItem(StockItem copyObj)
        {
            if (copyObj == null)
                throw new ArgumentNullException();

            Length = copyObj.Length;
            Count = copyObj.Count;
            Cost = copyObj.Cost;
            NextStepGeneration = copyObj.NextStepGeneration;
        }
    }
}
