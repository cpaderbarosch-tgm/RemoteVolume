using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteVolume.Server
{
    public class Server
    {
        #region Attributes

        private Socket _socket;
        private Socket _client;

        private bool _online = false;
        public bool Online { get => _online; }

        private bool _userConnected = false;
        public bool UserConnected { get => _userConnected; }

        private const int PORT = 3131;
        private const int BUFFER_SIZE = 2048;
        private byte[] buffer = new byte[BUFFER_SIZE];

        #endregion

        #region General Functions

        public bool Start()
        {
            if (!_online)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, PORT));
                _socket.Listen(10);

                _online = true;

                Console.WriteLine(String.Format("Server running at {0}:{1}", IPAddress.Any.ToString(), PORT));

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Stop()
        {
            if (_online)
            {
                _socket.Close();

                _online = false;

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Accept()
        {
#if DEBUG
            Console.WriteLine("Accepted a user");
#endif

            _client = _socket.Accept();
            _userConnected = true;
        }

        public string Receive()
        {
#if DEBUG
            Console.WriteLine("Received a message");
#endif

            int recv_count = _client.Receive(buffer, 0, BUFFER_SIZE, SocketFlags.None);

            byte[] recvBuf = new byte[recv_count];
            Array.Copy(buffer, recvBuf, recv_count);

            string received = Encoding.ASCII.GetString(recvBuf);

            if (received == "disconnect")
            {
                _userConnected = false;
                return null;
            } else if (received == "" || received == Environment.NewLine) {
                return null;
            }

            return received;
        }

        #endregion
    }
}