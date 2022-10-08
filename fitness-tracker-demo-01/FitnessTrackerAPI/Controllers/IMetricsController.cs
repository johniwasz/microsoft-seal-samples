using FitnessTracker.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrackerAPI.Controllers
{
    public interface IMetricsController
    {
        ActionResult AddRunItem([FromBody] RunItem request);
        ActionResult<SummaryItem> GetMetrics();
        ActionResult SetPublicKey(PublicKeyModel publicKeyEncoded);
    }
}