using Microsoft.Extensions.Hosting;

namespace SteelCutOptimizer.Server.Structs
{
    public class OrderItem
    {
        public int Length { get; set; }
        public int Count { get; set; }
        public int? MaxRelax { get; set; }
        public bool? CanBeRelaxed { get; set; }

        public OrderItem() { }

        public OrderItem(OrderItem copyObj)
        {
            if (copyObj == null)
                throw new ArgumentNullException();

            Length = copyObj.Length;
            Count = copyObj.Count;
            MaxRelax = copyObj.MaxRelax;
            CanBeRelaxed = copyObj.CanBeRelaxed;
        }
    }
}
