namespace DictionaryRanker.Submissions.kkentzoglanakis
{
    public class DummyDictionary : IDictionaryChecker
    {
        public class HashFunctions
        {

            public unsafe static int sdbm(string message)
            {
                ulong h = 0;
                int length = message.Length;
                fixed (char* buffer = message)
                {
                    char* c = buffer;
                    for (int i = 0; i < length; ++c, ++i)
                    {
                        h = *c + (h << 6) + (h << 16) - h;
                    }
                }
                return (int)(h & 0xFFFFFF);
            }

            public unsafe static int murmur(string message)
            {
                const uint seed = 0xc58f1a7b;
                const uint m = 0x5bd1e995;
                const int r = 24;
                int length = message.Length;
                if (length == 0)
                    return 0;
                uint h = seed ^ (uint)length;

                fixed (char* buffer = message)
                {
                    char* c = buffer;
                    while (length >= 4)
                    {
                        uint k = (uint)(*c++ | *c++ << 8 | *c++ << 16 | *c++ << 24);
                        k *= m;
                        k ^= k >> r;
                        k *= m;

                        h *= m;
                        h ^= k;
                        length -= 4;
                    }
                    switch (length)
                    {
                        case 3:
                            h ^= (uint)(*c++ | *c++ << 8);
                            h ^= (uint)(*c << 16);
                            h *= m;
                            break;
                        case 2:
                            h ^= (uint)(*c++ | *c << 8);
                            h *= m;
                            break;
                        case 1:
                            h ^= *c;
                            h *= m;
                            break;
                        default:
                            break;
                    }
                    h ^= h >> 13;
                    h *= m;
                    h ^= h >> 15;
                }
                return (int)(h & 0xFFFFFF);
            }
        }

        public void Initialize(string word, IBitStorage dictionary)
        {
            int hv1 = HashFunctions.murmur(word);
            int hv2 = HashFunctions.sdbm(word);
            dictionary.Set(hv1);
            dictionary.Set(hv2);
            dictionary.Set((hv1 + hv2) & 0xFFFFFF);
        }

        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            int hv1 = HashFunctions.murmur(word);
            int hv2 = HashFunctions.sdbm(word);
            return dictionary.IsSet(hv1) &&
                dictionary.IsSet(hv2) &&
                dictionary.IsSet((hv1 + hv2) & 0xFFFFFF);
        }
    }
}