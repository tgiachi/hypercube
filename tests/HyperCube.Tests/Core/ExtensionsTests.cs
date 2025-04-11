using HyperCube.Tests.Core.Contexts;

namespace HyperCube.Tests.Core;

using System.Net;
using HyperCube.Core.Extensions;
using HyperCube.Core.Utils;
using NUnit.Framework;

[TestFixture]
public class StringMethodExtensionTests
{
    [Test]
    public void ToSnakeCase_Extension_ReturnsSameResultAsUtilMethod()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var extensionResult = input.ToSnakeCase();
        var utilResult = StringUtils.ToSnakeCase(input);

        // Assert
        Assert.That(extensionResult, Is.EqualTo(utilResult));
    }

    [Test]
    public void ToSnakeCaseUpper_Extension_ReturnsSameResultAsUpperSnakeCase()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var extensionResult = input.ToSnakeCaseUpper();
        var utilResult = StringUtils.ToUpperSnakeCase(input);

        // Assert
        Assert.That(extensionResult, Is.EqualTo(utilResult));
    }

    [Test]
    public void ToCamelCase_Extension_ReturnsSameResultAsUtilMethod()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var extensionResult = input.ToCamelCase();
        var utilResult = StringUtils.ToCamelCase(input);

        // Assert
        Assert.That(extensionResult, Is.EqualTo(utilResult));
    }

    [Test]
    public void ToPascalCase_Extension_ReturnsSameResultAsUtilMethod()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var extensionResult = input.ToPascalCase();
        var utilResult = StringUtils.ToPascalCase(input);

        // Assert
        Assert.That(extensionResult, Is.EqualTo(utilResult));
    }

    [Test]
    public void ToKebabCase_Extension_ReturnsSameResultAsUtilMethod()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var extensionResult = input.ToKebabCase();
        var utilResult = StringUtils.ToKebabCase(input);

        // Assert
        Assert.That(extensionResult, Is.EqualTo(utilResult));
    }

    [Test]
    public void ToTitleCase_Extension_ReturnsSameResultAsUtilMethod()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var extensionResult = input.ToTitleCase();
        var utilResult = StringUtils.ToTitleCase(input);

        // Assert
        Assert.That(extensionResult, Is.EqualTo(utilResult));
    }
}

[TestFixture]
public class JsonMethodExtensionTests
{
    [Test]
    public void ToJson_Extension_WithoutContext_WorksCorrectly()
    {
        // Arrange
        var obj = new TestClass { Name = "Test", Value = 42 };

        // Act
        var json = obj.ToJson();

        // Assert
        Assert.That(json, Is.Not.Null.Or.Empty);
        Assert.That(json, Does.Contain("name"));
        Assert.That(json, Does.Contain("value"));
        Assert.That(json, Does.Contain("Test"));
        Assert.That(json, Does.Contain("42"));
    }

    [Test]
    public void ToJson_Extension_WithContext_WorksCorrectly()
    {
        // Arrange
        var obj = new TestClass { Name = "Test", Value = 42 };

        // Act
        var json = obj.ToJson(TestJsonContext.Default);

        // Assert
        Assert.That(json, Is.Not.Null.Or.Empty);
        Assert.That(json, Does.Contain("name"));
        Assert.That(json, Does.Contain("value"));
        Assert.That(json, Does.Contain("Test"));
        Assert.That(json, Does.Contain("42"));
    }

    [Test]
    public void FromJson_Extension_GenericMethod_WorksCorrectly()
    {
        // Arrange
        var json = @"{""name"":""Test"",""value"":42}";

        // Act
        var obj = json.FromJson<TestClass>();

        // Assert
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.Name, Is.EqualTo("Test"));
        Assert.That(obj.Value, Is.EqualTo(42));
    }

    [Test]
    public void FromJson_Extension_WithTypeAndWithoutContext_WorksCorrectly()
    {
        // Arrange
        var json = @"{""name"":""Test"",""value"":42}";

        // Act
        var obj = json.FromJson(typeof(TestClass));

        // Assert
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj, Is.TypeOf<TestClass>());

        var typedObj = (TestClass)obj!;
        Assert.That(typedObj.Name, Is.EqualTo("Test"));
        Assert.That(typedObj.Value, Is.EqualTo(42));
    }

    [Test]
    public void FromJson_Extension_WithTypeAndContext_WorksCorrectly()
    {
        // Arrange
        var json = @"{""name"":""Test"",""value"":42}";

        // Act
        var obj = json.FromJson(typeof(TestClass), TestJsonContext.Default);

        // Assert
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj, Is.TypeOf<TestClass>());

        var typedObj = (TestClass)obj!;
        Assert.That(typedObj.Name, Is.EqualTo("Test"));
        Assert.That(typedObj.Value, Is.EqualTo(42));
    }
}

