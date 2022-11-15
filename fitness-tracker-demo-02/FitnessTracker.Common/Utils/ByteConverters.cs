using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTracker.Common.Utils
{
    public static class ByteConverters
    {

        public static string ToByteString(this int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return GetByteString(bytes);
        }

        /// <summary>
        /// Helper function: Convert a ulong to a hex string representation
        /// </summary>
        public static string ToByteString(this ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return GetByteString(bytes);
        }

        public static string ToByteString(this double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return GetByteString(bytes);
        }

        public static Plaintext ToPlainText(this int value)
        {
            string byteString = value.ToByteString();

            return new Plaintext(byteString);
        }

        public static Plaintext ToPlainText(this ulong value)
        {
            string byteString = value.ToByteString();

            return new Plaintext(byteString);
        }

        public static Plaintext ToPlainText(this double value)
        {
            string byteString = value.ToByteString();

            return new Plaintext(byteString);
        }

        private static string GetByteString(byte[] byteRepresentation)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteRepresentation);
            }
            return BitConverter.ToString(byteRepresentation).Replace("-", "");

        }

    }
}
