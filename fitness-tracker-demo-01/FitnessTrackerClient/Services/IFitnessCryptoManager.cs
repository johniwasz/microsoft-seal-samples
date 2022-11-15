using System;
using System.Threading.Tasks;
using FitnessTrackerClient.Models;

namespace FitnessTrackerClient.Services
{
    public interface IFitnessCryptoManager : IDisposable
    {
        public Task<DecryptedMetricsResponse> GetMetricsAsync();
        public Task InitializeAsync();
        public Task SendNewRunAsync(RunEntry newRun);
    }
}