using HyperCube.Core.Utils;

namespace HyperCube.Tests.Core;

[TestFixture]
public class StringUtilsTests
{
    [Test]
    public void ToSnakeCase_WithPascalCase_ReturnsSnakeCase()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var result = StringUtils.ToSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("hello_world"));
    }

    [Test]
    public void ToSnakeCase_WithCamelCase_ReturnsSnakeCase()
    {
        // Arrange
        var input = "helloWorld";

        // Act
        var result = StringUtils.ToSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("hello_world"));
    }

    [Test]
    public void ToSnakeCase_WithMixedCase_ReturnsSnakeCase()
    {
        // Arrange
        var input = "HTTPResponse";

        // Act
        var result = StringUtils.ToSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("http_response"));
    }

    [Test]
    public void ToSnakeCase_WithOneCharacter_ReturnsLowercaseCharacter()
    {
        // Arrange
        var input = "A";

        // Act
        var result = StringUtils.ToSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("a"));
    }

    [Test]
    public void ToSnakeCase_WithNullOrEmpty_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToSnakeCase(null!));
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToSnakeCase(string.Empty));
    }

    [Test]
    public void ToCamelCase_WithPascalCase_ReturnsCamelCase()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var result = StringUtils.ToCamelCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("helloWorld"));
    }

    [Test]
    public void ToCamelCase_WithSnakeCase_ReturnsCamelCase()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var result = StringUtils.ToCamelCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("helloWorld"));
    }

    [Test]
    public void ToCamelCase_WithKebabCase_ReturnsCamelCase()
    {
        // Arrange
        var input = "hello-world";

        // Act
        var result = StringUtils.ToCamelCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("helloWorld"));
    }

    [Test]
    public void ToCamelCase_WithSpaces_ReturnsCamelCase()
    {
        // Arrange
        var input = "hello world";

        // Act
        var result = StringUtils.ToCamelCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("helloWorld"));
    }

    [Test]
    public void ToCamelCase_WithNullOrEmpty_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToCamelCase(null!));
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToCamelCase(string.Empty));
    }

    [Test]
    public void ToPascalCase_WithCamelCase_ReturnsPascalCase()
    {
        // Arrange
        var input = "helloWorld";

        // Act
        var result = StringUtils.ToPascalCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void ToPascalCase_WithSnakeCase_ReturnsPascalCase()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var result = StringUtils.ToPascalCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void ToPascalCase_WithKebabCase_ReturnsPascalCase()
    {
        // Arrange
        var input = "hello-world";

        // Act
        var result = StringUtils.ToPascalCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void ToPascalCase_WithSpaces_ReturnsPascalCase()
    {
        // Arrange
        var input = "hello world";

        // Act
        var result = StringUtils.ToPascalCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void ToPascalCase_WithNullOrEmpty_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToPascalCase(null!));
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToPascalCase(string.Empty));
    }

    [Test]
    public void ToKebabCase_WithPascalCase_ReturnsKebabCase()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var result = StringUtils.ToKebabCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("hello-world"));
    }

    [Test]
    public void ToKebabCase_WithCamelCase_ReturnsKebabCase()
    {
        // Arrange
        var input = "helloWorld";

        // Act
        var result = StringUtils.ToKebabCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("hello-world"));
    }

    [Test]
    public void ToKebabCase_WithSnakeCase_ReturnsKebabCase()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var result = StringUtils.ToKebabCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("hello-world"));
    }

    [Test]
    public void ToKebabCase_WithNullOrEmpty_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToKebabCase(null!));
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToKebabCase(string.Empty));
    }

    [Test]
    public void ToUpperSnakeCase_WithPascalCase_ReturnsUpperSnakeCase()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var result = StringUtils.ToUpperSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("HELLO_WORLD"));
    }

    [Test]
    public void ToUpperSnakeCase_WithCamelCase_ReturnsUpperSnakeCase()
    {
        // Arrange
        var input = "helloWorld";

        // Act
        var result = StringUtils.ToUpperSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("HELLO_WORLD"));
    }

    [Test]
    public void ToUpperSnakeCase_WithKebabCase_ReturnsUpperSnakeCase()
    {
        // Arrange
        var input = "hello-world";

        // Act
        var result = StringUtils.ToUpperSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("HELLO_WORLD"));
    }

    [Test]
    public void ToTitleCase_WithSnakeCase_ReturnsTitleCase()
    {
        // Arrange
        var input = "hello_world";

        // Act
        var result = StringUtils.ToTitleCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("Hello World"));
    }

    [Test]
    public void ToTitleCase_WithKebabCase_ReturnsTitleCase()
    {
        // Arrange
        var input = "hello-world";

        // Act
        var result = StringUtils.ToTitleCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("Hello World"));
    }

    [Test]
    public void ToTitleCase_WithCamelCase_ReturnsTitleCase()
    {
        // Arrange
        var input = "helloWorld";

        // Act
        var result = StringUtils.ToTitleCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("Hello World"));
    }

    [Test]
    public void ToTitleCase_WithPascalCase_ReturnsTitleCase()
    {
        // Arrange
        var input = "HelloWorld";

        // Act
        var result = StringUtils.ToTitleCase(input);

        // Assert
        Assert.That(result, Is.EqualTo("Hello World"));
    }
}
