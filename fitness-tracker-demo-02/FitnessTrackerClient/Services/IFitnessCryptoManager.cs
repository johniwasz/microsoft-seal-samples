using Microsoft.Research.SEAL;
using System;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public interface IFitnessCryptoManager : IDisposable
    {
        public Task InitializeAsync();

        public PublicKey PublicKey
        {
            get;
        }

    }
}
