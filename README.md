Microsoft SEAL Samples
======================

This repo includes a number of demo projects that make use of [Microsoft SEAL](https://www.microsoft.com/en-us/research/project/microsoft-seal/), a homomorphic encryption library. They are based on the [sample code](https://zarmada.blob.core.windows.net/ai-school-module-updates/ai-school-lab-seal.zip) referenced in the [Homomorphic encryption with SEAL](https://learn.microsoft.com/en-us/azure/architecture/solution-ideas/articles/homomorphic-encryption-seal) article. All of the demo projects are based on this sample. It's recommended that you download the sample from the arcticle and work through that before exploring the demo projects here.

Each demo project builds on the work in the prior project.

# fitness-tracker-demo-01
# fitness-tracker-demo-02

The original [sample code](https://zarmada.blob.core.windows.net/ai-school-module-updates/ai-school-lab-seal.zip) uses .NET Core 2.2 and directly references the Microsoft SEAL C++ dll. The following updates are applied to this demo project:

- Upgraded to .NET 7.0.100
- Replaced Newtonsoft.Json with System.Text.Json
- Applied IDisposable pattern to clean up unmanaged resources (i.e. SEALContext, etc.)
- Moved api cryptography logic to a dedicated singleton service instead of using a singleton controller
- Directly referencing the [Microsoft.Research.SEALNet 4.0.0](https://www.nuget.org/packages/Microsoft.Research.SEALNet/) Nuget package. This is a C# wrapper around the C++ dll generated from the [Microsoft SEAL repo](https://github.com/microsoft/SEAL).
- Client creates secret and public key. Public key is passed to the server. The original sample created the public key and secret on the API. The client obtained the keys by calling a `metrics\keys` route. This defeats the intent of homomorphic encryption. The fitness client keeps personal health data that should be available as `plaintext` to the API. This update makes the client responsible for generating the public key and secret.
- Added FitnessTrackerPerf project which uses Benchmark.NET
- Added a CKKS controller and logic to calculate average speed



