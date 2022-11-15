using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using FitnessTracker.Common.Utils;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerPerf
{
    //[Config(typeof(AntiVirusFriendlyConfig))]
    [RPlotExporter]
    public class EncryptBenchmark
    {
        const ulong unencryptedValue = 100;

        private SEALContext _context;
        private KeyGenerator _keyGenerator;
        private PublicKey _publicKey;
        private Encryptor _encryptor;
        private Plaintext _valueText;
        private Decryptor _decryptor;

        private Ciphertext _encryptedValue;

        [Params(SchemeType.BFV, SchemeType.BGV, SchemeType.CKKS)]
        public SchemeType SchemeType;

        [Params(1024, 4096, 8192, 16384, 32768)]
        public ulong PolyModulusDegree;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _context = SEALUtils.GetContext(PolyModulusDegree, SchemeType);
            _keyGenerator = new KeyGenerator(_context);
            _keyGenerator.CreatePublicKey(out _publicKey);
            _encryptor = new Encryptor(_context, _publicKey);

            _decryptor = new Decryptor(_context, _keyGenerator.SecretKey);
            
            _valueText = unencryptedValue.ToPlainText();

            _encryptedValue = new Ciphertext();
            _encryptor.Encrypt(_valueText, _encryptedValue);
        }

        [Benchmark]
        public void EncryptCipherText()
        {            
            var ciphertext = new Ciphertext();
            _encryptor.Encrypt(_valueText, ciphertext);
        }

        [Benchmark]
        public void DecryptCipherText()
        {
            var decryptedText = new Plaintext();
            _decryptor.Decrypt(_encryptedValue, decryptedText);
            string val = decryptedText.ToString();
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
}
