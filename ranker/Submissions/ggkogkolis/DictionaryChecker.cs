using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DictionaryRanker.Submissions.ggkogkolis
{
    public class DictionaryChecker : IDictionaryChecker
    {
        private const int Max = 256 * 64 * 1024;
        private const uint M = 0x5bd1e995;
        private const int R = 24;

        public void Initialize(string word, IBitStorage dictionary)
        {

            var address = ComputeAddress(word, Hash);
            dictionary.Set(address);
            address = ComputeAddress(word, Hash2);
            dictionary.Set(address);

        }

        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            var address = ComputeAddress(word, Hash);
            var isSet1 = dictionary.IsSet(address);
            address = ComputeAddress(word, Hash2);
            return dictionary.IsSet(address) && isSet1;
        }

        public static int ComputeAddress(string word, Func<byte[], uint> hashFunction)
        {
            var hash = unchecked((int)hashFunction(Encoding.UTF8.GetBytes(word)));
            return Math.Abs(hash % Max);
        }

        protected internal static uint Hash2(byte[] data)
        {
            const uint seed = 31;

            var length = data.Length;
            if (length == 0)
                return 0;
            var h = seed ^ (uint)length;
            var currentIndex = 0;
            while (length >= 4)
            {
                var k =
                    (uint)
                        (data[currentIndex++] | data[currentIndex++] << 8 | data[currentIndex++] << 16 |
                         data[currentIndex++] << 24);
                k *= M;
                k ^= k >> R;
                k *= M;

                h *= M;
                h ^= k;
                length -= 4;
            }
            switch (length)
            {
                case 3:
                    h ^= (ushort)(data[currentIndex++] | data[currentIndex++] << 8);
                    h ^= (uint)(data[currentIndex] << 16);
                    h *= M;
                    break;
                case 2:
                    h ^= (ushort)(data[currentIndex++] | data[currentIndex] << 8);
                    h *= M;
                    break;
                case 1:
                    h ^= data[currentIndex];
                    h *= M;
                    break;
                default:
                    break;
            }

            // Do a few final mixes of the hash to ensure the last few
            // bytes are well-incorporated.

            h ^= h >> 13;
            h *= M;
            h ^= h >> 15;

            return h;
        }

        protected internal static uint Hash(byte[] dataToHash)
        {
            var dataLength = dataToHash.Length;
            if (dataLength == 0)
                return 0;
            var hash = (uint)dataLength;
            var remainingBytes = dataLength & 3; // mod 4
            var numberOfLoops = dataLength >> 2; // div 4
            var currentIndex = 0;
            var arrayHack = new BytetoUInt16Converter { Bytes = dataToHash }.UInts;
            while (numberOfLoops > 0)
            {
                hash += arrayHack[currentIndex++];
                var tmp = (uint)(arrayHack[currentIndex++] << 11) ^ hash;
                hash = (hash << 16) ^ tmp;
                hash += hash >> 11;
                numberOfLoops--;
            }
            currentIndex *= 2; // fix the length
            switch (remainingBytes)
            {
                case 3:
                    hash += (ushort)(dataToHash[currentIndex++] | dataToHash[currentIndex++] << 8);
                    hash ^= hash << 16;
                    hash ^= (uint)dataToHash[currentIndex] << 18;
                    hash += hash >> 11;
                    break;
                case 2:
                    hash += (ushort)(dataToHash[currentIndex++] | dataToHash[currentIndex] << 8);
                    hash ^= hash << 11;
                    hash += hash >> 17;
                    break;
                case 1:
                    hash += dataToHash[currentIndex];
                    hash ^= hash << 10;
                    hash += hash >> 1;
                    break;
                default:
                    break;
            }

            /* Force "avalanching" of final 127 bits */
            hash ^= hash << 3;
            hash += hash >> 5;
            hash ^= hash << 4;
            hash += hash >> 17;
            hash ^= hash << 25;
            hash += hash >> 6;

            return hash;
        }

        [StructLayout(LayoutKind.Explicit)]
        // no guarantee this will remain working
        private struct BytetoUInt16Converter
        {
            [FieldOffset(0)]
            public byte[] Bytes;

            [FieldOffset(0)]
            public readonly ushort[] UInts;
        }
    }
}