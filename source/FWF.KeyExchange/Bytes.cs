
namespace FWF.KeyExchange
{
    public enum ByteSize
    {
        B,
        KB,
        MB,
        GB,
        TB,
        PB
    }

    public static class Bytes
    {
        public const long KB = 1024;
        public const long MB = KB * 1024;
        public const long GB = MB * 1024;
        public const long TB = GB * 1024;
        public const long PB = TB * 1024;
    }
}
