using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FitnessTrackerAPI.Services
{
    public abstract class CryptoServerManager : IDisposable, ICryptoServerManager
    {
        private readonly SEALContext _sealContext;
        private bool _disposed;
        private Evaluator _evaluator;
        private Encryptor _encryptor;
        private List<EncryptedRunInfoBGV> _runListBGV = new List<EncryptedRunInfoBGV>();
        private List<EncryptedRunInfoCKKS> _runListCKKS = new List<EncryptedRunInfoCKKS>();
        private IOptions<FitnessCryptoConfig> _config;
        private Microsoft.Research.SEAL.PublicKey _publicKey;
        private ILogger<CryptoServerManager> _logger;
        private double _scale;

        private CKKSEncoder _encoder;

        public CryptoServerManager(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManager> logger)
        {
            // Initialize context
            _config = config;
            _sealContext = SEALUtils.GetContext(_config.Value.PolyModulusDegree, this.SchemeType);
            _evaluator = new Evaluator(_sealContext);
            _logger = logger;
          
            if(this.SchemeType == SchemeType.CKKS)
            {
                _encoder = new CKKSEncoder(_sealContext);
                _scale = Math.Pow(2.0, 40);
            }
        }


        public abstract SchemeType SchemeType
        {
            get;
        }

        public void SetPublicKey(string publicKeyEncoded)
        {
            _logger?.LogDebug("[API]: SetPublicKey - set public key from client");

            _publicKey = SEALUtils.BuildPublicKeyFromBase64String(publicKeyEncoded, _sealContext);
            _encryptor = new Encryptor(_sealContext, _publicKey);
        }

        public void AddRunItem(RunItem request)
        {
            // Add AddRunItem code
            string runInfo = LogUtils.RunItemInfo("API", "AddRunItem", request);
            _logger?.LogInformation(runInfo);

            var distance = SEALUtils.BuildCiphertextFromBase64String(request.Distance, _sealContext);
            var time = SEALUtils.BuildCiphertextFromBase64String(request.Time, _sealContext);

            switch(this.SchemeType)
            {
                case SchemeType.BGV:
                    _runListBGV.Add(new EncryptedRunInfoBGV
                    {
                        Distance = distance,
                        Hours = time
                    });
                    break;
                case SchemeType.CKKS:

                    var timeReciprocal = SEALUtils.BuildCiphertextFromBase64String(request.TimeReciprocal, _sealContext);

                    Ciphertext speed = new Ciphertext();

                    _evaluator.Multiply(distance, timeReciprocal, speed);

                    _runListCKKS.Add(new EncryptedRunInfoCKKS
                    {
                        Distance = distance,
                        Time = time,
                        Speed = speed
                    });
                    break;
            }

            
        }

        public SummaryItem GetMetrics()
        {

            var summaryItem = new SummaryItem();

            switch (this.SchemeType)
            {
                case SchemeType.BGV:
                    var totalDistanceBGV = SumEncryptedValues(_runListBGV.Select(m => m.Distance));
                    var totalHoursBGV = SumEncryptedValues(_runListBGV.Select(m => m.Hours));
                    var totalMetricsBGV = SEALUtils.CreateCiphertext(_runListBGV.Count(), _encryptor);


                    summaryItem.TotalRuns = SEALUtils.CiphertextToBase64String(totalMetricsBGV);
                    summaryItem.TotalDistance = SEALUtils.CiphertextToBase64String(totalDistanceBGV);
                    summaryItem.TotalTime = SEALUtils.CiphertextToBase64String(totalHoursBGV);

                    break;
                case SchemeType.CKKS:

                    int count = _runListCKKS.Count();

                    var totalDistanceCKKS = SumEncryptedValues(_runListCKKS.Select(m => m.Distance));
                    var totalTimeCKKS = SumEncryptedValues(_runListCKKS.Select(m => m.Time));
                    var totalSpeed = SumEncryptedValues(_runListCKKS.Select(m => m.Speed));


                    // Encode and encrypt the total number of runs sent
                    List<double> runCount = new List<double>() { (double)count };
                    Plaintext encodedCount = new Plaintext();
                    _encoder.Encode(runCount, _scale, encodedCount);
                    Ciphertext ecryptedTotal = new Ciphertext();
                    _encryptor.Encrypt(encodedCount, ecryptedTotal);


                    // Get the average pace
                    Plaintext encodedCountReciprocal = new Plaintext();
                    List<double> averagePaceList = new List<double>();
                    double runCountReciprocal = 1 / (double)count;
                    averagePaceList.Add(runCountReciprocal);
                    _encoder.Encode(averagePaceList, _scale, encodedCountReciprocal);            
                    Ciphertext encryptedPace = new Ciphertext();
                    _evaluator.MultiplyPlain(totalSpeed, encodedCountReciprocal, encryptedPace);

                  
                    summaryItem.TotalRuns = SEALUtils.CiphertextToBase64String(ecryptedTotal);
                    summaryItem.TotalDistance = SEALUtils.CiphertextToBase64String(totalDistanceCKKS);
                    summaryItem.TotalTime = SEALUtils.CiphertextToBase64String(totalTimeCKKS);
                    summaryItem.AverageSpeed = SEALUtils.CiphertextToBase64String(encryptedPace);
                    break;
            }

            string statsLog = LogUtils.SummaryStatisticInfo("API", "GetMetrics", summaryItem);
            _logger?.LogInformation(statsLog);

            return summaryItem;
        }

        private Ciphertext GetRunCount(int count)
        {
            Ciphertext retCipherText = null;

            if (this.SchemeType == SchemeType.CKKS)
            {
                Plaintext encodedCount = new Plaintext();

                List<double> runCount = new List<double>() {  (double)count };

                _encoder.Encode(runCount, _scale, encodedCount);

                retCipherText = new Ciphertext();
                _encryptor.Encrypt(encodedCount, retCipherText);

            }
            else
            {
                retCipherText = SEALUtils.CreateCiphertext(_runListBGV.Count(), _encryptor);
            }

            return retCipherText;
        }

        private Ciphertext SumEncryptedValues(IEnumerable<Ciphertext> encryptedData)
        {
            if (encryptedData.Any())
            {
                Ciphertext encTotal = new Ciphertext();
                _evaluator.AddMany(encryptedData, encTotal);
                return encTotal;
            }
            else
            {
                return SEALUtils.CreateCiphertext(0, _encryptor);
            }
        }


        public void Dispose() => Dispose(true);

        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _encoder?.Dispose();
                _publicKey?.Dispose();
                _encryptor?.Dispose();
                _evaluator.Dispose();
                _sealContext.Dispose();
            }

            _runListBGV = null;

            _runListCKKS = null;

            _disposed = true;
        }
    }
}
