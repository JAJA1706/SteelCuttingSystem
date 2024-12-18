using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public interface IAmplApiService
    {
        public AmplResult SolveCuttingStockProblem(string dataFilePath);
    }
}
