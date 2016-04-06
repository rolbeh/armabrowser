using Magic.Annotations;

namespace System.IO
{
    internal static class EncodingSteamExtensions
    {
        public static uint ReadSteamUInt32([NotNull] this BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            var b = EncodeByte(reader);
            uint result = b;

            b = EncodeByte(reader);
            result += (uint) (b << 8);

            b = EncodeByte(reader);
            result += (uint) (b << 16);

            b = EncodeByte(reader);
            result += (uint) (b << 24);


            return result;
        }

        public static byte ReadSteamByte([NotNull] this BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            return EncodeByte(reader);
        }

        private static byte EncodeByte([NotNull] BinaryReader reader)
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

        public static SteamDecodedBytes DecodeSteamRuleFile_1_56([NotNull] this SteamUnframedBytes unframed)
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

        internal static SteamUnframedBytes UnframeSteamBytes_1_56([NotNull] this byte[] respose)
        {
            var fagmentCount = respose[5] + (respose[6] >> 8);
            var offset = 7;
            var result = new byte[respose.Length];
            var destinationOffset = 0;
            for (var i = 0; i < fagmentCount; i++)
            {
                var fragmentNr = respose[offset];
                if (respose[offset + 1] != fagmentCount) throw new InvalidDataException();
                offset += 3;

                var fragmentLen = Array.FindIndex(respose, offset, b => b == 0x0) - offset;
                if (fragmentNr < fagmentCount)
                {
                    if (fragmentLen != 127)
                        throw new InvalidDataException();
                }

                Array.Copy(respose, offset, result, destinationOffset, fragmentLen);
                destinationOffset += fragmentLen;

                offset += fragmentLen + 1;
                if (fragmentNr == fagmentCount)
                    break;
            }
            Array.Resize(ref result, destinationOffset);
            return new SteamUnframedBytes(result);
        }
    }

    internal class SteamUnframedBytes
    {
        public SteamUnframedBytes([NotNull] byte[] bytes)
        {
            Bytes = bytes;
        }

        [NotNull]
        public byte[] Bytes { get; }
    }

    internal class SteamDecodedBytes : IDisposable
    {
        private readonly Stream _decodedStream;
        private readonly bool _leaveOpen;

        public SteamDecodedBytes([NotNull] byte[] decodedBytes)
        {
            Data = new MemoryStream(decodedBytes);
            _leaveOpen = false;
        }

        public SteamDecodedBytes([NotNull] Stream decodedStream, bool leaveOpen = false)
        {
            _decodedStream = decodedStream;
            _leaveOpen = leaveOpen;
            Data = decodedStream;
        }

        [NotNull]
        public Stream Data { get; private set; }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!_leaveOpen)
                      Data?.Dispose();
                    Data = null;
                }
                
                disposedValue = true;
            }
        }


        // ~SteamDecodedBytes() {
        //   Dispose(false);
        // }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}