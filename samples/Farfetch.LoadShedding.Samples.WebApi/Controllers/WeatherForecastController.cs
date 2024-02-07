using Farfetch.LoadShedding.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Farfetch.LoadShedding.Samples.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMongoCollection<WeatherForecast> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastController"/> class.
        /// </summary>
        /// <param name="collection"></param>
        public WeatherForecastController(IMongoCollection<WeatherForecast> collection)
        {
            _collection = collection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet(Name = "GetWeatherForecast")]
        [EndpointPriority(Farfetch.LoadShedding.Tasks.Priority.Critical)]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            return await _collection
                .Find(FilterDefinition<WeatherForecast>.Empty)
                .ToListAsync();
        }
    }
}
