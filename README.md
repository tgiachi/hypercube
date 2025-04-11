# HyperCube Framework

![](images/hypercube_cover.png)

## Overview

HyperCube is a high-performance, modular framework designed to accelerate development of:

- Console applications
- Server applications
- Web applications (backend)

Built with a focus on modularity, documentation, and developer experience, HyperCube provides the building blocks for
creating robust applications quickly and efficiently.

## Features

- **Modular Architecture**: Build your application using composable modules that can be easily reused across projects
- **Comprehensive Utilities**: Ready-to-use utility classes for common operations:
    - String manipulation (case conversions, environment variable handling)
    - Cryptography (hashing, encryption, password management)
    - JSON & YAML serialization with AOT support
    - IP address handling
- **Performance Optimized**: Built with AOT (Ahead-of-Time) compilation support
- **Well Documented**: Thorough XML documentation for all components
- **Developer Friendly**: Intuitive API design with consistent patterns

## Installation

```bash
dotnet add package HyperCube.Core
```

## Core Utilities

### String Manipulation

```csharp
using HyperCube.Core.Utils;
using HyperCube.Core.Extensions;

// Case conversions
var snakeCase = "HelloWorld".ToSnakeCase();  // "hello_world"
var kebabCase = "HelloWorld".ToKebabCase();  // "hello-world"
var camelCase = "Hello_World".ToCamelCase(); // "helloWorld"

// Environment variable replacement
var path = "{HOME}/documents".ReplaceEnvVariable();  // "/home/user/documents"
```

### Cryptography

```csharp
using HyperCube.Core.Utils;

// Simple hashing
var hash = HashUtils.ComputeSha256Hash("sensitive data");

// Password management
var passwordHash = HashUtils.CreatePassword("secure_password");  // Returns "Hash:Salt"
var isValid = HashUtils.VerifyPassword("secure_password", passwordHash);  // true

// Encryption/Decryption
var key = Convert.FromBase64String(HashUtils.GenerateBase64Key());
var encrypted = HashUtils.Encrypt("secret message", key);
var decrypted = HashUtils.Decrypt(encrypted, key);  // "secret message"
```

### JSON Handling with AOT Support

```csharp
using HyperCube.Core.Utils;
using System.Text.Json.Serialization;

// Define your context for AOT support
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    WriteIndented = true)]
[JsonSerializable(typeof(MyClass))]
public partial class MyJsonContext : JsonSerializerContext {}

// Serialize/deserialize with AOT support
var json = JsonUtils.Serialize(myObject, MyJsonContext.Default);
var obj = JsonUtils.Deserialize<MyClass>(json, MyJsonContext.Default);

// Or use extension methods
var json = myObject.ToJson(MyJsonContext.Default);
var obj = json.FromJson<MyClass>(MyJsonContext.Default);
```

### IP Address Handling

## License

HyperCube is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.