using FitnessTracker.Common.Models;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public interface IFitnessTrackerApiClient
    {
        Task AddNewRunningDistance(RunItem metricsRequest);
        Task<SummaryItem> GetMetrics();
        Task SendPublicKey(PublicKeyModel publicKey);
    }
}