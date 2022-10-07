using FitnessTracker.Common.Models;
using System.Diagnostics;

namespace FitnessTracker.Common.Utils
{
    public static class LogUtils
    {
        public static void RunItemInfo(string from, string method, RunItem runItem, bool convertFromBase64 = false)
        {
            if (convertFromBase64)
            {
                Debug.WriteLine($"[{from}] {method}");
                Debug.WriteLine($"[{from}] SummaryItem contents");
                Debug.WriteLine($"[{from}] \t \t Distance: {SEALUtils.Base64Decode(runItem.Distance)}");
                Debug.WriteLine($"[{from}] \t \t Time: {SEALUtils.Base64Decode(runItem.Time)}");
            }
            else
            {
                Debug.WriteLine($"[{from}] {method}");
                Debug.WriteLine($"[{from}] SummaryItem contents");
                Debug.WriteLine($"[{from}] \t \t Distance: " +
                    $"{(runItem.Distance.Length > 25 ? runItem.Distance.Substring(0, 25) : runItem.Distance)}" +
                    $"{(runItem.Distance.Length > 25 ? "..." : "")}");
                Debug.WriteLine($"[{from}] \t \t Time: " +
                    $"{(runItem.Time.Length > 25 ? runItem.Time.Substring(0, 25) : runItem.Time)}" +
                    $"{(runItem.Time.Length > 25 ? "..." : "")}");
            }
        }

        public static void SummaryStatisticInfo(string from, string method, SummaryItem summaryItem, bool convertFromBase64 = false)
        {
            if (convertFromBase64)
            {
                Debug.WriteLine($"[{from}] {method}");
                Debug.WriteLine($"[{from}] RunItem received object values as base64");
                Debug.WriteLine($"[{from}] \t \t TotalRuns: {SEALUtils.Base64Decode(summaryItem.TotalRuns)}");
                Debug.WriteLine($"[{from}] \t \t TotalDistance: {SEALUtils.Base64Decode(summaryItem.TotalDistance)}");
                Debug.WriteLine($"[{from}] \t \t TotalHours: {SEALUtils.Base64Decode(summaryItem.TotalHours)}");
            }
            else
            {
                Debug.WriteLine($"[{from}] {method}");
                Debug.WriteLine($"[{from}] RunItem received object values as base64");
                Debug.WriteLine($"[{from}] \t \t TotalRuns: " +
                    $"{(summaryItem.TotalRuns.Length > 25 ? summaryItem.TotalRuns.Substring(0, 25) : summaryItem.TotalRuns)}" +
                    $"{(summaryItem.TotalRuns.Length > 25 ? "..." : "")}");
                Debug.WriteLine($"[{from}] \t \t TotalDistance: " +
                    $"{(summaryItem.TotalDistance.Length > 25 ? summaryItem.TotalDistance.Substring(0, 25) : summaryItem.TotalDistance)}" +
                    $"{(summaryItem.TotalDistance.Length > 25 ? "..." : "")}");
                Debug.WriteLine($"[{from}] \t \t TotalHours: " +
                    $"{(summaryItem.TotalHours.Length > 25 ? summaryItem.TotalHours.Substring(0, 25) : summaryItem.TotalHours)}" +
                    $"{(summaryItem.TotalHours.Length > 25 ? "..." : "")}");
            }
        }
    }
}
