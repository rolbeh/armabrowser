using System;
using System.IO;

namespace Magic.Steam.Queries
{
    public class SteamDecodedBytes : IDisposable
    {
        private readonly Stream _decodedStream;
        private readonly bool _leaveOpen;

        public SteamDecodedBytes(byte[] decodedBytes)
        {
            Data = new MemoryStream(decodedBytes);
            _leaveOpen = false;
        }

        public SteamDecodedBytes(Stream decodedStream, bool leaveOpen = false)
        {
            _decodedStream = decodedStream;
            _leaveOpen = leaveOpen;
            Data = decodedStream;
        }

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