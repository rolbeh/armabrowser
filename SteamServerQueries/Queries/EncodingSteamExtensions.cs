
using System;
using System.IO;

namespace Magic.Steam.Queries
{
    public static class EncodingSteamExtensions
    {
        public static uint ReadSteamUInt32(this BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            var b = DecodeByte(reader);
            uint result = b;

            b = DecodeByte(reader);
            result += (uint) (b << 8);

            b = DecodeByte(reader);
            result += (uint) (b << 16);

            b = DecodeByte(reader);
            result += (uint) (b << 24);


            return result;
        }

        public static byte ReadSteamByte(this BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            return DecodeByte(reader);
        }

        private static byte DecodeByte(BinaryReader reader)
        {
            byte result = reader.ReadByte();
            if (result == 0x01)
            {
                byte nextByte = (byte) reader.PeekChar();
                switch (nextByte)
                {
                    case 0x01:
                        nextByte = reader.ReadByte();
                        result = 0x01;
                        break;
                    case 0x02:
                        nextByte = reader.ReadByte();
                        result = 0x00;
                        break;
                    case 0x03:
                        nextByte = reader.ReadByte();
                        result = 0xFF;
                        break;
                }
            }
            if (result == 0x03)
            {
                byte nextByte = (byte)reader.PeekChar();
                switch (nextByte)
                {
                    case 0x03:
                        nextByte = reader.ReadByte();
                        result = 0x03;
                        break;
                }
            }

            return result;
        }

        public static SteamDecodedBytes DecodeSteamRuleFile_1_56(this SteamDefragmentedBytes unframed)
        {
            using (Stream file = new MemoryStream(unframed.Bytes, false))
            using (var reader = new BinaryReader(file))
            using (var targetStream = new MemoryStream())
            {
                while (file.Position < file.Length)
                {
                    targetStream.WriteByte(reader.ReadSteamByte());
                }

                return new SteamDecodedBytes(targetStream.ToArray());
            }
        }

        public static SteamDefragmentedBytes DefragmentSteamBytes_1_56(this byte[] respose)
        {
            var fragmentCount = respose[5] + (respose[6] >> 8);
            var offset = 7;
            var result = new byte[respose.Length];
            var destinationOffset = 0;
            for (var i = 0; i < fragmentCount; i++)
            {
                var fragmentNr = respose[offset];
                if (respose[offset + 1] != fragmentCount) throw new InvalidDataException($"expect {fragmentCount} but {respose[offset + 1]} found!");
                offset += 3;

                var fragmentLen = Array.FindIndex(respose, offset, b => b == 0x0) - offset;
                if (fragmentNr < fragmentCount)
                {
                    if (fragmentLen != 127)
                        throw new InvalidDataException();
                }

                Array.Copy(respose, offset, result, destinationOffset, fragmentLen);
                destinationOffset += fragmentLen;

                offset += fragmentLen + 1;
                if (fragmentNr == fragmentCount)
                    break;
            }
            Array.Resize(ref result, destinationOffset);
            return new SteamDefragmentedBytes(result);
        }
    }
}