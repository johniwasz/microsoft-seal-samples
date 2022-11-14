using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Net.Http.Headers;
using Polly.Extensions.Http;
using Polly;
using System.Net.Http;
using FitnessTrackerClient.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Services;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services
            .Configure<FitnessCryptoConfig>(options =>
            {
                options.PolyModulusDegree = SEALUtils.DEFAULTPOLYMODULUSDEGREE;
            })
            .AddSingleton<IFitnessCryptoManagerBGV, FitnessCryptoManagerBGV>()
            .AddSingleton<IFitnessCryptoManagerCKKS, FitnessCryptoManagerCKKS>()
            .AddHostedService<ClientWorker>()
            .AddScoped<IFitnessTrackerApiClient, FitnessTrackerApiClient>()
            .AddHttpClient<IFitnessTrackerApiClient, FitnessTrackerApiClient>()
            .ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://localhost:58849/api/");
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/json");
            })
            .AddPolicyHandler(GetRetryPolicy()))
    .Build();

await host.RunAsync();


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                    retryAttempt)));
}
