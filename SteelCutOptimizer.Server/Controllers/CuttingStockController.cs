using Microsoft.AspNetCore.Mvc;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Utils;
using SteelCutOptimizer.Server.AMPLInstruments;

namespace SteelCutOptimizer.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CuttingStockController : ControllerBase
    {
        private readonly ILogger<CuttingStockController> _logger;
        private readonly IAmplInstrumentsFactory _amplInstrumentsFactory;

        public CuttingStockController(
            ILogger<CuttingStockController> logger,
            IAmplInstrumentsFactory amplInstrumentsFactory)
        {
            _logger = logger;
            _amplInstrumentsFactory = amplInstrumentsFactory;
        }

        [HttpPost]
        public IActionResult solve([FromBody] CuttingStockProblemDataDTO problemData)
        {
            try
            {
                var settings = problemData.AlgorithmSettings;
                UniqueID uniqueId = new UniqueID();
                var dataConverter = _amplInstrumentsFactory.CreateConverter(settings, uniqueId.Get());
                var dataValidator = _amplInstrumentsFactory.CreateValidator(settings);

                dataValidator.ValidateEntryData(problemData);
                dataConverter.AdjustEntryData(problemData);
                string dataFilePath = dataConverter.ConvertToAmplDataFile(problemData);


                var apiService = _amplInstrumentsFactory.CreateApiService(settings);
                AmplResult results = apiService.SolveCuttingStockProblem(dataFilePath);

                dataConverter.DisposeDataFile();
                dataValidator.ValidateResultData(results, problemData);
                var dto = dataConverter.ConvertResultDataToDTO(results);

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
