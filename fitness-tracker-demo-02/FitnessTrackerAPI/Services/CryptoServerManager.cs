using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessTrackerAPI.Services;

public abstract class CryptoServerManager : IDisposable, ICryptoServerManager
{
    protected readonly SEALContext _sealContext;
    private bool _disposed;
    protected Evaluator _evaluator;
    protected Encryptor _encryptor;

    private Microsoft.Research.SEAL.PublicKey _publicKey;
    protected ILogger<CryptoServerManager> _logger;

    public CryptoServerManager(IOptions<FitnessCryptoConfig> config, ILogger<CryptoServerManager> logger)
    {
        // Initialize context
        _sealContext = SEALUtils.GetContext(config.Value.PolyModulusDegree, this.SchemeType);
        _evaluator = new Evaluator(_sealContext);
        _logger = logger;
    }


    public abstract SchemeType SchemeType
    {
        get;
    }

    public void SetPublicKey(string publicKeyEncoded)
    {
        _logger?.LogDebug("[API]: SetPublicKey - set public key from client");

        _publicKey = SEALUtils.BuildPublicKeyFromBase64String(publicKeyEncoded, _sealContext);
        _encryptor = new Encryptor(_sealContext, _publicKey);
    }


    protected Ciphertext SumEncryptedValues(IEnumerable<Ciphertext> encryptedData)
    {
        if (encryptedData.Any())
        {
            Ciphertext encTotal = new();
            _evaluator.AddMany(encryptedData, encTotal);
            return encTotal;
        }
        else
        {
            return SEALUtils.CreateCiphertext(0, _encryptor);
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
            _publicKey?.Dispose();
            _encryptor?.Dispose();
            _evaluator.Dispose();
            _sealContext.Dispose();
        }

        _disposed = true;
    }
}

