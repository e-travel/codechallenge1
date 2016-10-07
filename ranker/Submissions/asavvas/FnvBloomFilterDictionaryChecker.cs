using System;
using System.Runtime.CompilerServices;

namespace DictionaryRanker.Submissions.asavvas
{
    public sealed class OneTimeBloomFilterDictionaryChecker : IDictionaryChecker
    {
        /// <summary>
        /// number of bits in the storage
        /// </summary>
        private const int m = 256 * 64 * 1024;

        /// <summary>
        /// number of hash functions
        /// </summary>
        public const byte k = 3;

        public byte K { get { return k; } }

        /// <summary>
        /// Performs Dillinger and Manolios double hashing. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ΤουΜανώλη(int primaryHash, int secondaryHash, byte i)
        {
            return Math.Abs((primaryHash + (i * secondaryHash)) % m);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            int primaryHash = word.GetHashCode();
            int secondaryHash;

            return dictionary.IsSet(Math.Abs(primaryHash % m)) &&
                   dictionary.IsSet(Math.Abs(OneAtATimeHash(word, out secondaryHash) % m)) &&
                   dictionary.IsSet(ΤουΜανώλη(primaryHash, secondaryHash, 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize(string word, IBitStorage dictionary)
        {
            int primaryHash = word.GetHashCode();
            dictionary.Set(Math.Abs(word.GetHashCode() % m));
            int secondaryHash;
            dictionary.Set(Math.Abs(OneAtATimeHash(word, out secondaryHash) % m));
            dictionary.Set(ΤουΜανώλη(primaryHash, secondaryHash, 2));
        }

        /// <summary>
        /// Hashes a string using Bob Jenkin's "One At A Time" method from Dr. Dobbs (http://burtleburtle.net/bob/hash/doobs.html).
        /// Runtime is suggested to be 9x+9, where x = input.Length. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int OneAtATimeHash(string input, out int hash)
        {
            hash = input[0];

            for (int i = 1; i < input.Length; ++i)
            {
                hash += input[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }
    }
}
