using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RedisCacheAPI.Services;
using StackExchange.Redis;


WebApplicationBuilder? builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>(provider =>
{
    ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost,abortConnect=false");
    return new RedisCacheService(connection);
});

WebApplication? app = builder.Build();

app.MapPost("{**endpoint}", async (HttpContext context, IRedisCacheService redisCacheService) =>
{
    // Read the stream (JSON representation) we got from the request body into a string
    using (StreamReader reader = new(context.Request.Body, Encoding.UTF8, true, 1024, true))
    {
        string responseBody = await reader.ReadToEndAsync();
                
        // We now have the request body we got from the sender as a string, so store it in the cache
        if (await redisCacheService.StoreDataInCacheAsync(redisCacheService.GenerateCacheKey(context.Request), responseBody))
            return Results.Created();
    }
            
    return Results.BadRequest($"Couldn't write {context.Response.Body} to cache.");
});

app.MapGet("{**endpoint}", async (HttpContext context, IRedisCacheService redisCacheService) =>
{
    string? cachedData = await redisCacheService.RetrieveDataFromCache(redisCacheService.GenerateCacheKey(context.Request));

    var options = new JsonSerializerOptions();
    options.TypeInfoResolver = new SourceGenerationContext();

    cachedData = JsonSerializer.Serialize<object>(cachedData, options);
        
    if (cachedData != null)
        return Results.Ok(cachedData);

    return Results.NotFound();
});

app.Run();

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(String))]
[JsonSerializable(typeof(Object))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}