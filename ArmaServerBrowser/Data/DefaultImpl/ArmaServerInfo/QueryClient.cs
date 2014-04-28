using ArmaServerBrowser.Data.DefaultImpl.ArmaServerInfo.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Data.DefaultImpl.ArmaServerInfo
{
    class QueryClient
    {
        private readonly byte[] timestamp = Helpers.GetTimeStamp();

        public PacketCollection QueryServer(IPEndPoint gameServerEndPoint)
        {
            if (gameServerEndPoint == null)
                throw new ArgumentNullException("gameServerEndPoint");

            PacketCollection packets = new PacketCollection();

            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (UdpClient udp = new UdpClient())
            {
                udp.Client = client;
                byte[] request, response,
                    timestamp = Helpers.GetTimeStamp();



                IPEndPoint remoteIpEndpoint = new IPEndPoint(gameServerEndPoint.Address, gameServerEndPoint.Port);
                client.ReceiveTimeout = 500;
                client.Connect(remoteIpEndpoint);

                request = GetChallengeRequest(timestamp);
                client.Send(request);


                var receiveState = new SocketReceiver();
                receiveState.Socket = client;
                receiveState.Buffer = new byte[1024 * 8];
                receiveState.BeginReceive().WaitOne();

                if (receiveState.SocketError != SocketError.Success)
                    return null;

                //response = udp.Receive(ref remoteIpEndpoint);
                response = receiveState.Buffer;
                ChallengePacket pChallenge = new ChallengePacket(response);

                request = GetInfosRequest(timestamp, pChallenge.GetChallenge());
                client.Send(request);

                try
                {


                    while (true)
                    {
                        try
                        {
                            response = udp.Receive(ref remoteIpEndpoint);
                            packets.Add(new InfoPacket(response));
                            if (udp.Available == 0)
                                break;
                        }
                        catch (SocketException ex)
                        {
                            if (ex.ErrorCode == (int)SocketError.TimedOut)
                                break;
                            else
                                throw;
                        }
                    }
                }
                finally
                {
                    client.Close();
                }
          
            }

            return packets;
        }

        class SocketReceiver
        {
            public Socket Socket;
            public SocketError SocketError;
            public bool Finished;
            private System.Threading.ManualResetEventSlim waitHandle = new System.Threading.ManualResetEventSlim(false);

            public byte[] Buffer;

            internal System.Threading.WaitHandle BeginReceive()
            {

                var ar = Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, out  SocketError, Receive, this);
                if (!ar.AsyncWaitHandle.WaitOne(Socket.ReceiveTimeout))
                {
                    waitHandle.Set();
                    SocketError = System.Net.Sockets.SocketError.TimedOut;
                }

                return waitHandle.WaitHandle;
            }

            private void Receive(IAsyncResult ar)
            {
                try
                {
                    var st = ar.AsyncState as SocketReceiver;
                    if (SocketError != System.Net.Sockets.SocketError.Success) return;
                    SocketError serr;
                    var len = st.Socket.EndReceive(ar, out serr);
                    st.SocketError = serr;
                    Array.Resize<byte>(ref st.Buffer, len);
                    st.Finished = true;
                }
                finally
                {
                    waitHandle.Set();
                }
            }
        }

        private byte[] GetChallengeRequest(byte[] timestamp)
        {
            return GetRequestHeader((byte)PacketTypes.Challenge, timestamp);
        }
        private byte[] GetInfosRequest(byte[] timestamp, byte[] challenge)
        {
            List<byte> result = new List<byte>();
            result.AddRange(GetRequestHeader((byte)PacketTypes.ServerInfo, timestamp));
            result.AddRange(challenge);
            result.AddRange(new byte[] { 0xff, 0xff, 0xff, 0x01 });
            return result.ToArray();
        }
        private byte[] GetRequestHeader(byte requestType, byte[] timestamp)
        {
            List<byte> result = new List<byte>();
            result.AddRange(new byte[] { 0xfe, 0xfd });
            result.Add(requestType);
            result.AddRange(timestamp);
            return result.ToArray();
        }
    }
}
