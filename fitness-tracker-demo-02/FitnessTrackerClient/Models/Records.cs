using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Models
{

    public record DecryptedMetricsResponse(string TotalRuns, string TotalDistance, string TotalHours);

    public record DecryptedMetricsAverageResponse(double TotalRuns, double TotalDistance, double TotalSeconds, double AverageSpeed);

    public record RunEntry<T>(T Distance, T Time);
}
