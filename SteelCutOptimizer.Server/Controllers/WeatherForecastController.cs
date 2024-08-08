using ampl;
using ampl.Entities;
using Microsoft.AspNetCore.Mvc;

namespace SteelCutOptimizer.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            String modelDirectory = "C:\\Programowanie\\Source\\Magisterka\\AMPL\\singleStock";

            using (AMPL a = new AMPL())
            {
                // Interpret files
                a.Read(System.IO.Path.Combine(modelDirectory, "cut.mod"));
                a.ReadData(System.IO.Path.Combine(modelDirectory, "cut.dat"));
                a.Read(System.IO.Path.Combine(modelDirectory, "cut.run"));

                Objective totalcost = a.GetObjective("Number");
                Variable lol = a.GetVariable("Cut");
                Console.WriteLine(totalcost.Value);
            }

            return new List<WeatherForecast>();
        }
    }
}
