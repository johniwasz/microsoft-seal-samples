using FitnessTracker.Common;
using FitnessTracker.Common.Models;
using FitnessTrackerClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public interface IFitnessCryptoManagerCKKS : IFitnessCryptoManager
    {
        public Task<DecryptedMetricsAverageResponse> GetMetricsAverageAsync();

        public Task SendNewRunAsync(RunEntry<double> runItem);

    }
}
