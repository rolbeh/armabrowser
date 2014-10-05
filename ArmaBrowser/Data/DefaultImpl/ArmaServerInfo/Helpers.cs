using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.DefaultImpl.ArmaServerInfo
{
    static class Helpers
    {
        public static byte[] GetTimeStamp()
        {
#if DEBUG
            return new byte[] { 0x06, 0x06, 0x06, 0x06 }; // Fixed timestamp footprint for easier packet debugging
#else

            DateTime now = DateTime.Now;
            DateTime epoch = new DateTime(1970, 1, 1);
            return BitConverter.GetBytes((int)(now - epoch).TotalSeconds);
#endif

        }
        public static int GetTimeStampInt()
        {
            return BitConverter.ToInt32(GetTimeStamp(), 0);
        }

        /// <summary>
        /// Converts a hex string to its byte array equivalent
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        /// <remarks>http://stackoverflow.com/a/14335076</remarks>
        public static byte[] ParseHex(string hexString)
        {
            if ((hexString.Length & 1) != 0)
            {
                throw new ArgumentException("Input must have even number of characters");
            }
            byte[] ret = new byte[hexString.Length / 2];
            for (int i = 0; i < ret.Length; i++)
            {
                int high = ParseNybble(hexString[i * 2]);
                int low = ParseNybble(hexString[i * 2 + 1]);
                ret[i] = (byte)((high << 4) | low);
            }

            return ret;
        }

        private static int ParseNybble(char c)
        {
            unchecked
            {
                uint i = (uint)(c - '0');
                if (i < 10)
                    return (int)i;
                i = ((uint)c & ~0x20u) - 'A';
                if (i < 6)
                    return (int)i + 10;
                throw new ArgumentException("Invalid nybble: " + c);
            }
        }

        public static byte[] GetPackedBytes(int value)
        {
            return new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)(value >> 0)
            };
        }
    }
}

