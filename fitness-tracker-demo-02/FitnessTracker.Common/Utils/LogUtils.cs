using FitnessTracker.Common.Models;
using System.Diagnostics;
using System.Text;

namespace FitnessTracker.Common.Utils
{
    public static class LogUtils
    {
        public static string RunItemInfoBGV(string from, string method, RunItemBGV runItem, bool convertFromBase64 = false)
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
                logText.AppendLine($"[{from}] \t \t Distance (Bytes): {SEALUtils.GetByteLength(runItem.Distance)}");

                logText.AppendLine($"[{from}] \t \t Time: " +
                    $"{(runItem.Time.Length > 25 ? runItem.Time.Substring(0, 25) : runItem.Time)}" +
                    $"{(runItem.Time.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t Time (Bytes): {SEALUtils.GetByteLength(runItem.Time)}");
            }

            return logText.ToString();
        }

        public static string RunItemInfoCKKS(string from, string method, RunItemCKKS runItem, bool convertFromBase64 = false)
        {
            StringBuilder logText = new StringBuilder();

            if (convertFromBase64)
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] SummaryItem contents");
                logText.AppendLine($"[{from}] \t \t Distance: {SEALUtils.Base64Decode(runItem.Distance)}");
                logText.AppendLine($"[{from}] \t \t Time: {SEALUtils.Base64Decode(runItem.Time)}");
                logText.AppendLine($"[{from}] \t \t Time Reciprocal: {SEALUtils.Base64Decode(runItem.TimeReciprocal)}");
            }
            else
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] SummaryItem contents");
                logText.AppendLine($"[{from}] \t \t Distance: " +
                    $"{(runItem.Distance.Length > 25 ? runItem.Distance.Substring(0, 25) : runItem.Distance)}" +
                    $"{(runItem.Distance.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t Distance (Bytes): {SEALUtils.GetByteLength(runItem.Distance)}");

                logText.AppendLine($"[{from}] \t \t Time: " +
                    $"{(runItem.Time.Length > 25 ? runItem.Time.Substring(0, 25) : runItem.Time)}" +
                    $"{(runItem.Time.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t Time (Bytes): {SEALUtils.GetByteLength(runItem.Time)}");

                logText.AppendLine($"[{from}] \t \t Time Reciprocal: " +
                    $"{(runItem.Time.Length > 25 ? runItem.Time.Substring(0, 25) : runItem.TimeReciprocal)}" +
                    $"{(runItem.Time.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t Time (Bytes): {SEALUtils.GetByteLength(runItem.TimeReciprocal)}");
            }

            return logText.ToString();
        }

        public static string SummaryStatisticInfoBGV(string from, string method, SummaryItemBGV summaryItem, bool convertFromBase64 = false)
        {
            StringBuilder logText = new();

            if (convertFromBase64)
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] RunItem received object values as base64");
                logText.AppendLine($"[{from}] \t \t TotalRuns: {SEALUtils.Base64Decode(summaryItem.TotalRuns)}");
                logText.AppendLine($"[{from}] \t \t TotalDistance: {SEALUtils.Base64Decode(summaryItem.TotalDistance)}");
                logText.AppendLine($"[{from}] \t \t TotalHours: {SEALUtils.Base64Decode(summaryItem.TotalTime)}");
            }
            else
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] RunItem received object values as base64");
                logText.AppendLine($"[{from}] \t \t TotalRuns: " +
                    $"{(summaryItem.TotalRuns.Length > 25 ? summaryItem.TotalRuns.Substring(0, 25) : summaryItem.TotalRuns)}" +
                    $"{(summaryItem.TotalRuns.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalRuns (Bytes): {SEALUtils.GetByteLength(summaryItem.TotalRuns)}");

                logText.AppendLine($"[{from}] \t \t TotalDistance: " +
                    $"{(summaryItem.TotalDistance.Length > 25 ? summaryItem.TotalDistance.Substring(0, 25) : summaryItem.TotalDistance)}" +
                    $"{(summaryItem.TotalDistance.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalDistance (Bytes): {SEALUtils.GetByteLength(summaryItem.TotalDistance)}");

                logText.AppendLine($"[{from}] \t \t TotalHours: " +
                    $"{(summaryItem.TotalTime.Length > 25 ? summaryItem.TotalTime.Substring(0, 25) : summaryItem.TotalTime)}" +
                    $"{(summaryItem.TotalTime.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalHours (Bytes): {SEALUtils.GetByteLength(summaryItem.TotalTime)}");
            }

            return logText.ToString();
        }

        public static string SummaryStatisticInfoCKKS(string from, string method, SummaryItemCKKS summaryItem, bool convertFromBase64 = false)
        {
            StringBuilder logText = new StringBuilder();

            if (convertFromBase64)
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] RunItem received object values as base64");
                logText.AppendLine($"[{from}] \t \t TotalRuns: {SEALUtils.Base64Decode(summaryItem.TotalRuns)}");
                logText.AppendLine($"[{from}] \t \t TotalDistance: {SEALUtils.Base64Decode(summaryItem.TotalDistance)}");
                logText.AppendLine($"[{from}] \t \t TotalHours: {SEALUtils.Base64Decode(summaryItem.TotalTime)}");
                logText.AppendLine($"[{from}] \t \t TotalHours: {SEALUtils.Base64Decode(summaryItem.AverageSpeed)}");
            }
            else
            {
                logText.AppendLine($"[{from}] {method}");
                logText.AppendLine($"[{from}] RunItem received object values as base64");
                logText.AppendLine($"[{from}] \t \t TotalRuns: " +
                    $"{(summaryItem.TotalRuns.Length > 25 ? summaryItem.TotalRuns.Substring(0, 25) : summaryItem.TotalRuns)}" +
                    $"{(summaryItem.TotalRuns.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalRuns (Bytes): {SEALUtils.GetByteLength(summaryItem.TotalRuns)}");

                logText.AppendLine($"[{from}] \t \t TotalDistance: " +
                    $"{(summaryItem.TotalDistance.Length > 25 ? summaryItem.TotalDistance.Substring(0, 25) : summaryItem.TotalDistance)}" +
                    $"{(summaryItem.TotalDistance.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalDistance (Bytes): {SEALUtils.GetByteLength(summaryItem.TotalDistance)}");

                logText.AppendLine($"[{from}] \t \t TotalHours: " +
                    $"{(summaryItem.TotalTime.Length > 25 ? summaryItem.TotalTime.Substring(0, 25) : summaryItem.TotalTime)}" +
                    $"{(summaryItem.TotalTime.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalHours (Bytes): {SEALUtils.GetByteLength(summaryItem.TotalTime)}");

                logText.AppendLine($"[{from}] \t \t TotalHours: " +
                    $"{(summaryItem.TotalTime.Length > 25 ? summaryItem.TotalTime.Substring(0, 25) : summaryItem.AverageSpeed)}" +
                    $"{(summaryItem.TotalTime.Length > 25 ? "..." : "")}");
                logText.AppendLine($"[{from}] \t \t TotalHours (Bytes): {SEALUtils.GetByteLength(summaryItem.AverageSpeed)}");
            }

            return logText.ToString();
        }
    }
}
