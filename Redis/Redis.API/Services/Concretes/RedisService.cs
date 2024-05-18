using Redis.API.Enums;
using Redis.API.Services.Abstracts;
using StackExchange.Redis;

namespace Redis.API.Services.Concretes
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _cache;

        public RedisService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _cache = redisConnection.GetDatabase(1);
        }


        public async Task<bool> KeyExists(string key)
        {
            return await _cache.KeyExistsAsync(key);
        }

        public async Task<string> GetValueAsync(string key)
        {
            return await _cache.StringGetAsync(key);
        }

        public async Task<bool> SetValueAsync(string key, string value)
        {
            return await _cache.StringSetAsync(key, value, TimeSpan.FromHours(1));
        }

        public async Task<RedisValue[]> GetListValueAsync(string key)
        {
            return await _cache.ListRangeAsync(key);
        }

        public async Task<long> SetListValueAsync(string key, string value, ListPushSide pushSide)
        {
            switch (pushSide)
            {
                case ListPushSide.Left:
                    return await _cache.ListLeftPushAsync(key, value);

                case ListPushSide.Right:
                    return await _cache.ListRightPushAsync(key, value);

                default:
                    return await _cache.ListRightPushAsync(key, value);
            }

        }

        public async Task<RedisValue[]> GetSortedAsync(string key, int start, int stop, Order order)
        {
            return await _cache.SortedSetRangeByScoreAsync(key, start, stop, order: order);
        }

        public async Task<bool> SetSortedAsync(string key, string value, int score)
        {
            return await _cache.SortedSetAddAsync(key, value, score);
        }


        public async Task<HashEntry[]> GetHashAsync(string key)
        {
            return await _cache.HashGetAllAsync(key);
        }

        public async Task SetHashAsync(string key, HashEntry[] entries)
        {
            await _cache.HashSetAsync(key, entries);
        }


        public async Task Clear(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task ClearAll()
        {
            var redisEndpoints = _redisConnection.GetEndPoints(true);
            foreach (var redisEndpoint in redisEndpoints)
            {
                var redisServer = _redisConnection.GetServer(redisEndpoint);
                redisServer.FlushAllDatabases();
            }
        }
    }
}
