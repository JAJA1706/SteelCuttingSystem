using SteelCutOptimizer.Server.Enums;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public interface IAmplDataConverterFactory
    {
        public IAmplDataConverter Create( CuttingStockProblemType problemType );
    }
}
