﻿#define DEBUG

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
            new Thread(() =>
            {
                _server = new Server();

                int startCounter = 0;

                while (startCounter <= 10)
                {
                    if (_server.Start())
                    {
                        do
                        {
                            _server.Accept();

                            while (_server.UserConnected)
                            {
                                string command = _server.Receive();

                                if (command != null)
                                {
                                    new Thread(() =>
                                    {
                                        Logic.Do(command);
                                    }).Start();
                                }
                            }
                        } while (_server.Online);
                    }
                    else
                    {
                        Console.WriteLine("Server Start failed - Attempt " + ++startCounter);
                    }
                }

                Console.WriteLine("Shutting down application");
                this.Shutdown();
            }).Start();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _server.Stop();
        }
    }
}
