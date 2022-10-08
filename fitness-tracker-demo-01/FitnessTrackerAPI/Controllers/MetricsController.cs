using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;

namespace FitnessTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase, IMetricsController
    {

        private ICryptoServerManager _cryptoServerManager;

        public MetricsController(ICryptoServerManager cryptoManager)
        {
            _cryptoServerManager = cryptoManager;
        }


        [HttpPost]
        [Route("keys")]
        public ActionResult SetPublicKey(PublicKeyModel publicKeyEncoded)
        {
            _cryptoServerManager.SetPublicKey(publicKeyEncoded.PublicKey);

            return Ok();
        }


        [HttpPost]
        [Route("")]
        public ActionResult AddRunItem([FromBody] RunItem request)
        {
            _cryptoServerManager.AddRunItem(request);
            return Ok();
        }

        [HttpGet]
        [Route("")]
        public ActionResult<SummaryItem> GetMetrics()
        {
            var summaryItem = _cryptoServerManager.GetMetrics();
            return Ok(summaryItem);
        }

    }
}