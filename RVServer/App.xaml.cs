using Newtonsoft.Json;
using System;
using System.Threading;
using System.Windows;

namespace RemoteVolume.Server
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
                Console.WriteLine("Accepted");

                while (_server.Online)
                {
                    string command = _server.Receive();
                    Console.WriteLine("Received");

                    if (command != "" && command != Environment.NewLine)
                    {
                        new Thread(() =>
                        {
                            Logic.Do(command);
                        }).Start();
                    }
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _server.Stop();
        }
    }
}
