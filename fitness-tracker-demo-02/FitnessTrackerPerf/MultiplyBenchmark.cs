using BenchmarkDotNet.Attributes;
using FitnessTracker.Common.Utils;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerPerf;

//[Config(typeof(AntiVirusFriendlyConfig))]
[RPlotExporter]
public class MultiplyBenchmark
{
    const ulong unencryptedValue = 100;

    const ulong multiplyBy = 2;

    private SEALContext _context;
    private KeyGenerator _keyGenerator;
    private PublicKey _publicKey;
    private Encryptor _encryptor;
    
    private Decryptor _decryptor;

    private Ciphertext _encryptedValue;

    private Ciphertext _multiplyEncryptedValue;

    private Evaluator _evaluator;

    private RelinKeys _relinKeys;

    [Params(SchemeType.BFV, SchemeType.BGV, SchemeType.CKKS)]
    public SchemeType SchemeType;

    [Params(16384)]
    public ulong PolyModulusDegree;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _context = SEALUtils.GetContext(PolyModulusDegree, SchemeType);
        _keyGenerator = new KeyGenerator(_context);
        _keyGenerator.CreatePublicKey(out _publicKey);

        _keyGenerator.CreateRelinKeys(out _relinKeys);
        _encryptor = new Encryptor(_context, _publicKey);

        _decryptor = new Decryptor(_context, _keyGenerator.SecretKey);

        Plaintext valueText = unencryptedValue.ToPlainText();

        _encryptedValue = new Ciphertext();
        _encryptor.Encrypt(valueText, _encryptedValue);

        Plaintext multiplyText = multiplyBy.ToPlainText();

        _multiplyEncryptedValue = new Ciphertext();

        _encryptor.Encrypt(multiplyText, _multiplyEncryptedValue);


        _evaluator = new Evaluator(_context);
    }

    [Benchmark]
    public void Multiply()
    {
        var ciphertext = new Ciphertext();
        _evaluator.Multiply(_encryptedValue, _multiplyEncryptedValue, ciphertext);
    }

    [Benchmark]
    public void Addition()
    {
        var ciphertext = new Ciphertext();
        _evaluator.Add(_encryptedValue, _multiplyEncryptedValue, ciphertext);
    }

    [Benchmark]
    public void Exponentiation()
    {
        var ciphertext = new Ciphertext();
        _evaluator.Exponentiate(_encryptedValue, 2, _relinKeys, ciphertext);
    }


    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _decryptor.Dispose();
        _encryptor.Dispose();
        _publicKey.Dispose();
        _keyGenerator.Dispose();
        _context.Dispose();
    }



}
