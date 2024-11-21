using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public interface IAmplDataConverterFactory
    {
        public IAmplDataConverter Create(AlgorithmSettings settings, string dataUniqueId);
    }
}
