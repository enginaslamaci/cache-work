using Redis.API.Enums;
using StackExchange.Redis;

namespace Redis.API.Services.Abstracts
{
    public interface IRedisService
    {
        Task<bool> KeyExists(string key);
        Task<string> GetValueAsync(string key);
        Task<bool> SetValueAsync(string key, string value);
        Task<RedisValue[]> GetListValueAsync(string key);
        Task<long> SetListValueAsync(string key, string value, ListPushSide pushSide);
        Task<RedisValue[]> GetSortedAsync(string key, int start, int stop, Order order);
        Task<bool> SetSortedAsync(string key, string value, int score);
        Task<HashEntry[]> GetHashAsync(string key);
        Task SetHashAsync(string key, HashEntry[] entries);
        Task Clear(string key);
        Task ClearAll();
    }
}
