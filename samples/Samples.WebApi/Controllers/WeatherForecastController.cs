using Farfetch.LoadShedding.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Samples.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMongoCollection<WeatherForecast> _collection;

        public WeatherForecastController(IMongoCollection<WeatherForecast> collection)
        {
            this._collection = collection;
        }

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
