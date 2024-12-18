using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public interface IAmplInstrumentsFactory
    {
        public IAmplDataValidator CreateValidator(AlgorithmSettings settings);
        public IAmplDataConverter CreateConverter(AlgorithmSettings settings, string dataUniqueId);
        public IAmplApiService CreateApiService(AlgorithmSettings settings);
    }
}
