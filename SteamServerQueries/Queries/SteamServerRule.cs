using System.Text;

namespace Magic.Steam.Queries
{
    public class SteamServerRule
    {
        public SteamServerRule(uint hash, uint publisherId, string name)
        {
            Hash = hash;
            PublisherId = publisherId;
            Name = name;
        }

        public string Key { get; set; }

        public uint Hash { get; }

        public uint PublisherId { get; }

        public string Name { get; }
        
        #region Overrides of Object

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Key       : ");
            sb.AppendLine(Key);
            sb.Append("Mod Hash  : ");
            sb.AppendLine(Hash.ToString("x"));
            sb.Append("Mod PubId : ");
            sb.AppendLine(PublisherId.ToString());
            sb.Append("Mod String: ");
            sb.AppendLine(Name);

            return sb.ToString();
        }

        #endregion
    }
}