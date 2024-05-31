using System.Text;
using RedisCacheAPI.Models;
using RedisCacheAPI.Services;
using StackExchange.Redis;

WebApplicationBuilder? builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>(provider =>
{
    ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost,abortConnect=false");
    return new RedisCacheService(connection);
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
    options.SerializerOptions.WriteIndented = false;
});

WebApplication? app = builder.Build();

app.MapPost("{**endpoint}", async (HttpContext context, IRedisCacheService redisCacheService) =>
{
	// Read the stream (JSON representation) we got from the request body into a string
	using (StreamReader reader = new(context.Request.Body, Encoding.UTF8))
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

	if (cachedData != null)
        return Results.Content(cachedData);

    return Results.NotFound();
});

app.Run();