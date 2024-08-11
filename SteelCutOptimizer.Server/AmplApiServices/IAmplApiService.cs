using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public interface IAmplApiService
    {
        public AmplResult SolveCuttingStockProblem(string dataFilePath);
    }
}
