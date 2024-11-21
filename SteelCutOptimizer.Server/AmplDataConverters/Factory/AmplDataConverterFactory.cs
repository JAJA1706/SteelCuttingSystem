using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public class AmplDataConverterFactory : IAmplDataConverterFactory
    {
        public IAmplDataConverter Create( AlgorithmSettings settings, string dataUniqueId )
        {
            if(settings.MainObjective == "cost" || settings.MainObjective == "waste")
            {
                return new MultipleStockDataConverter(settings, dataUniqueId);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
