using System.Text.Json;
using System.Text.Json.Serialization;
using HyperCube.Core.Utils;
using HyperCube.Tests.Core.Contexts;

namespace HyperCube.Tests.Core;

[TestFixture]
public class JsonUtilsTests
{
    [Test]
    public void GetDefaultJsonSettings_ReturnsExpectedOptions()
    {
        // Act
        var options = JsonUtils.GetDefaultJsonSettings();

        // Assert
        Assert.That(options, Is.Not.Null);
        Assert.That(options.PropertyNamingPolicy, Is.EqualTo(JsonNamingPolicy.SnakeCaseLower));
        Assert.That(options.WriteIndented, Is.True);
        Assert.That(
            options.DefaultIgnoreCondition,
            Is.EqualTo(JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)
        );
        Assert.That(options.PropertyNameCaseInsensitive, Is.True);
        Assert.That(options.Converters, Has.Count.GreaterThan(0));
    }

    [Test]
    public void Serialize_ReturnsValidJson()
    {
        // Arrange
        var testObj = new TestClass
        {
            StringProperty = "Test",
            IntProperty = 42,
            BoolProperty = true,
            ListProperty = new List<string> { "item1", "item2" }
        };

        // Act
        var json = JsonUtils.Serialize(testObj);

        // Assert
        Assert.That(json, Is.Not.Null.Or.Empty);
        Assert.That(json, Does.Contain("string_property"));
        Assert.That(json, Does.Contain("int_property"));
        Assert.That(json, Does.Contain("bool_property"));
        Assert.That(json, Does.Contain("list_property"));
    }

    [Test]
    public void Deserialize_ReturnsValidObject()
    {
        // Arrange
        var json = @"{
            ""string_property"": ""Test"",
            ""int_property"": 42,
            ""bool_property"": true,
            ""list_property"": [""item1"", ""item2""]
        }";

        // Act
        var obj = JsonUtils.Deserialize<TestClass>(json);

        // Assert
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StringProperty, Is.EqualTo("Test"));
        Assert.That(obj.IntProperty, Is.EqualTo(42));
        Assert.That(obj.BoolProperty, Is.True);
        Assert.That(obj.ListProperty, Is.Not.Null);
        Assert.That(obj.ListProperty, Has.Count.EqualTo(2));
    }

    [Test]
    public void Serialize_WithJsonContext_ReturnsValidJson()
    {
        // Arrange
        var testObj = new TestClass
        {
            StringProperty = "Test",
            IntProperty = 42,
            BoolProperty = true,
            ListProperty = new List<string> { "item1", "item2" }
        };

        // Act
        var json = JsonUtils.Serialize(testObj, TestJsonContext.Default);

        // Assert
        Assert.That(json, Is.Not.Null.Or.Empty);
        Assert.That(json, Does.Contain("string_property"));
        Assert.That(json, Does.Contain("int_property"));
        Assert.That(json, Does.Contain("bool_property"));
        Assert.That(json, Does.Contain("list_property"));
    }

    [Test]
    public void Deserialize_WithJsonContext_ReturnsValidObject()
    {
        // Arrange
        var json = @"{
            ""string_property"": ""Test"",
            ""int_property"": 42,
            ""bool_property"": true,
            ""list_property"": [""item1"", ""item2""]
        }";

        // Act
        var obj = JsonUtils.Deserialize<TestClass>(json, TestJsonContext.Default);

        // Assert
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StringProperty, Is.EqualTo("Test"));
        Assert.That(obj.IntProperty, Is.EqualTo(42));
        Assert.That(obj.BoolProperty, Is.True);
        Assert.That(obj.ListProperty, Is.Not.Null);
        Assert.That(obj.ListProperty, Has.Count.EqualTo(2));
    }

    [Test]
    public void Deserialize_WithType_ReturnsValidObject()
    {
        // Arrange
        var json = @"{
            ""string_property"": ""Test"",
            ""int_property"": 42,
            ""bool_property"": true,
            ""list_property"": [""item1"", ""item2""]
        }";

        // Act
        var obj = JsonUtils.Deserialize(json, typeof(TestClass));

        // Assert
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj, Is.TypeOf<TestClass>());

        var typedObj = (TestClass)obj!;
        Assert.That(typedObj.StringProperty, Is.EqualTo("Test"));
        Assert.That(typedObj.IntProperty, Is.EqualTo(42));
        Assert.That(typedObj.BoolProperty, Is.True);
        Assert.That(typedObj.ListProperty, Is.Not.Null);
        Assert.That(typedObj.ListProperty, Has.Count.EqualTo(2));
    }

    [Test]
    public void Deserialize_WithTypeAndContext_ReturnsValidObject()
    {
        // Arrange
        var json = @"{
            ""string_property"": ""Test"",
            ""int_property"": 42,
            ""bool_property"": true,
            ""list_property"": [""item1"", ""item2""]
        }";

        // Act
        var obj = JsonUtils.Deserialize(json, typeof(TestClass), TestJsonContext.Default);

        // Assert
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj, Is.TypeOf<TestClass>());

        var typedObj = (TestClass)obj!;
        Assert.That(typedObj.StringProperty, Is.EqualTo("Test"));
        Assert.That(typedObj.IntProperty, Is.EqualTo(42));
        Assert.That(typedObj.BoolProperty, Is.True);
        Assert.That(typedObj.ListProperty, Is.Not.Null);
        Assert.That(typedObj.ListProperty, Has.Count.EqualTo(2));
    }
}
