namespace SteelCutOptimizer.Server.AmplApiServices
{
    public class AmplResult
    {

    }

    public interface IAmplApiService
    {
        public AmplResult SolveCuttingStockProblem(string dataFilePath);
    }
}
