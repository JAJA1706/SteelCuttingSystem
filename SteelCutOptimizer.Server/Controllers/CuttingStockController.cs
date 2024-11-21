using Microsoft.AspNetCore.Mvc;
using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.AmplDataConverters;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Utils;
using System.Text.Json;

namespace SteelCutOptimizer.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CuttingStockController : ControllerBase
    {
        private readonly ILogger<CuttingStockController> _logger;
        private readonly IAmplDataConverterFactory _amplDataConverterFactory;
        private readonly IAmplApiServiceFactory _amplApiServiceFactory;

        public CuttingStockController(
            ILogger<CuttingStockController> logger, 
            IAmplDataConverterFactory amplDataConverterFactory,
            IAmplApiServiceFactory amplApiServiceFactory)
        {
            _logger = logger;
            _amplDataConverterFactory = amplDataConverterFactory;
            _amplApiServiceFactory = amplApiServiceFactory;
        }

        [HttpPost]
        public IActionResult solve([FromBody] CuttingStockProblemDataDTO problemData)
        {
            try
            {
                var settings = problemData.AlgorithmSettings;
                UniqueID uniqueId = new UniqueID();
                var amplDataConverter = _amplDataConverterFactory.Create(settings, uniqueId.Get());

                amplDataConverter.AdjustEntryData(problemData);
                string dataFilePath = amplDataConverter.ConvertToAmplDataFile(problemData);


                var amplApiService = _amplApiServiceFactory.Create(settings);
                AmplResult results = amplApiService.SolveCuttingStockProblem(dataFilePath);

                amplDataConverter.DisposeDataFile();
                amplDataConverter.ValidateResultData(results, problemData);
                var dto = amplDataConverter.ConvertResultDataToDTO(results);

                return Ok(dto);
            }
            catch (AMPLException exc)
            {
                return BadRequest(exc.Message);
            }
            catch (InvalidDataException exc)
            {
                return BadRequest(exc.Message);
            }
            catch(NotImplementedException exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}
