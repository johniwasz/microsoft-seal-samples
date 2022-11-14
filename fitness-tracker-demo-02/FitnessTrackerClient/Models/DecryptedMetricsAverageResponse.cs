using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Models
{
    public class DecryptedMetricsAverageResponse
    {
        public double TotalRuns { get; set; }
        public double TotalDistance { get; set; }
        public double TotalSeconds { get; set; }

        public double AverageSpeed { get; set; }

    }
}
