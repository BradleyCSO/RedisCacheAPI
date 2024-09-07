using StackExchange.Redis;

namespace RedisCacheAPI.Services;

/// <summary>
///     Service responsible for storing/getting data from a Redis cache instance
/// </summary>
public interface IRedisCacheService
{
    /// <summary>
    ///     Responsible for setting items to a Redis cache instance
    /// </summary>
    /// <param name="request">The request's endpoint: used to set the cache's key</param>
    /// <param name="responseBody">The response body we're caching</param>
    /// <param name="expiry">TTL: The duration (in seconds) that the entry stays stored in the cache: default 3600 seconds (1 hour)</param>
    /// <returns>Boolean: true if the item was stored, else false</returns>
    Task<bool> StoreDataInCacheAsync(string request, string responseBody, int expiry = 3600);

    /// <summary>
    ///     Responsible for getting items from a Redis cache instance
    /// </summary>
    /// <param name="request">The request's endpoint (cache key) to get the response body for</param>
    /// <returns>The response body if one is stored, else null</returns>
    Task<string?> RetrieveDataFromCache(string request);

    /// <summary>
    ///     Responsible for generating a cache key given a request: 
    ///     used for both getting/setting data to the Redis cache instance
    /// </summary>
    /// <param name="request">The cache key to generate for a given request</param>
    /// <returns>The generated cache key provided a valid <see cref="HttpRequest"/></returns>
    string GenerateCacheKey(HttpRequest request);
}

public class RedisCacheService(ConnectionMultiplexer connection) : IRedisCacheService
{
    public async Task<bool> StoreDataInCacheAsync(string request, string responseBody, int expiry = 3600) =>
        await connection.GetDatabase().StringSetAsync(request, responseBody, TimeSpan.FromSeconds(expiry));

    public async Task<string?> RetrieveDataFromCache(string request) =>
        await connection.GetDatabase().StringGetAsync(request);

    public string GenerateCacheKey(HttpRequest request) =>
        $"{request.Path.Value}{request.QueryString}";
}