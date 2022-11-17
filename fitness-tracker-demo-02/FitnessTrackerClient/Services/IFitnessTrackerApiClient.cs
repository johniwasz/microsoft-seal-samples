using FitnessTracker.Common.Models;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public interface IFitnessTrackerApiClient
    {
        Task AddNewRunningDistanceBGVAsync(RunItem metricsRequest);

        Task AddNewRunningDistanceCKKSAsync(RunItem metricsRequest);

        Task<SummaryItem> GetMetricsBGVAsync();

        Task<SummaryItem> GetMetricsCKKSAsync();

        Task SendPublicKeyBGVAsync(PublicKeyModel publicKey);

        Task SendPublicKeyCKKSAsync(PublicKeyModel publicKey);
    }
}