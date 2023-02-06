namespace poketeam_backend.Repository.Caching
{
    public interface ICacheService
    {
        T? TryGet<T>(string cacheKey);
        void Set<T>(string cacheKey, T value);
        void Remove(string cacheKey);
    }
}
