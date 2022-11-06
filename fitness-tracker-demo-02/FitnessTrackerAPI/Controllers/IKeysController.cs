using FitnessTracker.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrackerAPI.Controllers
{
    public interface IKeysController
    {
        ActionResult SetPublicKey(PublicKeyModel publicKeyEncoded);

    }
}
