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
    bool writtenToCache = await redisCacheService.StoreDataInCacheAsync(context.Request.Path.Value, context.Response.Body);

    if (writtenToCache)
        return Results.Created();
    else
        return Results.BadRequest($"Couldn't write {context.Response.Body} to cache.");
});

app.MapGet("{**endpoint}", async (HttpContext context, IRedisCacheService redisCacheService) =>
{
    string? cachedData = await redisCacheService.RetrieveDataFromCache(context.Request.Path.Value);

    if (cachedData != null)
        return Results.Ok();
    else
        return Results.NotFound();
});

app.Run();