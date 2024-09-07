using System.Text.Json.Serialization;

namespace RedisCacheAPI.Models;

[JsonSerializable(typeof(string))]
internal partial class SourceGenerationContext : JsonSerializerContext { }