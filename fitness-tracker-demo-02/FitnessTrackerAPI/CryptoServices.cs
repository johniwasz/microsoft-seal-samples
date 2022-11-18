using FitnessTracker.Common.Models;
using FitnessTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Research.SEAL;
using System.Threading.Tasks;

namespace FitnessTrackerAPI;

public static class CryptoServices
{

    internal static void SavePublicKeyBGV(
        CryptoServerManagerBGV cryptoManager,
        [FromBody] PublicKeyModelBGV publicKeyEncoded)
    {
        cryptoManager.SetPublicKey(publicKeyEncoded.PublicKey);
    }

    internal static void SaveRunBGV(
        CryptoServerManagerBGV cryptoManager, 
        [FromBody] RunItemBGV request)
    {
        cryptoManager.AddRunItemBGV(request);
    }

    internal static SummaryItemBGV GetMetricsBGV(CryptoServerManagerBGV cryptoManager)
    {
        var summaryItem = cryptoManager.GetMetrics();
        return summaryItem;
    }

    internal static void SavePublicKeyCKKS(
        CryptoServerManagerCKKS cryptoManager,
        [FromBody] PublicKeyModelCKKS publicKeyEncoded)
    {
        cryptoManager.SetPublicKey(publicKeyEncoded.PublicKey);
        cryptoManager.SetRelinKeys(publicKeyEncoded.RelinearizationKeys);
    }


    internal static void SaveRunCKKS(
        CryptoServerManagerCKKS cryptoManager, 
        [FromBody] RunItemCKKS request)
    {
        cryptoManager.AddRunItemCKKS(request);
    }

    internal static SummaryItemCKKS GetMetricsCKKS(
        CryptoServerManagerCKKS cryptoManager)
    {
        return cryptoManager.GetMetrics();
    }
}
