using System.Runtime.CompilerServices;

namespace DictionaryRanker.Submissions.lpappas
{
    public class DictionaryChecker : IDictionaryChecker
    {
        private const int max = 256 * 64 * 1024;
        private const ulong fnv64Offset = 14695981039346656037;
        private const ulong fnv64Prime = 0x100000001b3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize(string word, IBitStorage dictionary)
        {
            ulong hash11 = fnv1a(word, 0);
            ulong hash22 = hash11;
            hash22 = hash22 >> 32;

            dictionary.Set((int)(hash11) & (max - 1));
            dictionary.Set((int)(hash11 + hash22) & (max - 1));
            dictionary.Set((int)(hash11 + 2 * hash22) & (max - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            ulong hash11 = fnv1a(word, 0);

            if (!dictionary.IsSet((int)(hash11) & (max - 1)))
            {
                return false;
            }

            ulong hash22 = hash11;
            hash22 = hash22 >> 32;

            // -2 ->50
            if (!dictionary.IsSet((int)(hash11 + hash22) & (max - 1)))
            {
                return false;
            }

            if (!dictionary.IsSet((int)(hash11 + 2 * hash22) & (max - 1)))
            {
                return false;
            }

            // *Might* be in the dictionary
            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong fnv1a(string bytes, ulong seed)
        {
            ulong hash = fnv64Offset ^ seed;
            fixed (char* src = bytes)
            {
                ulong c;
                char* s = src;
                while ((c = s[0]) != 0)
                {
                    hash = hash ^ c;
                    hash *= fnv64Prime;
                    s += 1;
                }
                return hash;
            }
        }
    }
}