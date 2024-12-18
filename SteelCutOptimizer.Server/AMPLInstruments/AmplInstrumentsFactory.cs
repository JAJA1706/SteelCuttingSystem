using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public class AmplInstrumentsFactory : IAmplInstrumentsFactory
    {
        public IAmplDataValidator CreateValidator(AlgorithmSettings settings)
        {
            if (settings.MainObjective == "cost" || settings.MainObjective == "waste")
            {
                return new MultipleStockDataValidator();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public IAmplDataConverter CreateConverter(AlgorithmSettings settings, string dataUniqueId)
        {
            if (settings.MainObjective == "cost" || settings.MainObjective == "waste")
            {
                return new MultipleStockDataConverter(settings, dataUniqueId);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public IAmplApiService CreateApiService(AlgorithmSettings settings)
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
