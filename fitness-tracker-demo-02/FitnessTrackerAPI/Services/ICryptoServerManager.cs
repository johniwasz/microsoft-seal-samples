using FitnessTracker.Common.Models;

namespace FitnessTrackerAPI.Services
{
    public interface ICryptoServerManager
    {
        void AddRunItem(RunItem request);
        SummaryItem GetMetrics();
        void SetPublicKey(string publicKeyEncoded);
    }
}