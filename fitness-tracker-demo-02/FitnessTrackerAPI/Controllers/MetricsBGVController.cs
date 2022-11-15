using FitnessTracker.Common.Models;
using FitnessTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;
using System;

namespace FitnessTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsBGVController : ControllerBase, IMetricsController
    {
        private ICryptoServerManager _cryptoServerManager;


        public MetricsBGVController(Func<SchemeType, ICryptoServerManager> cryptoManager)
        {            
            _cryptoServerManager = cryptoManager(SchemeType.BGV);
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