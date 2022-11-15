using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using Microsoft.Research.SEAL;
using Xunit.Abstractions;

namespace FitnessTrackerTests
{
    public class CryptoValidation
    {
        private readonly ITestOutputHelper _output;

        public CryptoValidation(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(1024, 20)]
        [InlineData(4096, 20)]
        [InlineData(8192, 20)]
        [InlineData(16384, 20)]
        [InlineData(32768, 20)]
        public void ValidateRunCalculation(ulong polyModulusDegree, int numEntries)
        {
            List<EncryptedRunInfo> runList = new List<EncryptedRunInfo>();

            using var context = SEALUtils.GetContext(polyModulusDegree); 

            using var keyGenerator = new KeyGenerator(context);

            keyGenerator.CreatePublicKey(out PublicKey publicKey);                        
            
            using var encryptor = new Encryptor(context, publicKey);
            
            using var decryptor = new Decryptor(context, keyGenerator.SecretKey);

            int expectedDistance = 0;
            int expectedTime = 0;
            int distance = 0;
            int time = 0;
            Random rnd = new Random();
            for (int i = 0; i < numEntries; i++)
            {
                distance = rnd.Next(1, 10);
                time = rnd.Next(1, 10);

                _output.WriteLine($"Distance {distance}, Time: {time}");

                EncryptedRunInfo encryptedRunInfo = new EncryptedRunInfo
                {
                    Distance = GetCipherText(encryptor, distance),
                    Hours = GetCipherText(encryptor, time)
                };

                runList.Add(encryptedRunInfo);

                expectedDistance += distance;
                expectedTime += time;
            }

            _output.WriteLine($"Expected Distance {expectedDistance}, Expected Time: {expectedTime}");           

            Ciphertext totalRunsEncrypted = SEALUtils.CreateCiphertextFromInt(runList.Count(), encryptor);


            string totalRunsText = DecryptCipherText(decryptor, totalRunsEncrypted);

            Ciphertext totalDistanceEncrypted = SumEncryptedValues(context, encryptor, runList.Select(m => m.Distance));
            string totalDistanceText = DecryptCipherText(decryptor, totalDistanceEncrypted);

            Ciphertext totalTimeEncrypted = SumEncryptedValues(context, encryptor, runList.Select(m => m.Hours));
            string totalTimeText = DecryptCipherText(decryptor, totalTimeEncrypted);

            _output.WriteLine($"Total runs text: {totalRunsText}, Total Distance text: {totalDistanceText}, TotalTime text: {totalTimeText}");

            int totalDistance = int.Parse(totalDistanceText, System.Globalization.NumberStyles.HexNumber);
            int totalTime = int.Parse(totalTimeText, System.Globalization.NumberStyles.HexNumber);
            int totalRuns = int.Parse(totalRunsText);

            _output.WriteLine($"Total runs: {totalRuns}, Total Distance: {totalDistance}, TotalTime: {totalTime}");

            Assert.Equal(expectedDistance, totalDistance);
            Assert.Equal(expectedTime, totalTime);
            Assert.Equal(numEntries, totalRuns);
        }

        private Ciphertext GetCipherText(Encryptor encryptor, int value)
        {
            string stringVal = value.ToString();
            var plaintext = new Plaintext(stringVal);
            var ciphertext = new Ciphertext();
            encryptor.Encrypt(plaintext, ciphertext);
            return ciphertext;
        }

        private Ciphertext SumEncryptedValues(SEALContext context, Encryptor encryptor, IEnumerable<Ciphertext> encryptedData)
        {
            using var evaluator = new Evaluator(context);

            if (encryptedData.Any())
            {
                var encTotal = new Ciphertext();
                evaluator.AddMany(encryptedData, encTotal);
                return encTotal;
            }
            else
            {
                return SEALUtils.CreateCiphertextFromInt(0, encryptor);
            }
        }

        private string DecryptCipherText(Decryptor decryptor, Ciphertext encryptedText)
        {
            var decryptedText = new Plaintext();
            decryptor.Decrypt(encryptedText, decryptedText);
            string val = decryptedText.ToString();
            return val;
        }



    }
}
