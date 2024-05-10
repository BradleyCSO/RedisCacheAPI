using PreferencesApi.Services.IServices;
using StackExchange.Redis;

namespace PreferencesApi.Services;

public class RedisCacheService(ConnectionMultiplexer connection) : IRedisCacheService
{
    public async Task<bool> StoreDataInCacheAsync(string request, object responseBody)
    {
        string cacheKey = GenerateCustomCacheRateLimitKey(request);

        try
        {
            return await connection.GetDatabase().StringSetAsync(cacheKey, responseBody.ToString());
        }
        finally 
        { 
            await connection.CloseAsync(); 
        }
    }

    public async Task<string?> RetrieveDataFromCache(string request)
    {
        string cacheKey = GenerateCustomCacheRateLimitKey(request);

        try
        {
            return await connection.GetDatabase().StringGetAsync(cacheKey);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private string GenerateCustomCacheRateLimitKey(string endpoint) => $"RateLimit:{endpoint}";
}