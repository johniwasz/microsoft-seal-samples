using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;


namespace FitnessTrackerTests
{
    internal class FitnessTrackerAPIApplication : WebApplicationFactory<FitnessTrackerAPI.Program>
    {
        private readonly string _environment;
        private readonly ulong _polyModulusDegree;

        public FitnessTrackerAPIApplication(string environment = "Development", ulong polyModulusDegree = 4096)
        {
            _environment = environment;
            _polyModulusDegree = polyModulusDegree;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(_environment);

            // Add mock/test services to the builder here
            //builder.ConfigureServices(services =>
            //{
                
            //});

            return base.CreateHost(builder);
        }

        
    }
}
