using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using System;

namespace FitnessTrackerAPI.Services
{
    public class CryptoServerManagerCKKS : CryptoServerManager
    {
        public CryptoServerManagerCKKS(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManagerCKKS> logger) : base(config, logger)
        {
        }

        public override SchemeType SchemeType => SchemeType.CKKS;
    }
}
