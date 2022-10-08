using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using Microsoft.Research.SEAL;
using System;
using System.Threading.Tasks;

namespace FitnessTrackerClient
{
    internal class FitnessCryptoManager : IDisposable
    {
        private bool _disposed;
        private Encryptor _encryptor;
        private Decryptor _decryptor;
        private SEALContext _context;
        private KeyGenerator _keyGenerator;
        private Microsoft.Research.SEAL.PublicKey _publicKey;

        internal async Task Initialize()
        {
            Console.WriteLine("SEAL LAB");
            Console.WriteLine("Setting up encryption...\n");

            // Add Initialization code here
            _context = SEALUtils.GetContext();

            _keyGenerator = new KeyGenerator(_context);

            _keyGenerator.CreatePublicKey(out Microsoft.Research.SEAL.PublicKey publicKey);

            _publicKey = publicKey;

            PublicKeyModel keyModel = new PublicKeyModel();
            keyModel.PublicKey = SEALUtils.PublicKeyToBase64String(_publicKey);
            await FitnessTrackerClient.SendPublicKey(keyModel);

            _encryptor = new Encryptor(_context, _publicKey);

            _decryptor = new Decryptor(_context, _keyGenerator.SecretKey);
        }


        internal async Task SendNewRun()
        {
            // Get distance from user
            Console.Write("Enter the new running distance (km): ");
            var newRunningDistance = Convert.ToInt32(Console.ReadLine());

            if (newRunningDistance < 0)
            {
                Console.WriteLine("Running distance must be greater than 0.");
                return;
            }

            // We will convert the Int value to Hexadecimal using the ToString("X") method
            var plaintext = new Plaintext($"{newRunningDistance.ToString("X")}");
            var ciphertextDistance = new Ciphertext();
            _encryptor.Encrypt(plaintext, ciphertextDistance);

            // Convert value to base64 string
            var base64Distance = SEALUtils.CiphertextToBase64String(ciphertextDistance);

            // Get time from user
            Console.Write("Enter the new running time (hours): ");
            var newRunningTime = Convert.ToInt32(Console.ReadLine());

            if (newRunningTime < 0)
            {
                Console.WriteLine("Running time must be greater than 0.");
                return;
            }

            // We will convert the Int value to Hexadecimal using the ToString("X") method
            var plaintextTime = new Plaintext($"{newRunningTime.ToString("X")}");
            var ciphertextTime = new Ciphertext();
            _encryptor.Encrypt(plaintextTime, ciphertextTime);

            // Convert value to base64 string
            var base64Time = SEALUtils.CiphertextToBase64String(ciphertextTime);

            var metricsRequest = new RunItem
            {
                Distance = base64Distance,
                Time = base64Time
            };

            LogUtils.RunItemInfo("CLIENT", "SendNewRun", metricsRequest);

            // Send new run to api
            await FitnessTrackerClient.AddNewRunningDistance(metricsRequest);
        }
       
        internal async Task<DecryptedMetricsResponse> GetMetrics()
        {
            // Get encrypted metrics
            var metrics = await FitnessTrackerClient.GetMetrics();

            DecryptedMetricsResponse response = new DecryptedMetricsResponse();

            LogUtils.SummaryStatisticInfo("CLIENT", "GetMetrics", metrics);

            // Decrypt the data
            var ciphertextTotalRuns = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalRuns, _context);
            var plaintextTotalRuns = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalRuns, plaintextTotalRuns);
            response.TotalRuns = plaintextTotalRuns.ToString();

            var ciphertextTotalDistance = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalDistance, _context);
            var plaintextTotalDistance = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalDistance, plaintextTotalDistance);
            response.TotalDistance = plaintextTotalDistance.ToString();

            var ciphertextTotalHours = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalHours, _context);
            var plaintextTotalHours = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalHours, plaintextTotalHours);
            response.TotalHours = plaintextTotalHours.ToString();


            return response;          
        }

        public void Dispose() => Dispose(true);

        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _encryptor.Dispose();
                _decryptor.Dispose();
                _keyGenerator.Dispose();
                _publicKey.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }

    }
}
