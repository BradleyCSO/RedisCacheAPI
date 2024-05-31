using StackExchange.Redis;

namespace RedisCacheAPI.Services;

public class RedisCacheService(ConnectionMultiplexer connection) : IRedisCacheService
{
    public async Task<bool> StoreDataInCacheAsync(string request, string responseBody, int expiry = 3600) =>
        await connection.GetDatabase().StringSetAsync(request, responseBody, TimeSpan.FromSeconds(expiry));

    public async Task<string?> RetrieveDataFromCache(string request) =>
        await connection.GetDatabase().StringGetAsync(request);

    public string GenerateCacheKey(HttpRequest request) =>
        $"{request.Path.Value}{request.QueryString}";
}