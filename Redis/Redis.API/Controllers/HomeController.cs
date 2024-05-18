using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis.API.Enums;
using Redis.API.Services.Abstracts;
using StackExchange.Redis;

namespace Redis.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IRedisService _redisService;

        public HomeController(IRedisService redisService)
        {
            _redisService = redisService;
        }


        [HttpPost("list")]
        public async Task SetList(string value, ListPushSide pushSide)
        {
            await _redisService.SetListValueAsync("list1", value, pushSide);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            if (await _redisService.KeyExists("list1"))
            {
                var result = await _redisService.GetListValueAsync("list1");
                return Ok(result.ToStringArray());
            }
            return NotFound();
        }


        /// <summary>
        /// values hold as unique. if exists value when save, not create new record only score update
        /// </summary>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        [HttpPost("sorted-set")]
        public async Task SortedSet(string value, int score)
        {
            await _redisService.SetSortedAsync("sorted1", value, score);
        }


        [HttpGet("sorted-get")]
        public async Task<IActionResult> SortedGet(Order order)
        {
            if (await _redisService.KeyExists("sorted1"))
            {
                var result = await _redisService.GetSortedAsync("sorted1", 0, 50, order);
                return Ok(result.ToStringArray());
            }
            return NotFound();
        }


        /// <summary>
        /// values hold as key-value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("hash-set")]
        public async Task HashSet()
        {
            var entries = new HashEntry[]
            {
                new HashEntry("code","xyz"),
                new HashEntry("name","pencil"),
                new HashEntry("stock",100)
            };

            await _redisService.SetHashAsync("hash1", entries);
        }


        [HttpGet("hash-get")]
        public async Task<IActionResult> HashGet()
        {
            if (await _redisService.KeyExists("hash1"))
            {
                var result = await _redisService.GetHashAsync("hash1");
                return Ok(result.ToStringDictionary());
            }
            return NotFound();
        }



        [HttpDelete]
        public async Task ClearCache(string key)
        {
            await _redisService.Clear(key);
        }

    }
}
