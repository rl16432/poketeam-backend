using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace msa_phase_3_backend.Repository.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly int AbsoluteExpirationInHours = 1;
        private readonly int SlidingExpirationInMinutes = 30;
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _cacheOptions;
        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;

            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(AbsoluteExpirationInHours),
                SlidingExpiration = TimeSpan.FromMinutes(SlidingExpirationInMinutes)
            };
        }

        public T? TryGet<T>(string cacheKey)
        {
            byte[] cachedData = _distributedCache.Get(cacheKey);
            T? deserialized = default;
            if (cachedData != null)
            {
                // If the data is found in the cache, encode and deserialize cached data.
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                deserialized = JsonSerializer.Deserialize<T>(cachedDataString);
            }
            return deserialized;
        }
        public void Set<T>(string cacheKey, T value)
        {
            string cachedDataString = JsonSerializer.Serialize(value);
            var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);
            
            _distributedCache.Set(cacheKey, dataToCache, _cacheOptions);
        }
        public void Remove(string cacheKey)
        {
            _distributedCache.Remove(cacheKey);
        }
    }
}
