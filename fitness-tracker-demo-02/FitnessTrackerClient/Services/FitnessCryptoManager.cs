using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public abstract class FitnessCryptoManager : IFitnessCryptoManager
    {
        private bool _disposed;
        private Encryptor _encryptor;
        private Decryptor _decryptor;
        protected SEALContext _context;
        private KeyGenerator _keyGenerator;
        protected IFitnessTrackerApiClient _apiClient;
        private PublicKey _publicKey;
        private IOptions<FitnessCryptoConfig> _config;
        private readonly ILogger _logger;

        public FitnessCryptoManager(IOptions<FitnessCryptoConfig> config, IFitnessTrackerApiClient apiClient, ILogger logger)
        {
            _config = config;
            _apiClient = apiClient;
            _logger = logger;
        }


        public virtual async Task InitializeAsync()
        {
            LogInformation("Initializing encryption");

            _context = SEALUtils.GetContext(_config.Value.PolyModulusDegree, this.SchemeType);

            _keyGenerator = new KeyGenerator(_context);

            _keyGenerator.CreatePublicKey(out PublicKey publicKey);


            // var secretKey = _keyGenerator.SecretKey;

            _publicKey = publicKey;

            LogInformation("Generating public key");
            PublicKeyModel keyModel = new PublicKeyModel();
            keyModel.PublicKey = SEALUtils.PublicKeyToBase64String(_publicKey);


            LogInformation("Sending public key to API");
            switch (this.SchemeType)
            {
                case SchemeType.BGV:
                    await _apiClient.SendPublicKeyBGVAsync(keyModel);
                    break;
                case SchemeType.CKKS:
                    await _apiClient.SendPublicKeyCKKSAsync(keyModel);
                    break;
            }
            

            _encryptor = new Encryptor(_context, _publicKey);

            _decryptor = new Decryptor(_context, _keyGenerator.SecretKey);
        }

        public abstract SchemeType SchemeType
        { get; }

        protected void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        protected string EncryptBase64(int value)
        {
            var plaintext = new Plaintext(value.ToString("X"));

            return EncryptBase64(plaintext);
        }


        protected string EncryptBase64(Plaintext value)
        {            
            var ciphertext = new Ciphertext();
            _encryptor.Encrypt(value, ciphertext);

            // Convert value to base64 string
            return SEALUtils.CiphertextToBase64String(ciphertext);
        }

        protected string DecryptBase64(string encryptedText)
        {
            var decryptedText = DecryptBase64ToPlaintext(encryptedText);
            return decryptedText.ToString();
        }

        protected Plaintext DecryptBase64ToPlaintext(string encryptedText)
        {
            var cypherText = SEALUtils.BuildCiphertextFromBase64String(encryptedText, _context);
            var decryptedText = new Plaintext();
            _decryptor.Decrypt(cypherText, decryptedText);
            return decryptedText;
        }


        public PublicKey PublicKey
        {
            get
            {
                return _publicKey;
            }
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
                _encryptor?.Dispose();
                _decryptor?.Dispose();
                _keyGenerator?.Dispose();
                _publicKey?.Dispose();
                _context?.Dispose();
            }

            _disposed = true;
        }
    }
}
