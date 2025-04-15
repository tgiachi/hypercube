# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="0.0.11"></a>
## [0.0.11](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.11) (2025-04-15)

### Bug Fixes

* **ResourceUtils.cs:** update resourcePath variable to correctly match resource name ([b228434](https://www.github.com/tgiachi/hypercube/commit/b228434b46b9839f8c8f9ef00f98f478c257066a))

<a name="0.0.10"></a>
## [0.0.10](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.10) (2025-04-15)

<a name="0.0.9"></a>
## [0.0.9](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.9) (2025-04-13)

### Features

* **EventDispatcherService:** add EventDispatcherService class to handle event subscription and dispatching ([f89818a](https://www.github.com/tgiachi/hypercube/commit/f89818a4f05d013bd7ce302bfbd0b1de5b35933a))
* **HyperCube.ScriptEngine.Core.csproj:** update package version to 0.0.8 ([be0875c](https://www.github.com/tgiachi/hypercube/commit/be0875c434751ab51c335dd167b857bfae463723))
* **HyperCube.Server.Core:** add new classes and interfaces for managing server core functionalities ([b518941](https://www.github.com/tgiachi/hypercube/commit/b5189414b1c157fb6233a55cc10b04dbec94c8c8))
* **RegisterScriptModuleExtension.cs:** add RegisterScriptModuleExtension class to provide methods for registering script modules in the service collection ([3a7c273](https://www.github.com/tgiachi/hypercube/commit/3a7c2734e456b46589df7446495cfa45b2ee810f))
* **sln:** add new project HyperCube.ScriptEngine.Core to the solution ([dfb7fd6](https://www.github.com/tgiachi/hypercube/commit/dfb7fd64b4f8946fbb19eeaa55a4ab2f8bb1723e))

<a name="0.0.8"></a>
## [0.0.8](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.8) (2025-04-13)

### Features

* **Hypercube.sln:** add HyperCube.Queue.Core project to solution ([0eb7260](https://www.github.com/tgiachi/hypercube/commit/0eb7260b48874aecb30b61d084d3388a1764b6c9))
* **queue:** add interfaces for queue listeners, publishers, and providers ([d07454a](https://www.github.com/tgiachi/hypercube/commit/d07454adb58cc8799dc3fdbc1961d67d8c21492b))
* **QueueService.cs:** add QueueService class with methods to create publishers, listeners, and publish messages ([160099c](https://www.github.com/tgiachi/hypercube/commit/160099c2b141cfaad6786f33eda8ba8d9b14f23e))

<a name="0.0.7"></a>
## [0.0.7](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.7) (2025-04-13)

### Features

* **Hypercube.sln:** add HyperCube.Entities.Core project to solution ([4c9b0cc](https://www.github.com/tgiachi/hypercube/commit/4c9b0cc3eaf193bb1cbdd87d79d70fe829555f93))

### Bug Fixes

* **entities:** update version number in csproj file from 0.0.1 to 0.0.6 ([c13c3ef](https://www.github.com/tgiachi/hypercube/commit/c13c3efbadcc86359937d3bca3381086afcc24d1))

<a name="0.0.6"></a>
## [0.0.6](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.6) (2025-04-12)

<a name="0.0.6"></a>
## [0.0.6](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.6) (2025-04-12)

### Features

* **HyperCubeHostBuilder.cs:** add new class HyperCubeHostBuilder for creating and configuring HyperCube host applications ([ac52644](https://www.github.com/tgiachi/hypercube/commit/ac526447fe584c158dcf652ce4e2494defbd282e))
* **HyperCubeHostBuilder.cs:** add support for HyperPostmanConfig to customize postman settings ([7350b0f](https://www.github.com/tgiachi/hypercube/commit/7350b0f1e1750ca6a37c42f8702e63b2a094cbb1))

<a name="0.0.5"></a>
## [0.0.5](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.5) (2025-04-11)

### Features

* **AddHyperCubeServiceExtension.cs:** add extension method to register HyperCube services with specified lifetime type ([aa3739a](https://www.github.com/tgiachi/hypercube/commit/aa3739a767c755a069876af55f05b975bab2cc34))
* **events:** add BasePostmanRecordEvent, ServerStartedEvent, and ServerStoppingEvent classes to handle postman events in the server core module ([1cba42e](https://www.github.com/tgiachi/hypercube/commit/1cba42e4e7eab062e12baefb069e974d8e59d35f))
* **ResourceUtils.cs:** add method to handle reading embedded resources with special characters in resource names ([01419cf](https://www.github.com/tgiachi/hypercube/commit/01419cf7dc5345368d62a20fc9d4c4bc49dee337))

<a name="0.0.4"></a>
## [0.0.4](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.4) (2025-04-11)

### Features

* **HyperCube.Postman.csproj:** update version from 0.0.1 to 0.0.2 for release ([00568b9](https://www.github.com/tgiachi/hypercube/commit/00568b99e2efe06bcf95887a15d7449f90181a24))
* **Hypercube.sln:** add HyperCube.Postman project to the solution ([e1d9943](https://www.github.com/tgiachi/hypercube/commit/e1d99437851b6d85ad43276b89b43757e5d7cf1f))
* **HyperPostmanConfig.cs:** remove unused properties ContinueOnError, TimeoutMilliseconds, ([69bbc08](https://www.github.com/tgiachi/hypercube/commit/69bbc088075ddbf86eebfb2f756c8d40026e3416))
* **RegisterPostmanServiceExtension.cs:** rename RegisterPostmanService method to AddPostman for better clarity and consistency ([51ff069](https://www.github.com/tgiachi/hypercube/commit/51ff069d10dd1fbd563e18b5fdba3aaa753f11e7))

<a name="0.0.2"></a>
## [0.0.2](https://www.github.com/tgiachi/hypercube/releases/tag/v0.0.2) (2025-04-11)

### Features

* **dependabot.yml:** add Dependabot configuration file for NuGet, GitHub Actions, and Docker to automate dependency updates and pull requests ([85c64fb](https://www.github.com/tgiachi/hypercube/commit/85c64fb432c948b9b60741f7aacb4ba061e1560a))
* **HyperCube.Core.csproj:** add support for generating documentation file, including symbols, and publishing repository URL ([bf4e104](https://www.github.com/tgiachi/hypercube/commit/bf4e104d61b7592abed5fc149d74751590e25da0))
* **sln:** add HyperCube.Tests project to solution ([0b6894e](https://www.github.com/tgiachi/hypercube/commit/0b6894e5c086210e272c18f6716596bd4d3d3366))

