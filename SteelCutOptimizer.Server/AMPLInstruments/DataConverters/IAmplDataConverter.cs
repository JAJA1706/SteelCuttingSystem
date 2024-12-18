using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public interface IAmplDataConverter
    {
        void AdjustEntryData(CuttingStockProblemDataDTO data);
        string ConvertToAmplDataFile(CuttingStockProblemDataDTO data);
        void DisposeDataFile();
        CuttingStockResultsDTO ConvertResultDataToDTO(AmplResult amplResult);
    }
}
