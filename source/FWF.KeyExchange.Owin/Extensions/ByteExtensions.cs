using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace FWF.KeyExchange.Owin
{
    public static class ByteExtensions
    {
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool IsEqualByte(this byte[] original, byte[] compare)
        {
            // Same instance
            if (ReferenceEquals(original, compare))
            {
                return true;
            }

            // Both null
            if (original == null && compare == null)
            {
                return true;
            }

            // Only one is null
            if (original == null)
            {
                return false;
            }
            if (compare == null)
            {
                return false;
            }

            // Different length
            if (original.Length != compare.Length)
            {
                return false;
            }

            return !original.Where(
                (t, index) => t != compare[index]
                ).Any();
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static string ToHex(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var hex = new char[data.Length * 2];

            for (int iter = 0; iter < data.Length; iter++)
            {
                var hexChar = ((byte)(data[iter] >> 4));
                hex[iter * 2] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
                hexChar = ((byte)(data[iter] & 0xF));
                hex[(iter * 2) + 1] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
            }

            return new string(hex);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static byte[] Sha256(this byte[] input)
        {
            if (input == null)
            {
                return null;
            }
            byte[] result;
            using (SHA256 sha = SHA256.Create())
            {
                result = sha.ComputeHash(input);
            }
            return result;
        }


    }
}
