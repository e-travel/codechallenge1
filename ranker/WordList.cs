using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DictionaryRanker
{
    public class WordList
    {
        private static List<string> _loadedList;
        private readonly SortedList<string, bool> _lst = new SortedList<string, bool>();
        private readonly Random _rnd;

        public WordList()
        {
            _rnd = new Random(19701971);
        }

        public void LoadWordList()
        {
            if (_loadedList == null)
            {
                _loadedList = File.ReadAllLines("..\\..\\words.txt").ToList();
            }

            foreach (var item in _loadedList)
            {
                _lst.Add(item, true);
            }
        }

        public string GetRandomWord()
        {
            var random = _rnd.Next(0, NumberOfWordsLoaded() - 1);
            return _lst.Keys[random];
        }

        public int NumberOfWordsLoaded()
        {
            return _lst.Count;
        }

        public void Remove(string value)
        {
            _lst.Remove(value);
        }

        public List<string> GetList()
        {
            return _lst.Keys.ToList();
        }
    }
}
