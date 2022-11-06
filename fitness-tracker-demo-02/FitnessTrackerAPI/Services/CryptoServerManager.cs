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
    public class CryptoServerManager : IDisposable, ICryptoServerManager
    {
        private readonly SEALContext _sealContext;
        private bool _disposed;
        private Evaluator _evaluator;
        private Encryptor _encryptor;
        private List<EncryptedRunInfo> _runList = new List<EncryptedRunInfo>();
        private IOptions<FitnessCryptoConfig> _config;
        private Microsoft.Research.SEAL.PublicKey _publicKey;
        private ILogger<CryptoServerManager> _logger;

        public CryptoServerManager(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManager> logger)
        {
            // Initialize context
            _config = config;
            _sealContext = SEALUtils.GetContext(_config.Value.PolyModulusDegree);
            _evaluator = new Evaluator(_sealContext);
            _logger = logger;
        }

        public void SetPublicKey(string publicKeyEncoded)
        {
            _logger.LogDebug("[API]: SetPublicKey - set public key from client");

            _publicKey = SEALUtils.BuildPublicKeyFromBase64String(publicKeyEncoded, _sealContext);
            _encryptor = new Encryptor(_sealContext, _publicKey);
        }

        public void AddRunItem(RunItem request)
        {
            // Add AddRunItem code
            string runInfo = LogUtils.RunItemInfo("API", "AddRunItem", request);
            _logger.LogInformation(runInfo);

            var distance = SEALUtils.BuildCiphertextFromBase64String(request.Distance, _sealContext);
            var time = SEALUtils.BuildCiphertextFromBase64String(request.Time, _sealContext);

            _runList.Add(new EncryptedRunInfo
            {
                Distance = distance,
                Hours = time
            });
        }

        public SummaryItem GetMetrics()
        {
            var totalDistance = SumEncryptedValues(_runList.Select(m => m.Distance));
            var totalHours = SumEncryptedValues(_runList.Select(m => m.Hours));
            var totalMetrics = SEALUtils.CreateCiphertext(_runList.Count(), _encryptor);

            var summaryItem = new SummaryItem
            {
                TotalRuns = SEALUtils.CiphertextToBase64String(totalMetrics),
                TotalDistance = SEALUtils.CiphertextToBase64String(totalDistance),
                TotalHours = SEALUtils.CiphertextToBase64String(totalHours)
            };

            string statsLog = LogUtils.SummaryStatisticInfo("API", "GetMetrics", summaryItem);
            _logger.LogInformation(statsLog);

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
                _publicKey?.Dispose();
                _encryptor?.Dispose();
                _evaluator.Dispose();
                _sealContext.Dispose();
            }

            _runList = null;

            _disposed = true;
        }
    }
}
