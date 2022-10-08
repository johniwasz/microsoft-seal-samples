using System;
using System.Threading.Tasks;

namespace FitnessTrackerClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using FitnessCryptoManager consoleManager = new FitnessCryptoManager();

            await consoleManager.Initialize();

            bool isExiting = false;
            while (!isExiting)
            {
                PrintMenu();
                var option = Convert.ToInt32(Console.ReadLine());

                switch (option)
                {
                    case 0:
                        Console.WriteLine("Exiting");
                        isExiting = true;
                        break;
                    case 1:
                        await consoleManager.SendNewRun();
                      break;
                    case 2:
                        DecryptedMetricsResponse decryptedResponse = await consoleManager.GetMetrics();
                        PrintMetrics(decryptedResponse);
                        break;
                }
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("********* Menu (enter the option number and press enter) *********");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Add running distance");
            Console.WriteLine("2. Get metrics");
            Console.Write("Option: ");
        }

        internal static void PrintMetrics(DecryptedMetricsResponse metricsResponse)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("********* Metrics *********");
            Console.WriteLine($"Total runs: {int.Parse(metricsResponse.TotalRuns, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine($"Total distance: {int.Parse(metricsResponse.TotalDistance, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine($"Total hours: {int.Parse(metricsResponse.TotalHours, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine(string.Empty);
        }

    }
}
