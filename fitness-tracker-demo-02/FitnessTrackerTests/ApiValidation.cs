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
        public async Task EnsurePublicKeyIsSynchronizedBGV(ulong polyModulusDegree)
        {        
            using var application = new FitnessTrackerAPIApplication(testOutputHelper: _output, polyModulusDegree: polyModulusDegree);

            using HttpClient appClient = application.CreateClient();

            var clientLoggerFactory = LoggerUtilties.GetXUnitLoggerFactory(_output);

            ILogger<FitnessCryptoManagerBGV> clientLogger = clientLoggerFactory.CreateLogger<FitnessCryptoManagerBGV>();

            IOptions<FitnessCryptoConfig> cryptoConfig = GetCryptoConfig(polyModulusDegree);

            IFitnessTrackerApiClient apiClient = new FitnessTrackerApiClient(appClient);

            using IFitnessCryptoManagerBGV cruptoManagerSut = new FitnessCryptoManagerBGV(cryptoConfig, apiClient, clientLogger);

            await cruptoManagerSut.InitializeAsync();

            var key = cruptoManagerSut.PublicKey;

            long keySize = key.SaveSize();

            _output.WriteLine($"PolyModulus Degree: {polyModulusDegree}, Key Size: {keySize}");

            RunEntry<int> newRun = new RunEntry<int>(1, 1);

            await cruptoManagerSut.SendNewRunAsync(newRun);


            var metricsResult = await cruptoManagerSut.GetMetricsAsync();

        }



        [Theory]
        //[InlineData(1024)]
       // [InlineData(4096)]
        [InlineData(8192)]
       // [InlineData(16384)]
        //[InlineData(32768)]
        public async Task EnsurePublicKeyIsSynchronizedCKKS(ulong polyModulusDegree)
        {
            using var application = new FitnessTrackerAPIApplication(testOutputHelper: _output, polyModulusDegree: polyModulusDegree);

            using HttpClient appClient = application.CreateClient();

            var clientLoggerFactory = LoggerUtilties.GetXUnitLoggerFactory(_output);

            ILogger<FitnessCryptoManagerCKKS> clientLogger = clientLoggerFactory.CreateLogger<FitnessCryptoManagerCKKS>();

            IOptions<FitnessCryptoConfig> cryptoConfig = GetCryptoConfig(polyModulusDegree);

            IFitnessTrackerApiClient apiClient = new FitnessTrackerApiClient(appClient);

            using IFitnessCryptoManagerCKKS cruptoManagerSut = new FitnessCryptoManagerCKKS(cryptoConfig, apiClient, clientLogger);

            await cruptoManagerSut.InitializeAsync();

            var key = cruptoManagerSut.PublicKey;

            long keySize = key.SaveSize();

            _output.WriteLine($"PolyModulus Degree: {polyModulusDegree}, Key Size: {keySize}");

            // 10 minutes
            int totalSeconds = 10 * 60;

            RunEntry<double> newRun = new RunEntry<double>(1, totalSeconds);

            await cruptoManagerSut.SendNewRunAsync(newRun);

            newRun = new RunEntry<double>(1, totalSeconds + 60);

            await cruptoManagerSut.SendNewRunAsync(newRun);

            DecryptedMetricsAverageResponse metricsResponse = await cruptoManagerSut.GetMetricsAverageAsync();

           
            _output.WriteLine($"TotalRuns: {(int)metricsResponse.TotalRuns}");

            _output.WriteLine($"TotalDistance: {(int)metricsResponse.TotalDistance}");

            _output.WriteLine($"TotalTime: {(int)metricsResponse.TotalSeconds}");

            _output.WriteLine($"AverageSpeed: {(int)metricsResponse.AverageSpeed}");

            Assert.Equal(2, (int)metricsResponse.TotalRuns);

            //var metricsResult = await cruptoManagerSut.GetMetricsAsync();

        }

    }
}