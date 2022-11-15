using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;

namespace FitnessTrackerAPI.Services
{
    public class CryptoServerManagerBGV : CryptoServerManager
    {
        public CryptoServerManagerBGV(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManagerBGV> logger) : base(config, logger)
        {
        }

        public override SchemeType SchemeType => SchemeType.BGV;

    }
}
