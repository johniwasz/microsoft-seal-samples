using FitnessTrackerClient.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    internal class ClientWorker : BackgroundService
    {
        private CancellationTokenSource _stoppingCts;
        private readonly IFitnessCryptoManagerBGV _cryptoManagerBGV;
        private readonly IFitnessCryptoManagerCKKS _cryptoManagerCKKS;
        private readonly ILogger<ClientWorker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public ClientWorker(
            IFitnessCryptoManagerBGV cryptoManagerBGV,
            IFitnessCryptoManagerCKKS cryptoManagerCKKS,
            IHostApplicationLifetime hostApplicationLifetime,
            ILogger<ClientWorker> logger)
        {
            _cryptoManagerBGV = cryptoManagerBGV;
            _cryptoManagerCKKS = cryptoManagerCKKS;
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
                await _cryptoManagerBGV.InitializeAsync();
                await _cryptoManagerCKKS.InitializeAsync();
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
                            isExiting = true;
                            break;
                        case 1:
                            RunEntry<int> newRun = GetNewRun();
                            if (newRun != null)
                            {
                                await _cryptoManagerBGV.SendNewRunAsync(newRun);
                            }
                            break;
                        case 2:
                            DecryptedMetricsResponse decryptedResponse = await _cryptoManagerBGV.GetMetricsAsync();
                            PrintMetrics(decryptedResponse);
                            break;
                        case 3:
                            RunEntry<double> newRunCKKS = GetNewRunCKKS();
                            if (newRunCKKS != null)
                            {
                                await _cryptoManagerCKKS.SendNewRunAsync(newRunCKKS);
                            }
                            break;
                        case 4:
                            DecryptedMetricsAverageResponse decryptedAverageResponse = await _cryptoManagerCKKS.GetMetricsAverageAsync();
                            PrintMetrics(decryptedAverageResponse);
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

        private void PrintMetrics(DecryptedMetricsAverageResponse decryptedAverageResponse)
        {


            Console.WriteLine(string.Empty);
            Console.WriteLine("********* Metrics *********");
            Console.WriteLine($"Total runs: {(int) decryptedAverageResponse.TotalRuns}");
            Console.WriteLine($"Total distance: {decryptedAverageResponse.TotalDistance}");

            TimeSpan totalTimeSpan = TimeSpan.FromSeconds(decryptedAverageResponse.TotalSeconds);

            double milesPerHour = decryptedAverageResponse.AverageSpeed * 60 * 60;

            Console.WriteLine($"Total time: {totalTimeSpan}");

            Console.WriteLine($"Average pace: {milesPerHour} mph");

            Console.WriteLine(string.Empty);
        }

        private RunEntry<int> GetNewRun()
        {
            RunEntry<int> runEntry = new RunEntry<int>();

            // Get distance from user
            Console.Write("Enter the new running distance (m): ");
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

        private RunEntry<double> GetNewRunCKKS()
        {
            RunEntry<double> runEntry = new RunEntry<double>();

            // Get distance from user
            Console.Write("Enter the new running distance (m): ");
            var newRunningDistance = Convert.ToDouble(Console.ReadLine());

            if (newRunningDistance < 0)
            {
                Console.WriteLine("Running distance must be greater than 0.");
                return null;
            }

            runEntry.Distance = newRunningDistance;

            // Get time from user
            Console.Write("Enter the new running time (HH:MM:SS.mmm): ");

            string timeEntry = Console.ReadLine();

            double runningTime;

            if (TimeSpan.TryParse(timeEntry, out TimeSpan runTime))
            {
                runningTime = runTime.TotalSeconds;
                runEntry.Time = runningTime;
            }
            else
            {
                Console.WriteLine("Could not parse running time.");
                return null;
            }

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
            Console.WriteLine("Integers (BGV):");
            Console.WriteLine("  1. Add run (whole numbers)");
            Console.WriteLine("  2. Get metrics");
            Console.WriteLine("Doubles (CKKS):");
            Console.WriteLine("  3. Add run ");
            Console.WriteLine("  4. Get metrics");

            Console.Write("Option: ");
        }
    }
}
