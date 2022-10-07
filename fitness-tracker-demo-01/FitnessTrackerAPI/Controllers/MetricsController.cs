using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;

namespace FitnessTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private List<double> _distances = new List<double>();
        private List<double> _times = new List<double>();

        private readonly SEALContext _sealContext;

        private Evaluator _evaluator;
        private Encryptor _encryptor;
        private List<ClientData> _metrics = new List<ClientData>();
        private Microsoft.Research.SEAL.PublicKey _publicKey;

        public MetricsController()
        {
            // Initialize context
            _sealContext = SEALUtils.GetContext();
            _evaluator = new Evaluator(_sealContext);
        }


        [HttpPost]
        [Route("keys")]
        public ActionResult SetPublicKey(PublicKeyModel publicKeyEncoded)
        {
            Debug.WriteLine("[API]: SetPublicKey - set public key from client");

            _publicKey = SEALUtils.BuildPublicKeyFromBase64String(publicKeyEncoded.PublicKey, _sealContext);
            _encryptor = new Encryptor(_sealContext, _publicKey);

            return Ok();
        }


        [HttpPost]
        [Route("")]
        public ActionResult AddRunItem([FromBody] RunItem request)
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

            return Ok();
        }

        [HttpGet]
        [Route("")]
        public ActionResult<SummaryItem> GetMetrics()
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

            return Ok(summaryItem);
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
    }
}