using FitnessTracker.Common.Models;

namespace FitnessTrackerAPI.Services;

public interface ICryptoServerManager
{ 
    void SetPublicKey(string publicKeyEncoded);
}