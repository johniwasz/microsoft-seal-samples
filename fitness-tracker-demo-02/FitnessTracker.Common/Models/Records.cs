using Microsoft.Research.SEAL;
using System.Collections.Generic;

namespace FitnessTracker.Common.Models;

public record EncryptedRunInfoBGV(Ciphertext Distance, Ciphertext Hours);

public record EncryptedRunInfoCKKS(Ciphertext Distance, Ciphertext Time, Ciphertext Speed);

public record PublicKeyModelBGV(string PublicKey);

public record PublicKeyModelCKKS(string PublicKey, string RelinearizationKeys);

public record RunItemBGV(string Distance, string Time, string TimeReciprocal);

public record RunItemCKKS(string Distance, string Time, string TimeReciprocal);

public record SummaryItemBGV(string TotalRuns, string TotalDistance, string TotalTime);

public record SummaryItemCKKS(string TotalRuns, string TotalDistance, string TotalTime, string AverageSpeed);