using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using FWF.KeyExchange.Logging;

namespace FWF.KeyExchange
{
    internal class RngRandom : IRandom
    {
        private static volatile RNGCryptoServiceProvider _rng;

        private readonly byte[] _buffer = new byte[_bufferSize];
        private int _bufferOffset = 0;
        private const int _bufferSize = 4096;

        private volatile static object _lockObject = new object();
        
        // Excludes the follow characters: BIOS1
        private static readonly char[] _chars = "ACDEFGHJKLNPQRTUVXYZ2346789".ToCharArray();
        private static readonly char[] _alphaChars = "ACDEFGHJKLNPQRTUVXYZ".ToCharArray();

        private readonly ILog _log;

        static RngRandom()
        {
            _rng = new RNGCryptoServiceProvider();
        }

        public RngRandom(
            ILogFactory logFactory
            )
        {
            _log = logFactory.CreateForType(this);

            //
            _rng.GetBytes(_buffer);
        }

        public int Any()
        {
            int r;
            
            lock (_lockObject)
            {
                if (_bufferOffset + 4 > _bufferSize)
                {
                    _rng.GetBytes(_buffer);
                    _bufferOffset = 0;
                }

                r = _buffer[_bufferOffset]
                    | _buffer[_bufferOffset + 1] << 8
                    | _buffer[_bufferOffset + 2] << 16
                    | _buffer[_bufferOffset + 3] << 24;

                _bufferOffset += 4;
            }

            // Or with max value to remove the first bit
            if (r < 0) r *= -1;

            return r;
        }

        public long AnyLong()
        {
            long r;

            lock (_lockObject)
            {
                if (_bufferOffset + 8 > _bufferSize)
                {
                    _rng.GetBytes(_buffer);
                    _bufferOffset = 0;
                }
                
                r = 
                    (long)_buffer[_bufferOffset + 4]
                    | (long)_buffer[_bufferOffset + 5] << 8
                    | (long)_buffer[_bufferOffset + 6] << 16
                    | (long)_buffer[_bufferOffset + 7] << 24
                    | (long)_buffer[_bufferOffset + 4] << 32
                    | (long)_buffer[_bufferOffset + 5] << 40
                    | (long)_buffer[_bufferOffset + 6] << 48
                    | (long)_buffer[_bufferOffset + 7] << 56;
                
                _bufferOffset += 8;
            }

            // Or with max value to remove the first bit
            if (r < 0) r *= -1;

            return r;
        }

        public int Any(int maxValue)
        {
            int r = Any();
            int a;

            checked
            {
                double rD = (double)r / (double)int.MaxValue;
                a = (int)(rD * maxValue);
            }

            return a;
        }

        public int Any(int minValue, int maxValue)
        {
            if (maxValue <= minValue)
            {
                throw new ArgumentOutOfRangeException("Must supply max greater than min");
            }

            int r = Any();
            int diff = maxValue - minValue;

            int a;

            checked
            {
                double rD = (double)r / (double)int.MaxValue;
                
                double rD2 = (double)rD * (double)diff;

                a = minValue + (int)rD2;
            }

            return a;
        }

        public long Any(long minValue, long maxValue)
        {
            if (maxValue <= minValue)
            {
                throw new ArgumentOutOfRangeException("Must supply max greater than min");
            }

            long r = AnyLong();
            long diff = maxValue - minValue;

            long a;

            checked
            {
                double rD = (double)r / (double)long.MaxValue;

                double rD2 = (double) rD*(double) diff;
                
                a = minValue + (long)rD2;
            }

            return a;
        }

        public T Any<T>(IList<T> list)
        {
            if (ReferenceEquals(list, null))
            {
                return default(T);
            }
            if (list.Count == 0)
            {
                return default(T);
            }
            if (list.Count == 1)
            {
                return list[0];
            }

            int randomIndex = Any(0, list.Count - 1);

            return list[randomIndex];
        }

        public string AnyString(int size)
        {
            return AnyString(size, false);
        }

        public string AnyString(int size, bool alphaOnly)
        {
            var builder = new StringBuilder(size);

            var lastChar = 'Z';

            for (var i = 0; i < size; i++)
            {
                int r;

                if (alphaOnly)
                {
                    r = Any(_alphaChars.Length - 1);
                }
                else
                {
                    r = Any(_chars.Length - 1);
                }

                if (r == lastChar)
                {
                    i--;
                    continue;
                }

                builder.Append(_chars[r]);

                lastChar = _chars[r];
            }

            return builder.ToString();
        }

        public bool AnyBoolean()
        {
            var b = new byte[1];
            _rng.GetBytes(b);

            // b[0] could be 0 to 255
            return b[0] > 127;
        }

        public byte[] AnyBytes(int length)
        {
            if (length <= 0 || length > (int) Bytes.MB)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            var b = new byte[length];
            _rng.GetBytes(b);

            return b;
        }

    }
}
