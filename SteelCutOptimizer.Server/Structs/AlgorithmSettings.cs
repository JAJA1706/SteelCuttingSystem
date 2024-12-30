namespace SteelCutOptimizer.Server.Structs
{
    public class AlgorithmSettings
    {
        public string MainObjective { get; set; } = "cost";
        public string RelaxationType { get; set; } = "manual";
        public string Solver { get; set; } = "cbc";

        public AlgorithmSettings() { }
        public AlgorithmSettings(AlgorithmSettings copyObj) 
        {
            MainObjective = copyObj.MainObjective;
            RelaxationType = copyObj.RelaxationType;
            Solver = copyObj.Solver;
        }
    }
}
