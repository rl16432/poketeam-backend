using Microsoft.Extensions.Caching.Memory;

namespace poketeam_backend.Repository.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly int AbsoluteExpirationInSeconds = 120;
        private readonly int SlidingExpirationInSeconds = 30;
        
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(AbsoluteExpirationInSeconds),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromMinutes(SlidingExpirationInSeconds)
            };

        }
        public T? TryGet<T>(string cacheKey)
        {
            var value = _memoryCache.Get<T>(cacheKey);
            return value;
        }
        public void Set<T>(string cacheKey, T value)
        {
            _memoryCache.Set(cacheKey, value, _cacheOptions);
        }
        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }
    }
}
