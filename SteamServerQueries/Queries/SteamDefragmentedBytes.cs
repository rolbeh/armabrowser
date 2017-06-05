namespace Magic.Steam.Queries
{
    public class SteamDefragmentedBytes
    {
        public SteamDefragmentedBytes(byte[] bytes)
        {
            Bytes = bytes;
        }
        
        public byte[] Bytes { get; }
    }
}