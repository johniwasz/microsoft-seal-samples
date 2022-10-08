using FitnessTracker.Common.Models;
using System.Diagnostics;
using System.Text;

namespace FitnessTracker.Common.Utils
{
    public static class LogUtils
    {
        public static string RunItemInfo(string from, string method, RunItem runItem, bool convertFromBase64 = false)
        {
            StringBuilder logText = new StringBuilder();

            if (convertFromBase64)
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] SummaryItem contents");
                logText.AppendLine($"[{from}] \t \t Distance: {SEALUtils.Base64Decode(runItem.Distance)}");
                logText.AppendLine($"[{from}] \t \t Time: {SEALUtils.Base64Decode(runItem.Time)}");
            }
            else
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] SummaryItem contents");
                logText.AppendLine($"[{from}] \t \t Distance: " +
                    $"{(runItem.Distance.Length > 25 ? runItem.Distance.Substring(0, 25) : runItem.Distance)}" +
                    $"{(runItem.Distance.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t Time: " +
                    $"{(runItem.Time.Length > 25 ? runItem.Time.Substring(0, 25) : runItem.Time)}" +
                    $"{(runItem.Time.Length > 25 ? "..." : "")}");
            }

            return logText.ToString();
        }

        public static string SummaryStatisticInfo(string from, string method, SummaryItem summaryItem, bool convertFromBase64 = false)
        {
            StringBuilder logText = new StringBuilder();

            if (convertFromBase64)
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] RunItem received object values as base64");
                logText.AppendLine($"[{from}] \t \t TotalRuns: {SEALUtils.Base64Decode(summaryItem.TotalRuns)}");
                logText.AppendLine($"[{from}] \t \t TotalDistance: {SEALUtils.Base64Decode(summaryItem.TotalDistance)}");
                logText.AppendLine($"[{from}] \t \t TotalHours: {SEALUtils.Base64Decode(summaryItem.TotalHours)}");
            }
            else
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] RunItem received object values as base64");
                logText.AppendLine($"[{from}] \t \t TotalRuns: " +
                    $"{(summaryItem.TotalRuns.Length > 25 ? summaryItem.TotalRuns.Substring(0, 25) : summaryItem.TotalRuns)}" +
                    $"{(summaryItem.TotalRuns.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalDistance: " +
                    $"{(summaryItem.TotalDistance.Length > 25 ? summaryItem.TotalDistance.Substring(0, 25) : summaryItem.TotalDistance)}" +
                    $"{(summaryItem.TotalDistance.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalHours: " +
                    $"{(summaryItem.TotalHours.Length > 25 ? summaryItem.TotalHours.Substring(0, 25) : summaryItem.TotalHours)}" +
                    $"{(summaryItem.TotalHours.Length > 25 ? "..." : "")}");
            }

            return logText.ToString();
        }
    }
}
