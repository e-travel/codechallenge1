﻿using System.Collections;

namespace DictionaryPuzzle
{
    public class BitStorage : IBitStorage
    {
        private const int MaxBits = 256 * 64 * 1024;

        private readonly BitArray _ba = new BitArray(MaxBits);

        public void Clear(int bitNumber)
        {
            Value(bitNumber, false);
        }

        public bool IsSet(int bitNumber)
        {
            return _ba[bitNumber];
        }

        public void Set(int bitNumber)
        {
            Value(bitNumber, true);
        }

        private void Value(int bitNumber, bool value)
        {
            _ba[bitNumber] = value;
        }
    }
}
