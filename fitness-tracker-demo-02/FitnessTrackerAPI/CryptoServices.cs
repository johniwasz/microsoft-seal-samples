using FitnessTracker.Common.Models;
using FitnessTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Research.SEAL;
using System.Threading.Tasks;

namespace FitnessTrackerAPI
{
    public static class CryptoServices
    {

        internal static void SavePublicKeyBGV(
            CryptoServerManagerBGV cryptoManager,
            [FromBody] PublicKeyModel publicKeyEncoded)
        {
            cryptoManager.SetPublicKey(publicKeyEncoded.PublicKey);
        }

        internal static void SaveRunBGV(
            CryptoServerManagerBGV cryptoManager, 
            [FromBody] RunItem request)
        {
            cryptoManager.AddRunItem(request);
        }

        internal static SummaryItem GetMetricsBGV(CryptoServerManagerBGV cryptoManager)
        {
            var summaryItem = cryptoManager.GetMetrics();
            return summaryItem;
        }

        internal static void SavePublicKeyCKKS(
            CryptoServerManagerCKKS cryptoManager,
            [FromBody] PublicKeyModel publicKeyEncoded)
        {
            cryptoManager.SetPublicKey(publicKeyEncoded.PublicKey);
        }


        internal static void SaveRunCKKS(
            CryptoServerManagerCKKS cryptoManager, 
            [FromBody] RunItem request)
        {
            cryptoManager.AddRunItem(request);
        }

        internal static SummaryItem GetMetricsCKKS(
            CryptoServerManagerCKKS cryptoManager)
        {
            return cryptoManager.GetMetrics();
        }
    }
}
