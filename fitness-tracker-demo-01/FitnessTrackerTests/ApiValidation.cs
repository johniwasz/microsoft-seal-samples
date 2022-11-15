using Xunit.Abstractions;
using FitnessTrackerClient.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FitnessTrackerClient.Models;
using Moq;
using FitnessTrackerTests.Logging;

namespace FitnessTrackerTests
{
    public class ApiValidation
    {

        private readonly ITestOutputHelper _output;

        public ApiValidation(ITestOutputHelper output)
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
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(8192)]
        [InlineData(16384)]
        [InlineData(32768)]
        public async Task EnsurePublicKeyIsSynchronized(ulong polyModulusDegree)
        {        
            using var application = new FitnessTrackerAPIApplication(testOutputHelper: _output, polyModulusDegree: polyModulusDegree);

            using HttpClient appClient = application.CreateClient();

            var clientLoggerFactory = LoggerUtilties.GetXUnitLoggerFactory(_output);

            ILogger<FitnessCryptoManager> clientLogger = clientLoggerFactory.CreateLogger<FitnessCryptoManager>();

            IOptions<FitnessCryptoConfig> cryptoConfig = GetCryptoConfig(polyModulusDegree);

            IFitnessTrackerApiClient apiClient = new FitnessTrackerApiClient(appClient);

            using IFitnessCryptoManager cruptoManagerSut = new FitnessCryptoManager(cryptoConfig, apiClient, clientLogger);

            await cruptoManagerSut.InitializeAsync();

            var key = ((FitnessCryptoManager)cruptoManagerSut).PublicKey;

            long keySize = key.SaveSize();

            _output.WriteLine($"PolyModulus Degree: {polyModulusDegree}, Key Size: {keySize}");

            RunEntry newRun = new RunEntry(1, 1);

            await cruptoManagerSut.SendNewRunAsync(newRun);

        }
    }
}