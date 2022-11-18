using FitnessTracker.Common.Models;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public interface IFitnessTrackerApiClient
    {
        Task AddNewRunningDistanceBGVAsync(RunItemBGV metricsRequest);

        Task AddNewRunningDistanceCKKSAsync(RunItemCKKS metricsRequest);

        Task<SummaryItemBGV> GetMetricsBGVAsync();

        Task<SummaryItemCKKS> GetMetricsCKKSAsync();

        Task SendPublicKeyBGVAsync(PublicKeyModelBGV publicKey);

        Task SendPublicKeyCKKSAsync(PublicKeyModelCKKS publicKey);
    }
}