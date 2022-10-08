using FitnessTrackerClient.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessTrackerClient
{
    internal class ClientWorker : BackgroundService
    {
        private CancellationTokenSource _stoppingCts;
        private readonly IFitnessCryptoManager _cryptoManager;
        private readonly ILogger<ClientWorker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private Task _executeTask;

        public ClientWorker(
            IFitnessCryptoManager cryptoManager,
            IHostApplicationLifetime hostApplicationLifetime,
            ILogger<ClientWorker> logger)
        {
            _cryptoManager = cryptoManager;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;

        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create linked token to allow cancelling executing task from provided token
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                // Configure encryption/decruption
                await _cryptoManager.Initialize();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing encryption");
                _hostApplicationLifetime.StopApplication();
                return;
            }

            // Store the task we're executing
            await ExecuteAsync(_stoppingCts.Token);

            // Otherwise it's running
            return;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool isExiting = false;
            try
            {
                while (!stoppingToken.IsCancellationRequested && !isExiting)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    PrintMenu();
                    var option = Convert.ToInt32(Console.ReadLine());

                    switch (option)
                    {
                        case 0:
                            Console.WriteLine("Exiting");
                            isExiting = true;
                            break;
                        case 1:
                            RunEntry newRun = GetNewRun();
                            if (newRun != null)
                            {
                                await _cryptoManager.SendNewRun(newRun);
                            }
                            break;
                        case 2:
                            Console.WriteLine("GetMetrics");
                            DecryptedMetricsResponse decryptedResponse = await _cryptoManager.GetMetrics();
                            PrintMetrics(decryptedResponse);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
            }

            _hostApplicationLifetime.StopApplication();
        }

        private RunEntry GetNewRun()
        {
            RunEntry runEntry = new RunEntry();

            // Get distance from user
            Console.Write("Enter the new running distance (km): ");
            var newRunningDistance = Convert.ToInt32(Console.ReadLine());

            if (newRunningDistance < 0)
            {
                Console.WriteLine("Running distance must be greater than 0.");
                return null;
            }

            runEntry.Distance = newRunningDistance;

            // Get time from user
            Console.Write("Enter the new running time (hours): ");
            var newRunningTime = Convert.ToInt32(Console.ReadLine());

            if (newRunningTime < 0)
            {
                Console.WriteLine("Running time must be greater than 0.");
                return null;
            }
            
            runEntry.Time = newRunningTime;

            return runEntry;
        }

        private void PrintMetrics(DecryptedMetricsResponse metricsResponse)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("********* Metrics *********");
            Console.WriteLine($"Total runs: {int.Parse(metricsResponse.TotalRuns, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine($"Total distance: {int.Parse(metricsResponse.TotalDistance, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine($"Total hours: {int.Parse(metricsResponse.TotalHours, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine(string.Empty);
        }

        private void PrintMenu()
        {
            Console.WriteLine("********* Menu (enter the option number and press enter) *********");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Add running distance");
            Console.WriteLine("2. Get metrics");
            Console.Write("Option: ");
        }
    }
}
