using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public interface IAmplDataValidator
    {
        void ValidateEntryData(CuttingStockProblemDataDTO entryData);
        void ValidateResultData(AmplResult amplResult, CuttingStockProblemDataDTO entryData);
    }
}
