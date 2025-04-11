using System.Text.Json.Serialization;

namespace HyperCube.Tests.Core.Contexts;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(TestClass))]
public partial class TestJsonContext : JsonSerializerContext
{

}
