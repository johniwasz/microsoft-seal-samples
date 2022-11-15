using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;

namespace FitnessTrackerAPI.Services
{
    public class CryptoServerManagerBFV : CryptoServerManager
    {
        public CryptoServerManagerBFV(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManagerBFV> logger) : base(config, logger)
        {
        }

        public override SchemeType SchemeType => SchemeType.BFV;

    }
}
