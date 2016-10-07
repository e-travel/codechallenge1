namespace DictionaryPuzzle
{
    /// <summary>
    /// An (obviously lame) dictionary implementation.
    /// </summary>
    public class DummyDictionary : IDictionaryChecker
    {
        public void Initialize(string word, IBitStorage dictionary)
        {
            dictionary.Set(1);
        }

        public bool IsWordPresent(string word, IBitStorage dictionary)
        {
            return dictionary.IsSet(1);
        }
    }
}
