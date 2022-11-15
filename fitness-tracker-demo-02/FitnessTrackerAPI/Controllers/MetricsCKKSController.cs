using FitnessTracker.Common.Models;
using FitnessTrackerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;
using System;

namespace FitnessTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsCKKSController : Controller
    { 
        private ICryptoServerManager _cryptoServerManager;

        public MetricsCKKSController(Func<SchemeType, ICryptoServerManager> cryptoManager)
        {
            _cryptoServerManager = cryptoManager(SchemeType.CKKS);
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
