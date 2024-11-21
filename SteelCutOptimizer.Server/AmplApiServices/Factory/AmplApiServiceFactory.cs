using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public class AmplApiServiceFactory : IAmplApiServiceFactory
    {
        public IAmplApiService Create( AlgorithmSettings settings )
        {
            if (settings.MainObjective == "cost" || settings.MainObjective == "waste")
            {
                if (settings.RelaxationType == "none")
                    return new MultipleStockService(settings);
                else
                    return new MultipleStockRelaxService(settings);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
