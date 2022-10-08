using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FitnessTrackerAPI.Services
{
    public class CryptoServerManager : IDisposable, ICryptoServerManager
    {
        private readonly SEALContext _sealContext;
        private bool _disposed;
        private Evaluator _evaluator;
        private Encryptor _encryptor;
        private List<ClientData> _metrics = new List<ClientData>();
        private IOptions<FitnessCryptoConfig> _config;
        private Microsoft.Research.SEAL.PublicKey _publicKey;

        public CryptoServerManager(IOptions<FitnessCryptoConfig> config)
        {
            // Initialize context
            _config = config;
            _sealContext = SEALUtils.GetContext(_config.Value.PolyModulusDegree);
            _evaluator = new Evaluator(_sealContext);
        }

        public void SetPublicKey(string publicKeyEncoded)
        {
            Debug.WriteLine("[API]: SetPublicKey - set public key from client");

            _publicKey = SEALUtils.BuildPublicKeyFromBase64String(publicKeyEncoded, _sealContext);
            _encryptor = new Encryptor(_sealContext, _publicKey);
        }

        public void AddRunItem(RunItem request)
        {
            // Add AddRunItem code
            LogUtils.RunItemInfo("API", "AddRunItem", request);
            var distance = SEALUtils.BuildCiphertextFromBase64String(request.Distance, _sealContext);
            var time = SEALUtils.BuildCiphertextFromBase64String(request.Time, _sealContext);

            _metrics.Add(new ClientData
            {
                Distance = distance,
                Hours = time
            });
        }

        public SummaryItem GetMetrics()
        {
            var totalDistance = SumEncryptedValues(_metrics.Select(m => m.Distance));
            var totalHours = SumEncryptedValues(_metrics.Select(m => m.Hours));
            var totalMetrics = SEALUtils.CreateCiphertextFromInt(_metrics.Count(), _encryptor);

            var summaryItem = new SummaryItem
            {
                TotalRuns = SEALUtils.CiphertextToBase64String(totalMetrics),
                TotalDistance = SEALUtils.CiphertextToBase64String(totalDistance),
                TotalHours = SEALUtils.CiphertextToBase64String(totalHours)
            };

            LogUtils.SummaryStatisticInfo("API", "GetMetrics", summaryItem);

            return summaryItem;
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
                return SEALUtils.CreateCiphertextFromInt(0, _encryptor);
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
                _publicKey?.Dispose();
                _encryptor?.Dispose();
                _evaluator.Dispose();
                _sealContext.Dispose();
            }

            _metrics = null;

            _disposed = true;
        }
    }
}
