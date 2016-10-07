using System.Runtime.CompilerServices;

namespace DictionaryRanker.Submissions.igavriilidis
{
    public class BloomDictionary : IDictionaryChecker
    {
        private const int shift1 = 14;
        private const int shift2 = 26;
        private const int shift3 = 38;

        private const ulong end = 0xffffff;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Initialize(string word, IBitStorage dictionary)
        {
            ulong hash = FastHash.Hash64(word);
            dictionary.Set((int)((hash >> shift1) & end));
            dictionary.Set((int)((hash >> shift2) & end));
            dictionary.Set((int)((hash >> shift3) & end));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool IsWordPresent(string word, IBitStorage dictionary)
        {
            ulong hash = FastHash.Hash64(word);

            if (!dictionary.IsSet((int)((hash >> shift1) & end)))
            {
                return false;
            }

            if (!dictionary.IsSet((int)((hash >> shift2) & end)))
            {
                return false;
            }

            if (!dictionary.IsSet((int)((hash >> shift3) & end)))
            {
                return false;
            }

            return true;
        }
    }
}
