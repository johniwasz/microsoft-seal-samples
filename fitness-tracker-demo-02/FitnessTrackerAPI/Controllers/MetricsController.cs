using FitnessTracker.Common.Models;
using FitnessTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;

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