using HyperCube.Core.Utils;

namespace HyperCube.Tests.Core;

[TestFixture]
public class HashUtilsTests
{
    [Test]
    public void ComputeSha256Hash_WithValidInput_ReturnsExpectedHash()
    {
        // Arrange
        var input = "test";
        var expectedHex = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

        // Act
        var result = HashUtils.ComputeSha256Hash(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedHex));
    }

    [Test]
    public void ComputeSha256Hash_WithEmptyString_ReturnsExpectedHash()
    {
        // Arrange
        var input = string.Empty;
        var expectedHex = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        // Act
        var result = HashUtils.ComputeSha256Hash(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedHex));
    }

    [Test]
    public void HashPassword_ReturnsDifferentHashForSamePassword()
    {
        // Arrange
        var password = "password123";

        // Act
        var (hash1, salt1) = HashUtils.HashPassword(password);
        var (hash2, salt2) = HashUtils.HashPassword(password);

        // Assert
        Assert.That(hash1, Is.Not.EqualTo(hash2));
        Assert.That(salt1, Is.Not.EqualTo(salt2));
    }

    [Test]
    public void CheckPasswordHash_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "password123";
        var (hash, salt) = HashUtils.HashPassword(password);

        // Act
        var result = HashUtils.CheckPasswordHash(password, hash, salt);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckPasswordHash_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "password123";
        var wrongPassword = "password124";
        var (hash, salt) = HashUtils.HashPassword(password);

        // Act
        var result = HashUtils.CheckPasswordHash(wrongPassword, hash, salt);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CreatePassword_ReturnsValidFormatWithHashAndSalt()
    {
        // Arrange
        var password = "password123";

        // Act
        var result = HashUtils.CreatePassword(password);

        // Assert
        Assert.That(result, Does.Contain(":"));
        var parts = result.Split(':');
        Assert.That(parts.Length, Is.EqualTo(2));
        Assert.That(parts[0], Is.Not.Empty);
        Assert.That(parts[1], Is.Not.Empty);
    }

    [Test]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "password123";
        var hashSalt = HashUtils.CreatePassword(password);

        // Act
        var result = HashUtils.VerifyPassword(password, hashSalt);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "password123";
        var wrongPassword = "password124";
        var hashSalt = HashUtils.CreatePassword(password);

        // Act
        var result = HashUtils.VerifyPassword(wrongPassword, hashSalt);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void VerifyPassword_WithInvalidFormat_ThrowsFormatException()
    {
        // Arrange
        var password = "password123";
        var invalidHashSalt = "invalidformat";

        // Assert
        Assert.Throws<FormatException>(() => HashUtils.VerifyPassword(password, invalidHashSalt));
    }

    [Test]
    public void GenerateRandomRefreshToken_ReturnsValidBase64String()
    {
        // Act
        var token = HashUtils.GenerateRandomRefreshToken();

        // Assert
        Assert.That(token, Is.Not.Null.Or.Empty);
        Assert.DoesNotThrow(() => Convert.FromBase64String(token));
    }

    [Test]
    public void GenerateRandomRefreshToken_WithCustomSize_ReturnsExpectedLength()
    {
        // Arrange
        int size = 64;

        // Act
        var token = HashUtils.GenerateRandomRefreshToken(size);
        var decodedBytes = Convert.FromBase64String(token);

        // Assert
        Assert.That(decodedBytes.Length, Is.EqualTo(size));
    }

    [Test]
    public void GenerateBase64Key_ReturnsValidBase64String()
    {
        // Act
        var key = HashUtils.GenerateBase64Key();

        // Assert
        Assert.That(key, Is.Not.Null.Or.Empty);
        Assert.DoesNotThrow(() => Convert.FromBase64String(key));
    }

    [Test]
    public void GenerateBase64Key_WithCustomSize_ReturnsExpectedLength()
    {
        // Arrange
        int byteLength = 64;

        // Act
        var key = HashUtils.GenerateBase64Key(byteLength);
        var decodedBytes = Convert.FromBase64String(key);

        // Assert
        Assert.That(decodedBytes.Length, Is.EqualTo(byteLength));
    }

    [Test]
    public void Encrypt_Decrypt_RoundTrip_ReturnsOriginalText()
    {
        // Arrange
        var plaintext = "This is a secret message";
        var key = new byte[32]; // 256-bit key filled with zeros for test

        // Act
        var encrypted = HashUtils.Encrypt(plaintext, key);
        var decrypted = HashUtils.Decrypt(encrypted, key);

        // Assert
        Assert.That(decrypted, Is.EqualTo(plaintext));
    }

    [Test]
    public void Encrypt_WithSameInputAndKey_ProducesDifferentOutput()
    {
        // Arrange
        var plaintext = "This is a secret message";
        var key = new byte[32]; // 256-bit key filled with zeros for test

        // Act
        var encrypted1 = HashUtils.Encrypt(plaintext, key);
        var encrypted2 = HashUtils.Encrypt(plaintext, key);

        // Assert
        Assert.That(encrypted1, Is.Not.EqualTo(encrypted2));
    }

    [Test]
    public void Encrypt_OutputContainsIV()
    {
        // Arrange
        var plaintext = "This is a secret message";
        var key = new byte[32]; // 256-bit key filled with zeros for test

        // Act
        var encrypted = HashUtils.Encrypt(plaintext, key);

        // Assert
        Assert.That(encrypted.Length, Is.GreaterThan(16)); // At least IV (16 bytes) + some ciphertext
    }
}
