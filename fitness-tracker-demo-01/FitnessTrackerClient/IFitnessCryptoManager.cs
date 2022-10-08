using System.Threading.Tasks;
using FitnessTrackerClient.Models;

namespace FitnessTrackerClient
{
    internal interface IFitnessCryptoManager
    {
        Task<DecryptedMetricsResponse> GetMetrics();
        Task Initialize();
        Task SendNewRun(RunEntry newRun);
    }
}