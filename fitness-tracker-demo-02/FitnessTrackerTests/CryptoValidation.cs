using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using Microsoft.Research.SEAL;
using Polly;
using Xunit.Abstractions;

namespace FitnessTrackerTests;

public class CryptoValidation
{
    private readonly ITestOutputHelper _output;

    public CryptoValidation(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    // [InlineData(1024, 20)] - Odd parsing error when decrypting values using this polyModulusDegree
    [InlineData(4096, 20)]
    [InlineData(8192, 20)]
    [InlineData(16384, 20)]
    [InlineData(32768, 20)]
    public void ValidateRunCalculation(ulong polyModulusDegree, ulong numEntries)
    {
        List<EncryptedRunInfoBGV> runList = new List<EncryptedRunInfoBGV>();

        using var context = SEALUtils.GetContext(polyModulusDegree, SchemeType.BGV);

        using var keyGenerator = new KeyGenerator(context);

        keyGenerator.CreatePublicKey(out PublicKey publicKey);

        using var encryptor = new Encryptor(context, publicKey);

        using var decryptor = new Decryptor(context, keyGenerator.SecretKey);

        ulong expectedDistance = 0;
        ulong expectedTime = 0;
        ulong distance = 0;
        ulong time = 0;
        Random rnd = new Random();
        for (ulong i = 0; i < numEntries; i++)
        {
            distance = (ulong)rnd.Next(1, 10);
            time = (ulong)rnd.Next(1, 10);

            _output.WriteLine($"Distance {distance}, Time: {time}");

            EncryptedRunInfoBGV encryptedRunInfo = new(GetCipherText(encryptor, distance), GetCipherText(encryptor, time));

            runList.Add(encryptedRunInfo);

            expectedDistance += distance;
            expectedTime += time;
        }

        _output.WriteLine($"Expected Distance {expectedDistance}, Expected Time: {expectedTime}");

        Ciphertext totalRunsEncrypted = SEALUtils.CreateCiphertext(runList.Count(), encryptor);

        string totalRunsText = DecryptCipherText(decryptor, totalRunsEncrypted);
        ulong totalRuns = ulong.Parse(totalRunsText, System.Globalization.NumberStyles.HexNumber);


        Ciphertext totalDistanceEncrypted = SumEncryptedValues(context, encryptor, runList.Select(m => m.Distance));
        _output.WriteLine($"totalDistanceEncrypted Noise Budget: {decryptor.InvariantNoiseBudget(totalDistanceEncrypted)}");
        string totalDistanceText = DecryptCipherText(decryptor, totalDistanceEncrypted);

        Ciphertext totalTimeEncrypted = SumEncryptedValues(context, encryptor, runList.Select(m => m.Hours));
        string totalTimeText = DecryptCipherText(decryptor, totalTimeEncrypted);

        _output.WriteLine($"Total runs text: {totalRunsText}, Total Distance text: {totalDistanceText}, TotalTime text: {totalTimeText}");

        ulong totalDistance = ulong.Parse(totalDistanceText, System.Globalization.NumberStyles.HexNumber);
        ulong totalTime = ulong.Parse(totalTimeText, System.Globalization.NumberStyles.HexNumber);

        _output.WriteLine($"Total runs: {totalRuns}, Total Distance: {totalDistance}, TotalTime: {totalTime}");

        Assert.Equal(expectedDistance, totalDistance);
        Assert.Equal(expectedTime, totalTime);
        Assert.Equal(numEntries, totalRuns);
    }

    private Ciphertext GetCipherText(Encryptor encryptor, ulong value)
    {
        var plaintext = value.ToPlainText();
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
            return SEALUtils.CreateCiphertext(0, encryptor);
        }
    }

    private string DecryptCipherText(Decryptor decryptor, Ciphertext encryptedText)
    {
        var decryptedText = new Plaintext();
        decryptor.Decrypt(encryptedText, decryptedText);
        string val = decryptedText.ToString();
        return val;
    }

    [Fact]
    public void ValidateBase64Encryption()
    {

        using var context = SEALUtils.GetContext(8196, SchemeType.CKKS);

        using var keyGenerator = new KeyGenerator(context);

        keyGenerator.CreatePublicKey(out PublicKey publicKey);

        using var encryptor = new Encryptor(context, publicKey);

        using var decryptor = new Decryptor(context, keyGenerator.SecretKey);

        using var encoder = new CKKSEncoder(context);

        List<double> origValue = new List<double> { 50d };

        double scale = Math.Pow(2.0, 40);

        Plaintext encodedValue = new Plaintext();
            
        encoder.Encode(origValue, scale, encodedValue);

        Ciphertext encrypted = new Ciphertext();

        encryptor.Encrypt(encodedValue, encrypted);


        string cipherText = SEALUtils.CiphertextToBase64String(encrypted);

        Ciphertext decodedCipherText = SEALUtils.BuildCiphertextFromBase64String(cipherText, context);

        Plaintext decryptedPlain = new Plaintext();

        decryptor.Decrypt(encrypted, decryptedPlain);


        List<double> result = new List<double>();

        encoder.Decode(decryptedPlain, result);

        _output.WriteLine(result[0].ToString());


    }
}
