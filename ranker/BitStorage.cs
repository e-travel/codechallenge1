using System;
using System.Collections;

namespace DictionaryRanker
{
    public class BitStorage : IBitStorage
    {
        public const int MaxBits = 256 * 64 * 1024;

        private readonly BitArray _ba = new BitArray(MaxBits);

        public void Clear(int bitNumber)
        {
            Value(bitNumber, false);
        }

        public bool IsSet(int bitNumber)
        {
            //CheckBit(bitNumber);
            return _ba[bitNumber];
        }

        public void Set(int bitNumber)
        {
            Value(bitNumber, true);
        }

        private void Value(int bitNumber, bool value)
        {
            //CheckBit(bitNumber);
            _ba[bitNumber] = value;
        }

        public int Length()
        {
            return _ba.Length;
        }

        private void CheckBit(int bitNumber)
        {
            if (bitNumber < 0 || bitNumber >= MaxBits)
            {
                throw new InvalidOperationException(string.Format("Invalid value {0} for bit - can be between 0 and {1}", bitNumber, MaxBits-1));
            }
        }
    }
}
