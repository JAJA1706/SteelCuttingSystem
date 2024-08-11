namespace SteelCutOptimizer.Server.Structs
{
    public class RelaxableLength
    {
        public int Length { get; set; }
        public int RelaxAmount { get; set; }

        public RelaxableLength() { }
        public RelaxableLength(int length, int relaxAmount)
        {
            Length = length;
            RelaxAmount = relaxAmount;
        }
    }
}
