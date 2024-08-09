using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.Enums;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public class AmplDataConverterFactory : IAmplDataConverterFactory
    {
        public IAmplDataConverter Create( CuttingStockProblemType problemType )
        {
            switch (problemType)
            {
                case CuttingStockProblemType.MultipleStock:
                    return new MultipleStockDataConverter();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
