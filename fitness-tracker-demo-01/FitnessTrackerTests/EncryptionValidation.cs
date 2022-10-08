using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using Xunit.Abstractions;
using Xunit.DependencyInjection;
using FitnessTrackerClient.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FitnessTrackerClient.Models;
using FitnessTracker.Common.Utils;
using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using FitnessTrackerAPI.Services;

namespace FitnessTrackerTests
{
    public class EncryptionValidation
    {

        private readonly ITestOutputHelper _output;

        public EncryptionValidation(ITestOutputHelper output)
        {
            _output = output;
        }

        internal IOptions<FitnessCryptoConfig> GetCryptoConfig(ulong polyModulusDegree)
        {
            FitnessCryptoConfig fitConfig = new FitnessCryptoConfig();
            fitConfig.PolyModulusDegree = polyModulusDegree;  
            IOptions<FitnessCryptoConfig> fitConfigOpts = Options.Create<FitnessCryptoConfig>(fitConfig);
            return fitConfigOpts;
        }

         
        [Theory]
       // [InlineData(1024)]
        [InlineData(4096)]
       // [InlineData(8196)]
       // [InlineData(16384)]
       // [InlineData(32768)]
        public async Task EnsurePublicKeyIsSynchronized(ulong polyModulusDegree)
        {

        
            using var application = new FitnessTrackerAPIApplication();

            using HttpClient appClient = application.CreateClient();


            ILogger<FitnessCryptoManager> logger = LoggerUtilties._loggerFactory.CreateLogger<FitnessCryptoManager>();
            IOptions<FitnessCryptoConfig> cryptoConfig = GetCryptoConfig(polyModulusDegree);

            // using HttpClient client = new HttpClient();

            IFitnessTrackerApiClient apiClient = new FitnessTrackerApiClient(appClient);

            using IFitnessCryptoManager cryptoManager = new FitnessCryptoManager(cryptoConfig, apiClient, logger);

            await cryptoManager.InitializeAsync();

            var key = ((FitnessCryptoManager) cryptoManager).PublicKey;

            long keySize = key.SaveSize();

            _output.WriteLine($"PolyModulus Degree: {polyModulusDegree}, Key Size: {keySize}");

        }
    }
}