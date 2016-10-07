namespace DictionaryRanker.Submissions.camanatidis
{
    public class CrazyBloomFilterX : IDictionaryChecker
    {
        public static unsafe int GetHashCode1(string word, int l)
        {
            var x = 352654597;
            fixed (char* s = word)
            {
                var q = (int*)s;
                for (; l > 0; l -= 2)
                    x = (((x << 5) & (-2031617)) + x) ^ *q++;
            }
            return (x & 65535) + ((x >> 16) * 1566083941);
        }

        public static unsafe int GetHashCode2(string word, int l)
        {
            var x = 0L;
            fixed (char* s = word)
            {
                var p = (long*)s;
                for (; l >= 3; l -= 4) x += *p++;
                if (l > 0) x += *(int*)p;
            }
            return (int)x;
        }

        public void Initialize(string word, IBitStorage dictionary)
        {
            var l = word.Length;
            int a = GetHashCode1(word, l), b = GetHashCode2(word, l);
            dictionary.Set(a & 16777215);
            dictionary.Set((a + b) & 16777215);
            dictionary.Set((a + b + b) & 16777215);
        }

        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            int l = word.Length, a = GetHashCode1(word, l);
            if (!dictionary.IsSet(a & 16777215)) return false;
            var b = GetHashCode2(word, l);
            return
                dictionary.IsSet((a + b) & 16777215) &&
                dictionary.IsSet((a + b + b) & 16777215);
        }
    }
}