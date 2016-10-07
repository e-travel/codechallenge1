namespace DictionaryRanker.Submissions.ikaradimas
{
    interface IHash
    {
        void Set(string bytes, IBitStorage dictionary);
        bool IsSet(string word, IBitStorage dictionary);
    }

    public class BloomDictionary : IDictionaryChecker
    {
        private const int _prime1 = 3461;
        private const int _prime2 = 5381;
        private const int _prime3 = 7549;
        private const int _prime4 = 3461;
        private const int ModuloMask = (2097152 * 8) - 1;

        public void Initialize(string word, IBitStorage dictionary)
        {
            int length = word.Length,
                remaining = length & 1,
                evenLength = length ^ remaining,
                index = 0,
                hash1 = _prime1,
                hash2 = _prime2;

            while (index < evenLength)
            {
                hash1 = ((hash1 << 5) + hash1) ^ word[index++];
                hash2 = ((hash2 << 7) - hash2) ^ word[index++];
            }

            if (remaining == 1)
                hash1 = ((hash1 << 5) + hash1) ^ word[index];

            hash1 = (hash1 ^ (hash2 * _prime3)) & ModuloMask;
            hash2 = (hash2 ^ (hash1 * _prime4)) & ModuloMask;

            dictionary.Set(hash1);
            dictionary.Set(hash2);
            dictionary.Set(((hash1 << 9) | (hash2 >> 15)) & ModuloMask);
        }

        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            int length = word.Length,
                remaining = length & 1,
                evenLength = length ^ remaining,
                index = 0,
                hash1 = _prime1,
                hash2 = _prime2;

            while (index < evenLength)
            {
                hash1 = ((hash1 << 5) + hash1) ^ word[index++];
                hash2 = ((hash2 << 7) - hash2) ^ word[index++];
            }

            if (remaining == 1)
                hash1 = ((hash1 << 5) + hash1) ^ word[index];

            hash1 = (hash1 ^ (hash2 * _prime3)) & ModuloMask;
            hash2 = (hash2 ^ (hash1 * _prime4)) & ModuloMask;

            return
                dictionary.IsSet(hash1)
                && dictionary.IsSet(hash2)
                && dictionary.IsSet(((hash1 << 9) | (hash2 >> 15)) & ModuloMask);
        }
    }
}