[TestFixture]
public class IpAddressExtensionTests
{
    [Test]
    public void ToIpAddress_WithAsterisk_ReturnsIPAddressAny()
    {
        // Arrange
        var input = "*";

        // Act
        var result = input.ToIpAddress();

        // Assert
        Assert.That(result, Is.EqualTo(IPAddress.Any));
    }

    [Test]
    public void ToIpAddress_WithDoubleColon_ReturnsIPv6Any()
    {
        // Arrange
        var input = "::";

        // Act
        var result = input.ToIpAddress();

        // Assert
        Assert.That(result, Is.EqualTo(IPAddress.IPv6Any));
    }

    [Test]
    public void ToIpAddress_WithAsteriskPattern_ReturnsIPAddressAny()
    {
        // Arrange
        var input = "*.*.*.*";

        // Act
        var result = input.ToIpAddress();

        // Assert
        Assert.That(result, Is.EqualTo(IPAddress.Any));
    }

    [Test]
    public void ToIpAddress_WithValidIPv4_ReturnsCorrectAddress()
    {
        // Arrange
        var input = "192.168.1.1";

        // Act
        var result = input.ToIpAddress();

        // Assert
        Assert.That(result.ToString(), Is.EqualTo(input));
    }

    [Test]
    public void ToIpAddress_WithValidIPv6_ReturnsCorrectAddress()
    {
        // Arrange
        var input = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";

        // Act
        var result = input.ToIpAddress();

        // Assert
        Assert.That(result.ToString(), Is.EqualTo("2001:db8:85a3::8a2e:370:7334"));
    }

    [Test]
    public void ToIpAddress_WithInvalidIPAddress_ThrowsFormatException()
    {
        // Arrange
        var input = "invalid-ip";

        // Assert
        Assert.Throws<FormatException>(() => input.ToIpAddress());
    }
}

[TestFixture]
public class EnvReplacerMethodExTests
{
    [Test]
    public void ReplaceEnvVariable_WithNoVariables_ReturnsSameString()
    {
        // Arrange
        var input = "Hello World";

        // Act
        var result = input.ReplaceEnvVariable();

        // Assert
        Assert.That(result, Is.EqualTo(input));
    }

    [Test]
    public void ReplaceEnvVariable_WithUnknownVariable_ReturnsSameString()
    {
        // Arrange
        var input = "Hello {UNKNOWN_VARIABLE}";

        // Act
        var result = input.ReplaceEnvVariable();

        // Assert
        Assert.That(result, Is.EqualTo(input));
    }

    [Test]
    public void ReplaceEnvVariable_WithValidVariable_ReturnsReplacedString()
    {
        // Arrange
        var variableName = "HYPERCUBE_TEST_VAR";
        var variableValue = "test_value";
        Environment.SetEnvironmentVariable(variableName, variableValue);
        var input = $"Hello {{HYPERCUBE_TEST_VAR}}";

        try
        {
            // Act
            var result = input.ReplaceEnvVariable();

            // Assert
            Assert.That(result, Is.EqualTo($"Hello {variableValue}"));
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable(variableName, null);
        }
    }

    [Test]
    public void ReplaceEnvVariable_WithMultipleVariables_ReplacesAllVariables()
    {
        // Arrange
        var var1Name = "HYPERCUBE_VAR1";
        var var1Value = "value1";
        var var2Name = "HYPERCUBE_VAR2";
        var var2Value = "value2";

        Environment.SetEnvironmentVariable(var1Name, var1Value);
        Environment.SetEnvironmentVariable(var2Name, var2Value);

        var input = $"First: {{HYPERCUBE_VAR1}}, Second: {{HYPERCUBE_VAR2}}";

        try
        {
            // Act
            var result = input.ReplaceEnvVariable();

            // Assert
            Assert.That(result, Is.EqualTo($"First: {var1Value}, Second: {var2Value}"));
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable(var1Name, null);
            Environment.SetEnvironmentVariable(var2Name, null);
        }
    }

    [Test]
    public void ReplaceEnvVariable_WithInvalidFormat_DoesNotReplace()
    {
        // Arrange
        var input = "Hello {INCOMPLETE";

        // Act
        var result = input.ReplaceEnvVariable();

        // Assert
        Assert.That(result, Is.EqualTo(input));
    }
}
