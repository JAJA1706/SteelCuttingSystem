using SteelCutOptimizer.Server.Enums;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public class AmplApiServiceFactory : IAmplApiServiceFactory
    {
        public IAmplApiService Create( CuttingStockProblemType problemType )
        {
            switch (problemType)
            {
                case CuttingStockProblemType.MultipleStock:
                    return new MultipleStockService();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
