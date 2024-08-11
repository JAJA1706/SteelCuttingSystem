using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public interface IAmplDataConverter
    {
        void AdjustEntryData(CuttingStockProblemDataDTO data);
        void ConvertToAmplDataFile(string dataFilePath, CuttingStockProblemDataDTO data);
        void ValidateResultData(AmplResult amplResult, CuttingStockProblemDataDTO entryData);
        CuttingStockResultsDTO ConvertResultDataToDTO(AmplResult amplResult);
    }
}
