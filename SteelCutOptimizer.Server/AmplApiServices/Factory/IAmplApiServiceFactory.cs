using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public interface IAmplApiServiceFactory
    {
        public IAmplApiService Create( AlgorithmSettings settings );
    }
}
