using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.DefaultImpl.ArmaServerInfo.Protocol
{
    public class ServerPacket : Packet
    {
        public ServerPacket(byte packetType, byte[] buffer)
            : base(packetType, buffer)
        { }
        public override byte[] GetData()
        {
            if (this._buffer == null)
                return null;
            return this.GetSelectedBytes(this._buffer, 5, this._buffer.Length - 5);
        }
    }

    public class ChallengePacket : ServerPacket
    {
        public ChallengePacket(byte[] buffer)
            : base((byte)PacketTypes.Challenge, buffer)
        { }
        public byte[] GetChallenge()
        {
            int challenge = Int32.Parse(GetDataString());
            return new byte[] {
                (byte)(challenge >> 24),
                (byte)(challenge >> 16),
                (byte)(challenge >> 8),
                (byte)(challenge >> 0)
            };
        }
    }

    public class InfoPacket : ServerPacket
    {
        public InfoPacket(byte[] buffer)
            : base((byte)PacketTypes.ServerInfo, buffer)
        { }
        public override byte[] GetData()
        {
            if (this._buffer == null)
                return null;
            return this.GetSelectedBytes(this._buffer, 16, this._buffer.Length - 16);
        }
    }
}
