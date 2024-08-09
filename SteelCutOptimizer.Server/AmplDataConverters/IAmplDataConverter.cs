using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.DTO;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public interface IAmplDataConverter
    {
        void ConvertToAmplDataFile(string dataFilePath, CuttingStockProblemDataDTO data);
        CuttingStockResultsDTO ConvertResultDataToDTO(AmplResult result);
    }
}
