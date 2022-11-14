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
    public class KeysController : ControllerBase, IKeysController
    {
        private ICryptoServerManager _cryptoServerManager;

        private Func<SchemeType, ICryptoServerManager> _cryptoManager;

        public KeysController(Func<SchemeType, ICryptoServerManager> cryptoManager)
        {
            _cryptoManager = cryptoManager;

        }

        [HttpPost]
        [Route("")]
        public ActionResult SetPublicKey([FromBody] PublicKeyModel publicKeyEncoded)
        {
            ICryptoServerManager cryptoServerManager = _cryptoManager(publicKeyEncoded.SchemeType);
            cryptoServerManager.SetPublicKey(publicKeyEncoded.PublicKey);

            return Ok();
        }

    }
}
