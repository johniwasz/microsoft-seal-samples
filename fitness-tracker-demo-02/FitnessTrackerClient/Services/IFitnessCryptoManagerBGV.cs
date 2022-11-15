using System;
using System.Threading.Tasks;
using FitnessTrackerClient.Models;

namespace FitnessTrackerClient.Services
{
    public interface IFitnessCryptoManagerBGV : IFitnessCryptoManager
    {
        public Task<DecryptedMetricsResponse> GetMetricsAsync();

        public Task SendNewRunAsync(RunEntry<int> newRun);
    }
}