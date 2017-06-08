using System.Threading;
using System.Windows;

namespace Server
{
    public partial class App : Application
    {
        private Server _server;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _server = new Server();

            if (_server.Start())
            {
                _server.Accept();

                while (_server.Online)
                {
                    string received = _server.Receive();

                    Thread validation = new Thread(() =>
                    {
                        Logic.Do(received);
                    });
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _server.Stop();
        }
    }
}
