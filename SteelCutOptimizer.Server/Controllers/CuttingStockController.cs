using Microsoft.AspNetCore.Mvc;
using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.AmplDataConverters;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Enums;
using SteelCutOptimizer.Server.Structs;
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
            String tempDirectory = AppDomain.CurrentDomain.BaseDirectory + "data/multipleStockExtended/";
            if(!Enum.TryParse(problemData.ProblemType, out CuttingStockProblemType problemType ))
            {
                return BadRequest("Invalid problem type");
            }

            var amplDataConverter = _amplDataConverterFactory.Create(problemType);
            try
            {
                amplDataConverter.AdjustEntryData(problemData);
                amplDataConverter.ConvertToAmplDataFile(tempDirectory, problemData);

                var amplApiService = _amplApiServiceFactory.Create(problemType);
                AmplResult results = amplApiService.SolveCuttingStockProblem(tempDirectory);

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
        }
    }
}
