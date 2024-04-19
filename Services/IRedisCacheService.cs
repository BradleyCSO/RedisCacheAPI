namespace PreferencesApi.Services.IServices;

public interface IRedisCacheService
{
    Task<bool> StoreDataInCacheAsync(string request, object responseBody);
    Task<string?> RetrieveDataFromCache(string request);
}