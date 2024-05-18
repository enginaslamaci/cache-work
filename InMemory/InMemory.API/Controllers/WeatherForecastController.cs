using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.API.Controllers
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
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddHours(6),
                Priority = CacheItemPriority.Normal
            };
        }

        [HttpPost]
        public async Task<IActionResult> Post() //manual set forecast
        {
            var forecasts = await GetForecasts();
            _memoryCache.Set<IEnumerable<WeatherForecast>>("weather", forecasts, _cacheOptions);

            return Ok("set forecast manually");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = _memoryCache.TryGetValue<IEnumerable<WeatherForecast>>("weather", out IEnumerable<WeatherForecast>? weatherForecasts);
            if (!result || DateTime.Now.Day >= weatherForecasts.FirstOrDefault().Date.Day)
            {
                var forecasts = await GetForecasts();
                _memoryCache.Set<IEnumerable<WeatherForecast>>("weather", forecasts, _cacheOptions);
                return Ok(forecasts);
            }
            return Ok(weatherForecasts);
        }


        private async Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
