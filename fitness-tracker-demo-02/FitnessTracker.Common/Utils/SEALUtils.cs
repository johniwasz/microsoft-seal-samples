using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Microsoft.Research.SEAL;

namespace FitnessTracker.Common.Utils
{
    public class SEALUtils
    {
        public const ulong DEFAULTPOLYMODULUSDEGREE = 4096;

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

            using var ms = new MemoryStream(payload);
            var ciphertext = new Ciphertext();
            ciphertext.Load(context, ms);

            return ciphertext;
        }


        public static Ciphertext CreateCiphertext(int value, Encryptor encryptor)
        {
            using Plaintext plaintext = value.ToPlainText();
            var ciphertext = new Ciphertext();
            encryptor.Encrypt(plaintext, ciphertext);
            return ciphertext;
        }

        public static Ciphertext CreateCiphertext(double value, Encryptor encryptor)
        {
            using Plaintext plaintext = value.ToPlainText();
            var ciphertext = new Ciphertext();
            encryptor.Encrypt(plaintext, ciphertext);
            return ciphertext;
        }

        public static Ciphertext CreateCiphertext(ulong value, Encryptor encryptor)
        {
            using Plaintext plaintext = value.ToPlainText();
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

        public static long GetByteLength(string base64EncodedData)
        {
            return System.Text.ASCIIEncoding.UTF8.GetByteCount(base64EncodedData);
        }
        /*
        public static SEALContext GetContext()
        {         
            return GetContext(DEFAULTPOLYMODULUSDEGREE, SchemeType.BFV);
        }

        public static SEALContext GetContext(ulong polyModulusDegree)
        {
            return GetContext(polyModulusDegree, SchemeType.BFV);
        }
        */



        public static SEALContext GetContext(ulong polyModulusDegree, SchemeType scheme)
        {
            /*
            The first parameter we set is the degree of the `polynomial modulus'. This
            must be a positive power of 2, representing the degree of a power-of-two
            cyclotomic polynomial; it is not necessary to understand what this means.

            Larger PolyModulusDegree makes ciphertext sizes larger and all operations
            slower, but enables more complicated encrypted computations. Recommended
            values are 1024, 2048, 4096, 8192, 16384, 32768, but it is also possible
            to go beyond this range.

            In this example we use a relatively small polynomial modulus. Anything
            smaller than this will enable only very restricted encrypted computations.
            */

            EncryptionParameters encParams = default(EncryptionParameters);

            switch(scheme)
            {
                case SchemeType.BFV:
                case SchemeType.BGV:
                    encParams = GetWholeNumberEncryptionParamters(polyModulusDegree, scheme);
                    break;
                case SchemeType.CKKS:
                    encParams = GetRealNumberEncryptionParamters();
                    break;
                default:
                    throw new ArgumentException($"Invalid Scheme {scheme}");

            }

            return new SEALContext(encParams);
        }

        private static EncryptionParameters GetRealNumberEncryptionParamters()
        {
            EncryptionParameters encryptionParameters = new EncryptionParameters(SchemeType.CKKS);

            ulong polyModulusDegree = 8192;
            encryptionParameters.PolyModulusDegree = polyModulusDegree;
            encryptionParameters.CoeffModulus = CoeffModulus.Create(
                polyModulusDegree, new int[] { 60, 40, 40, 60 });

            return encryptionParameters;
        }

        private static EncryptionParameters GetWholeNumberEncryptionParamters(ulong polyModulusDegree, SchemeType scheme)
        {
            var encryptionParameters = new EncryptionParameters(scheme)
            {
                PolyModulusDegree = polyModulusDegree,

                /*
               Next we set the [ciphertext] `coefficient modulus' (CoeffModulus). This
               parameter is a large integer, which is a product of distinct prime numbers,
               numbers, each represented by an instance of the Modulus class. The
               bit-length of CoeffModulus means the sum of the bit-lengths of its prime
               factors.

               A larger CoeffModulus implies a larger noise budget, hence more encrypted
               computation capabilities. However, an upper bound for the total bit-length
               of the CoeffModulus is determined by the PolyModulusDegree, as follows:

                   +----------------------------------------------------+
                   | PolyModulusDegree   | max CoeffModulus bit-length  |
                   +---------------------+------------------------------+
                   | 1024                | 27                           |
                   | 2048                | 54                           |
                   | 4096                | 109                          |
                   | 8192                | 218                          |
                   | 16384               | 438                          |
                   | 32768               | 881                          |
                   +---------------------+------------------------------+

               These numbers can also be found in native/src/seal/util/hestdparms.h encoded
               in the function SEAL_HE_STD_PARMS_128_TC, and can also be obtained from the
               function

                   CoeffModulus.MaxBitCount(polyModulusDegree).

               For example, if PolyModulusDegree is 4096, the coeff_modulus could consist
               of three 36-bit primes (108 bits).

               Microsoft SEAL comes with helper functions for selecting the CoeffModulus.
               For new users the easiest way is to simply use

                   CoeffModulus.BFVDefault(polyModulusDegree),

               which returns IEnumerable<Modulus> consisting of a generally good choice
               for the given PolyModulusDegree.
               */

                CoeffModulus = CoeffModulus.BFVDefault(polyModulusDegree),

                /*
                To enable batching, we need to set the plain_modulus to be a prime number
                congruent to 1 modulo 2*PolyModulusDegree. Microsoft SEAL provides a helper
                method for finding such a prime. In this example we create a 20-bit prime
                that supports batching.
                */
                PlainModulus = PlainModulus.Batching(polyModulusDegree, 20)
            };

            return encryptionParameters;

        }
    }
}
