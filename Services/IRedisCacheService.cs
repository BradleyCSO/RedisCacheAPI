namespace RedisCacheAPI.Services;

public interface IRedisCacheService
{
    Task<bool> StoreDataInCacheAsync(string request, string responseBody);
    Task<string?> RetrieveDataFromCache(string request);
    string GenerateCacheKey(HttpRequest request);
}