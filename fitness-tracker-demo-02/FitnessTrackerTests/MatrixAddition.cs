using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using static System.Formats.Asn1.AsnWriter;

namespace FitnessTrackerTests
{
    public class MatrixAddition
    {

        private readonly ITestOutputHelper _output;

        public MatrixAddition(ITestOutputHelper output)
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

            double[] podVectorTime = new double[vectorSize];
            podVectorTime[0] = 1d / 4.1d;
            podVectorTime[1] = 1d / 3.2d;
            podVectorTime[2] = 1d / 2.1d;
            podVectorTime[3] = 1d / 1.4d;
            for (int i = 4; i < vectorSize; i++)
            {
                podVectorTime[i] = 0;
            }

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

            try
            {
                decryptor.Decrypt(totalDistanceEncrypted, totalDistanceDecrypted);
            }
            catch(Exception ex)
            {
                _output.WriteLine(ex.ToString());
            }

            try
            {

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
            catch(Exception ex)
            {
                _output.WriteLine(ex.ToString());
            }


            Ciphertext encryptedTime = EncryptVector(podVectorTime, scale, encoder, encryptor, decryptor);

            _output.WriteLine($"Encrypted Time save size: {encryptedTime.SaveSize()}");

            Ciphertext multiplyResult = new Ciphertext();

            evaluator.Multiply(encryptedDistance, encryptedTime, multiplyResult);

            _output.WriteLine($"Speed Result save size: {encryptedTime.SaveSize()}");           
            
            _output.WriteLine($"Slot count: {encoder.SlotCount}");

            Plaintext decryptedMatrix = new Plaintext();

            List<double> podResult = new List<double>();
            decryptor.Decrypt(multiplyResult, decryptedMatrix);
            encoder.Decode(decryptedMatrix, podResult);

            for (int i = 0; i < 4; i++)
            {
                _output.WriteLine(podResult[i].ToString());
            }

        }

        private List<Ciphertext> EncryptList(List<double> values, double scale, CKKSEncoder encoder, Encryptor encryptor, Decryptor decryptor)
        {
            List<Ciphertext> encryptedValues = new List<Ciphertext>();

            foreach(double value in values)
            {
                Plaintext valuePlainText = new Plaintext();
                encoder.Encode(value, scale, valuePlainText);

                Ciphertext xEncrypted = new Ciphertext();
                encryptor.Encrypt(valuePlainText, xEncrypted);

                encryptedValues.Add(xEncrypted);
            }

            return encryptedValues;
        }

        [Fact]
        public void ValidateRunCalculationVectors()
        {
            using var context = SEALUtils.GetContext(8096, SchemeType.CKKS);

            using var evaluator = new Evaluator(context);

            using var keyGenerator = new KeyGenerator(context);

            keyGenerator.CreatePublicKey(out PublicKey publicKey);

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

            int vectorSize = 4096;

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

            double[] podVectorTime = new double[vectorSize];
            podVectorTime[0] = 4.1d;
            podVectorTime[1] = 3.2d;
            podVectorTime[2] = 2.1d;
            podVectorTime[3] = 1.4d;
            for (int i = 4; i < vectorSize; i++)
            {
                podVectorTime[i] = 0;
            }

            Ciphertext encryptedTime = EncryptVector(podVectorTime, scale, encoder, encryptor, decryptor);

            _output.WriteLine($"Encrypted Time save size: {encryptedTime.SaveSize()}");


            Ciphertext multiplyResult = new Ciphertext();

            evaluator.Multiply(encryptedDistance, encryptedTime, multiplyResult);

            _output.WriteLine($"Speed Result save size: {encryptedTime.SaveSize()}");


            _output.WriteLine($"Slot count: {encoder.SlotCount}");




            Plaintext decryptedMatrix = new Plaintext();

            List<double> podResult = new List<double>();
            decryptor.Decrypt(multiplyResult, decryptedMatrix);
            encoder.Decode(decryptedMatrix, podResult);

            for (int i = 0; i < 4; i++)
            {
                _output.WriteLine(podResult[i].ToString());
            }

        }


        private Ciphertext EncryptVector(double[] matrix, double scale, CKKSEncoder encoder,  Encryptor encryptor, Decryptor decryptor)
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
}
