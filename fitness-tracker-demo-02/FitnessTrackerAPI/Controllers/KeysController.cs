using FitnessTracker.Common.Models;
using FitnessTrackerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase, IKeysController
    {
        private ICryptoServerManager _cryptoServerManager;

        public KeysController(ICryptoServerManager cryptoManager)
        {
            _cryptoServerManager = cryptoManager;
        }

        [HttpPost]
        [Route("")]
        public ActionResult SetPublicKey([FromBody] PublicKeyModel publicKeyEncoded)
        {
            _cryptoServerManager.SetPublicKey(publicKeyEncoded.PublicKey);

            return Ok();
        }

    }
}
