using System.Linq;

namespace DictionaryRanker.Submissions.Base
{
    public class Dictionary : IDictionaryChecker
    {
        private readonly IHash[] _hashers = { new HashCode16Bit(), new KernighanRitchie16Bit(), new Sedgwicks16Bit() };

        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            return _hashers.Select(hasher => hasher.Hash(word)).All(dictionary.IsSet);
        }

        public void Initialize(string word, IBitStorage dictionary)
        {
            foreach (var hasher in _hashers)
            {
                var hash = hasher.Hash(word);
                dictionary.Set(hash);
            }
        }
    }

    public interface IHash
    {
        int Hash(string value);
    }

    public class KernighanRitchie : IHash
    {
        public virtual int Hash(string value)
        {
            int hash = 0;
            for (int i = 0; i < value.Length; i++)
            {
                hash = (hash * 131) + value[i];
            }
            return hash;
        }
    }

    public class KernighanRitchie16Bit : KernighanRitchie
    {
        public override int Hash(string value)
        {
            return base.Hash(value).Chop();
        }
    }

    public class Sedgwicks : IHash
    {
        public virtual int Hash(string value)
        {
            int hash = 0;
            const int b = 378551;
            int a = 63689;
            for (int i = 0; i < value.Length; i++)
            {
                hash = hash * a + value[i];
                a = a * b;
            }
            return hash;
        }
    }

    public class Sedgwicks16Bit : Sedgwicks
    {
        public override int Hash(string value)
        {
            return base.Hash(value).Chop();
        }
    }

    public class HashCode16Bit : IHash
    {
        public int Hash(string value)
        {
            return value.GetHashCode().Chop();
        }
    }

    public static class Extensions
    {
        public static int Chop(this int n)
        {
            return (int)(uint)(n & 0xFFFFFF);
        }
    }
}
