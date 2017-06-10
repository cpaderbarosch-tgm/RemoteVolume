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
                int startCounter = 0;

                while (startCounter <= 10)
                {
                    try
                    {
                        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        _socket.Bind(new IPEndPoint(IPAddress.Any, PORT));
                        _socket.Listen(10);

                        _online = true;

                        Log(String.Format("Server running at {0}:{1}", IPAddress.Any.ToString(), PORT));

                        return true;
                    }
                    catch (SocketException)
                    {
                        Log("Server Start failed - Attempt " + ++startCounter);
                    }
                }

                Log("Starting Server failed");
                return false;
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
                _client.Send(Encoding.ASCII.GetBytes("closed"));
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
#if TESTING
            Log("Accepting");
#endif

            _client = _socket.Accept();
            _userConnected = true;

            Log("User connected");
        }

        public string Receive()
        {
#if TESTING
            Log("Receiving");
#endif

            int recv_count = _client.Receive(buffer, 0, BUFFER_SIZE, SocketFlags.None);

            byte[] recvBuf = new byte[recv_count];
            Array.Copy(buffer, recvBuf, recv_count);

            string received = Encoding.ASCII.GetString(recvBuf);

            if (received.Contains("disconnect"))
            {
                Log("User disconnected");

                _userConnected = false;
                return null;
            } else if (string.IsNullOrEmpty(received) || string.IsNullOrWhiteSpace(received)) {
                return null;
            }

            received.Replace("\r\n", "");
            return received;
        }

        public void Log(string message)
        {
            Console.WriteLine("[ Server ]: " + message);
        }

        #endregion
    }
}