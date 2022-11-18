using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace FitnessTrackerTests;

public class VectorAddition
{

    private readonly ITestOutputHelper _output;

    public VectorAddition(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ValidateRunCalculation()
    {
        List<EncryptedRunInfoBGV> runList = new List<EncryptedRunInfoBGV>();

        using var context = SEALUtils.GetContext(8096, SchemeType.CKKS);

        using var evaluator = new Evaluator(context);

        using var keyGenerator = new KeyGenerator(context);

        keyGenerator.CreatePublicKey(out PublicKey publicKey);

        keyGenerator.CreateGaloisKeys(out GaloisKeys galoisKeys);

        using var encryptor = new Encryptor(context, publicKey);

        using var decryptor = new Decryptor(context, keyGenerator.SecretKey);

        using CKKSEncoder encoder = new CKKSEncoder(context);

        /*
        We choose the initial scale to be 2^40. At the last level, this leaves us
        60-40=20 bits of precision before the decimal point, and enough (roughly
        10-20 bits) of precision after the decimal point. Since our intermediate
        primes are 40 bits (in fact, they are very close to 2^40), we can achieve
        scale stabilization as described above.
        */
        double scale = Math.Pow(2.0, 40);

        // int vectorSize = 4096;
        int vectorSize = 4;

        double[] podVectorDistance = new double[vectorSize];
        podVectorDistance[0] = 1.1d;
        podVectorDistance[1] = 2.1d;
        podVectorDistance[2] = 3.2d;
        podVectorDistance[3] = 4.2d;
        for (int i = 4; i < vectorSize; i++)
        {
            podVectorDistance[i] = 0;
        }

        Ciphertext encryptedDistance = EncryptVector(podVectorDistance, scale, encoder, encryptor, decryptor);

        _output.WriteLine($"Encrypted Distace save size: {encryptedDistance.SaveSize()}");

        List<Ciphertext> rotationOutput = new List<Ciphertext>(4);

        for (int steps = 0; steps < vectorSize; steps++)
        {
            Ciphertext rotated = new Ciphertext();
            evaluator.RotateVector(encryptedDistance, steps, galoisKeys, rotated);
            rotationOutput.Add(rotated);
        }

        Ciphertext totalDistanceEncrypted = new Ciphertext();

        evaluator.AddMany(rotationOutput, totalDistanceEncrypted);

        Plaintext totalDistanceDecrypted = new Plaintext();

         decryptor.Decrypt(totalDistanceEncrypted, totalDistanceDecrypted);

        if (totalDistanceDecrypted != null)
        {
            List<double> addedVector = new List<double>();

            encoder.Decode(totalDistanceDecrypted, addedVector);

            _output.WriteLine($"Total Distance: {addedVector[0]}");
        }
        else
        {
            _output.WriteLine($"Total Distance decryption silently failed");
        }

    }

    private Ciphertext EncryptVector(double[] matrix, double scale, CKKSEncoder encoder, Encryptor encryptor, Decryptor decryptor)
    {
        _output.WriteLine("Input plaintext matrix:");

        using Plaintext distancePlain = new Plaintext();
        _output.WriteLine("Encode plaintext matrix to xPlain:");

        List<Double> matrixList = matrix.ToList();

        encoder.Encode(matrixList, scale, distancePlain);

        Ciphertext xEncrypted = new Ciphertext();
        _output.WriteLine(String.Empty);
        _output.WriteLine("Encrypt xPlain to xEncrypted.");
        encryptor.Encrypt(distancePlain, xEncrypted);
        _output.WriteLine(String.Empty);

        return xEncrypted;
    }
}
