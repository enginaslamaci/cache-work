using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis.API.Services.Abstracts;

namespace Redis.API.Controllers
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
        private readonly IRedisService _redisService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;
        }

        [HttpPost]
        public async Task Post() //manual set forecast
        {
            var forecasts = await GetForecasts();
            await _redisService.SetValueAsync("weather", JsonConvert.SerializeObject(forecasts));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _redisService.GetValueAsync("weather");
            var weatherForecast = result != null ? JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(result) : new List<WeatherForecast>();

            if (!weatherForecast.Any() || DateTime.Now.Day >= weatherForecast.FirstOrDefault().Date.Day)
            {
                var forecasts = await GetForecasts();
                await _redisService.SetValueAsync("weather", JsonConvert.SerializeObject(forecasts));
                return Ok(forecasts);
            }
            return Ok(weatherForecast);
        }


        [HttpDelete]
        public async Task ClearCache(string key)
        {
            await _redisService.Clear(key);
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
