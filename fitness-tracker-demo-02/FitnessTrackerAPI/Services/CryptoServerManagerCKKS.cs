using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace FitnessTrackerAPI.Services;

public class CryptoServerManagerCKKS : CryptoServerManager
{
    private CKKSEncoder _encoder;
    private double _scale;
    private List<EncryptedRunInfoCKKS> _runListCKKS = new List<EncryptedRunInfoCKKS>();
    private RelinKeys _relinKeys;

    public CryptoServerManagerCKKS(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManagerCKKS> logger) : base(config, logger)
    {
        _encoder = new CKKSEncoder(_sealContext);
        _scale = Math.Pow(2.0, 40);
    }

    public override SchemeType SchemeType => SchemeType.CKKS;

    public void SetRelinKeys(string relinKeysEncoded)
    {
        _logger?.LogDebug("[API]: SetRelinKeys - set relin keys from client");

        using KeyGenerator keygen = new KeyGenerator(_sealContext);

        keygen.CreateRelinKeys(out RelinKeys relinKeys);
        _relinKeys = relinKeys;

        _relinKeys = SEALUtils.BuildRelinKeysFromBase64String(relinKeysEncoded, _sealContext);
    }


    public void AddRunItemCKKS(RunItemCKKS request)
    {
        // Add AddRunItem code
        string runInfo = LogUtils.RunItemInfoCKKS("API", "AddRunItemCKKS", request);
        _logger?.LogInformation(runInfo);

        var distance = SEALUtils.BuildCiphertextFromBase64String(request.Distance, _sealContext);
        var time = SEALUtils.BuildCiphertextFromBase64String(request.Time, _sealContext);
        var timeReciprocal = SEALUtils.BuildCiphertextFromBase64String(request.TimeReciprocal, _sealContext);

        Ciphertext speed = new();

        _evaluator.Multiply(distance, timeReciprocal, speed);
        //_evaluator.RelinearizeInplace(speed, _relinKeys);

        _runListCKKS.Add(new EncryptedRunInfoCKKS(distance, time, speed));
    }

    public SummaryItemCKKS GetMetrics()
    {
        int count = _runListCKKS.Count;

        var totalDistanceCKKS = SumEncryptedValues(_runListCKKS.Select(m => m.Distance));
        var totalTimeCKKS = SumEncryptedValues(_runListCKKS.Select(m => m.Time));
        var totalSpeed = SumEncryptedValues(_runListCKKS.Select(m => m.Speed));
      

        // Encode and encrypt the total number of runs sent
        List<double> runCount = new() { (double)count };
        Plaintext encodedCount = new();
        _encoder.Encode(runCount, _scale, encodedCount);
        Ciphertext ecryptedTotal = new();
        _encryptor.Encrypt(encodedCount, ecryptedTotal);


        // Get the average pace
        Plaintext encodedCountReciprocal = new();
        List<double> averagePaceList = new();
        double runCountReciprocal = 1 / (double)count;
        averagePaceList.Add(runCountReciprocal);
        _encoder.Encode(averagePaceList, _scale, encodedCountReciprocal);
        Ciphertext encryptedPace = new();

        
        _evaluator.MultiplyPlainInplace(totalSpeed, encodedCountReciprocal);
        _evaluator.RelinearizeInplace(totalSpeed, _relinKeys);
        

        SummaryItemCKKS summaryItem = new(
            SEALUtils.CiphertextToBase64String(ecryptedTotal),
            SEALUtils.CiphertextToBase64String(totalDistanceCKKS),
            SEALUtils.CiphertextToBase64String(totalTimeCKKS),
            SEALUtils.CiphertextToBase64String(totalSpeed));

        string statsLog = LogUtils.SummaryStatisticInfoCKKS("API", "GetMetrics", summaryItem);
        _logger?.LogInformation(statsLog);

        return summaryItem;
    }
}
