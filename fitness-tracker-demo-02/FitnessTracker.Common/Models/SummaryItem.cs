using FitnessTracker.Common.Utils;
using System.Diagnostics;

namespace FitnessTracker.Common.Models
{
    public class SummaryItem
    {
        public string TotalRuns { get; set; }

        public string TotalDistance { get; set; }

        public string TotalTime { get; set; }

        public string AverageSpeed { get; set; }
    }
}
