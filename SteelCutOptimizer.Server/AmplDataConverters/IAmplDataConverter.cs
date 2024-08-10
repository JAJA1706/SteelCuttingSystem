using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.DTO;

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
