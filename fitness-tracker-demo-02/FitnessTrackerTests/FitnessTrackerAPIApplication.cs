using FitnessTrackerClient.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FitnessTrackerTests.Logging;
using Xunit.Abstractions;

namespace FitnessTrackerTests
{
    internal class FitnessTrackerAPIApplication : WebApplicationFactory<FitnessTrackerAPI.Program>
    {
        private readonly string _environment;
        private readonly ulong _polyModulusDegree;

        private readonly ITestOutputHelper _testOutputHelper;

        public FitnessTrackerAPIApplication(ITestOutputHelper testOutputHelper, string environment = "Development", ulong polyModulusDegree = 4096)
        {
            _testOutputHelper = testOutputHelper;
            _environment = environment;
            _polyModulusDegree = polyModulusDegree;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(_environment);

            builder.ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.Services.AddSingleton<ILoggerProvider>(serviceProvider => new XUnitLoggerProvider(_testOutputHelper));
            });

            // Add mock/test services to the builder here
            builder.ConfigureServices(services =>
            {             
                services.Configure<FitnessCryptoConfig>(options =>
                {
                    options.PolyModulusDegree = _polyModulusDegree;
                });
            });

            return base.CreateHost(builder);
        }

        
    }
}
