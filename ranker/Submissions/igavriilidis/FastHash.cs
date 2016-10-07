using System.Runtime.CompilerServices;

namespace DictionaryRanker.Submissions.igavriilidis
{
    public static class FastHash
    {
        private const ulong k0 = 0xb492b66fbe98f273U;
        private const ulong k1 = 0xc3a5c85c97cb3127U;
        private const ulong k2 = 0x9ae16a3b2f90404fU;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Rotate64(ulong val, int shift) =>
           (val >> shift) | (val << (64 - shift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong Fetch64(byte* p) => *(ulong*)p;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong HashLen16(ulong u, ulong v, ulong mul)
        {
            return (v ^ u) * mul;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong H32(byte* s, uint len, ulong mul)
        {
            ulong a = Fetch64(s) & k0;
            ulong b = Fetch64(s + 8);
            ulong c = Fetch64(s + len - 8) * mul;
            ulong d = Fetch64(s + len - 16) ^ k1;

            ulong u = Rotate64(a + b, 2) + Rotate64(c, 32) + d;
            ulong v = a + c + b + k0;
            return (v ^ u) * len * mul;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen0to16(byte* s, uint len)
        {
            const ulong mul = k2 + 8;
            ulong a = Fetch64(s) + k2;
            ulong b = Fetch64(s + len - 8);
            ulong c = Rotate64(b, 32) * mul + a;
            ulong d = (Rotate64(a, 16) + b) * mul;
            return HashLen16(c, d, mul + len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen17to32(byte* s, uint len)
        {
            const ulong mul = k2 + 8;
            ulong a = Fetch64(s) * k1;
            ulong b = Fetch64(s + 8);
            ulong c = Fetch64(s + len - 8) * mul;
            ulong d = Fetch64(s + len - 16) * k2;
            return HashLen16(Rotate64(a + b, 24) + Rotate64(c, 32) + d, Rotate64(b + len, 46) + c, mul);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen33to64(byte* s, uint len)
        {
            ulong h0 = H32(s, 24, k2 + len);
            ulong h1 = H32(s + len - 32, 34, k2);
            return (h1 * k2 + h0) * k2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen65to96(byte* s, uint len)
        {
            ulong mul1 = k2 - 16 * len;
            ulong h0 = H32(s, 32, k0);
            ulong h1 = H32(s + 32, 32, k1);
            ulong h2 = H32(s + len - 32, 34, k2);
            return (h2 * 4 + (h0 >> 16) + (h1 >> 4)) * mul1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong Hash64(byte* s, uint len)
        {
            if (len <= 16)
            {
                return HashLen0to16(s, len);
            }
            else if (len <= 32)
            {
                return HashLen17to32(s, len);
            }
            else if (len <= 64)
            {
                return HashLen33to64(s, len);
            }
            else
            {
                return HashLen65to96(s, len);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ulong Hash64(string s)
        {
            fixed (char* buffer = s)
            {
                return Hash64((byte*)buffer, (uint)(s.Length * sizeof(char)));
            }
        }
    }
}
