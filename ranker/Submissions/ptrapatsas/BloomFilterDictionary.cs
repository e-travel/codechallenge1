namespace DictionaryRanker.Submissions.ptrapatsas
{
    public class BloomFilterDictionary : IDictionaryChecker
    {
        public void Initialize(string item, IBitStorage m)
        {
            int h1 = item.GetHashCode();
            int h2 = GetMyHashCode(item);

            for (int i = 0; i < 3; i++)
            {
                int hash = Ch(h1, h2, i);
                m.Set(hash);
            }
        }

        public bool IsWordPresent(string item, IBitStorage m)
        {
            int h1 = item.GetHashCode();
            int h2 = GetMyHashCode(item);

            for (int i = 0; i < 3; i++)
            {
                int hash = Ch(h1, h2, i);
                if (!m.IsSet(hash)) return false;
            }
            return true;
        }

        private static int Ch(int h1, int h2, int i)
        {
            int rh = (h1 + (i * h2)) % 16777216;
            return rh < 0 ? -rh : rh;
        }

        public static int GetMyHashCode(string w)
        {
            unsafe
            {
                fixed (char* chPtr = w)
                {
                    int num1 = 352654597;
                    int num2 = num1;
                    int* numPtr = (int*)chPtr;
                    int length = w.Length;
                    while (length > 2)
                    {
                        num1 = (num1 << 5) + num1 + (num1 >> 27) ^ *numPtr;
                        num2 = (num2 << 5) + num2 + (num2 >> 27) ^ numPtr[1];
                        numPtr += 2;
                        length -= 4;
                    }
                    if (length > 0)
                        num1 = (num1 << 5) + num1 + (num1 >> 27) ^ *numPtr;
                    return num1 + num2 * 1566083941;
                }
            }
        }
    }
}
