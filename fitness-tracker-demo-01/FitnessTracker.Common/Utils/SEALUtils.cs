using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Research.SEAL;

namespace FitnessTracker.Common.Utils
{
    public class SEALUtils
    {
        public static string CiphertextToBase64String(Ciphertext ciphertext)
        {
            using (var ms = new MemoryStream())
            {
                ciphertext.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string DoubleToBase64String(double value)
        {
            using (var ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(value.ToString())))
            {
                return Convert.ToBase64String(ms.ToArray());
            }
        }


        public static Ciphertext BuildCiphertextFromBase64String(string base64, SEALContext context)
        {
            var payload = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(payload))
            {
                var ciphertext = new Ciphertext();
                ciphertext.Load(context, ms);

                return ciphertext;
            }
        }

        public static Ciphertext CreateCiphertextFromInt(double value, Encryptor encryptor)
        {
            var plaintext = new Plaintext($"{value}");
            var ciphertext = new Ciphertext();
            encryptor.Encrypt(plaintext, ciphertext);
            return ciphertext;
        }

        public static PublicKey BuildPublicKeyFromBase64String(string base64, SEALContext context)
        {
            var payload = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(payload))
            {
                var publicKey = new PublicKey();
                publicKey.Load(context, ms);

                return publicKey;
            }
        }

        public static SecretKey BuildSecretKeyFromBase64String(string base64, SEALContext context)
        {
            var payload = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(payload))
            {
                var secretKey = new SecretKey();
                secretKey.Load(context, ms);

                return secretKey;
            }
        }

        public static string SecretKeyToBase64String(SecretKey secretKey)
        {
            using (var ms = new MemoryStream())
            {
                secretKey.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string PublicKeyToBase64String(PublicKey publicKey)
        {
            using (var ms = new MemoryStream())
            {
                publicKey.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static SEALContext GetContext()
        {
            ulong modulusDegree = 4096;
            var encryptionParameters = new EncryptionParameters(SchemeType.BFV)
            {
//                PolyModulusDegree = 32768,
                PolyModulusDegree = modulusDegree,
                CoeffModulus = CoeffModulus.BFVDefault(polyModulusDegree: modulusDegree)
            };

            encryptionParameters.SetPlainModulus(0x133Ful);

            Debug.WriteLine("[COMMON]: Successfully created context");

            return new SEALContext(encryptionParameters);
        }
    }
}
