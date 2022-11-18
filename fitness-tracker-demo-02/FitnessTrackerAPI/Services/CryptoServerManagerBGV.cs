using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using System.Collections.Generic;
using System.Linq;

namespace FitnessTrackerAPI.Services;

public class CryptoServerManagerBGV : CryptoServerManager
{

    private List<EncryptedRunInfoBGV> _runListBGV = new List<EncryptedRunInfoBGV>();
    

    public CryptoServerManagerBGV(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManagerBGV> logger) : base(config, logger)
    {
    }

    public override SchemeType SchemeType => SchemeType.BGV;


    public void AddRunItemBGV(RunItemBGV request)
    {
        // Add AddRunItem code
        string runInfo = LogUtils.RunItemInfoBGV("API", "AddRunItemBGV", request);
        _logger?.LogInformation(runInfo);

        var distance = SEALUtils.BuildCiphertextFromBase64String(request.Distance, _sealContext);
        var time = SEALUtils.BuildCiphertextFromBase64String(request.Time, _sealContext);
        _runListBGV.Add(new EncryptedRunInfoBGV(distance, time));
    }

    public SummaryItemBGV GetMetrics()
    {
        var totalDistanceBGV = SumEncryptedValues(_runListBGV.Select(m => m.Distance));
        var totalHoursBGV = SumEncryptedValues(_runListBGV.Select(m => m.Hours));
        var totalMetricsBGV = SEALUtils.CreateCiphertext(_runListBGV.Count, _encryptor);

        SummaryItemBGV summaryItem = new(
            SEALUtils.CiphertextToBase64String(totalMetricsBGV),
            SEALUtils.CiphertextToBase64String(totalDistanceBGV),
            SEALUtils.CiphertextToBase64String(totalHoursBGV));

        return summaryItem;
    }
}
