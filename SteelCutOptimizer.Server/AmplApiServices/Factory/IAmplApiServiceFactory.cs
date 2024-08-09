using SteelCutOptimizer.Server.Enums;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public interface IAmplApiServiceFactory
    {
        public IAmplApiService Create( CuttingStockProblemType problemType );
    }
}
