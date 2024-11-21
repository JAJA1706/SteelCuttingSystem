namespace SteelCutOptimizer.Server.Structs
{
    public class Segment
    {
        public int OrderId { get; set; } //we need the orderId in case there is multiple orderItems with the same length
        public int Length { get; set; }
        public int RelaxAmount { get; set; }

        public Segment() { }
        public Segment(int orderId, int length, int relaxAmount)
        {
            OrderId = orderId;
            Length = length;
            RelaxAmount = relaxAmount;
        }
    }
}
