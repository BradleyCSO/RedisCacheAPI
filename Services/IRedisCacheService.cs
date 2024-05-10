namespace RedisCacheAPI.Services;

public interface IRedisCacheService
{
    Task<bool> StoreDataInCacheAsync(string request, object responseBody);
    Task<string?> RetrieveDataFromCache(string request);
}