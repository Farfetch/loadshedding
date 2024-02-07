using Farfetch.LoadShedding.PerformanceTests.Models;
using Microsoft.AspNetCore.Mvc;

namespace Farfetch.LoadShedding.PerformanceTests.Controllers
{
    [ApiController]
    [Route("api")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] s_summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        [HttpGet("GetWeather")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = s_summaries[Random.Shared.Next(s_summaries.Length)],
            })
            .ToArray();
        }

        [HttpGet("GetWeatherDelayed")]
        public async Task<IEnumerable<WeatherForecast>> GetDelayedAsync()
        {
            await Task.Delay(3000);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = s_summaries[Random.Shared.Next(s_summaries.Length)],
            })
            .ToArray();
        }
    }
}
