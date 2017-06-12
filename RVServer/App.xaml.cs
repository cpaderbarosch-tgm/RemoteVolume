//#define TESTING

using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

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
                                    Do(JsonConvert.DeserializeObject<Command>(command));
                                }).Start();
                            }
                        }
                    } while (_server.Online);
                }

                this.Shutdown();
            }).Start();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _server.Stop();
        }

        public static void Do(Command command)
        {
            SystemSounds.Hand.Play();

            switch (command.Action)
            {
                case Action.ChangeVolume:
                    break;
                case Action.ToggleMute:
                    break;
            }
        }

        public void SendMixer()
        {
            List<AppVolume> apps = new List<AppVolume>();

            apps.Add(new AppVolume("Master Volume", null, VolumeControl.GetMasterMute(), VolumeControl.GetMasterVolume()));

            foreach (AudioSession session in AudioUtilities.GetAllSessions())
            {
                if (session.Process != null)
                {
                    int pid = session.ProcessId;
                    apps.Add(new AppVolume(session.Process.ProcessName, pid, VolumeControl.GetApplicationMute(pid), VolumeControl.GetApplicationVolume(pid)));
                }
            }

            string json = JsonConvert.SerializeObject(apps.ToArray());
            _server.Send(json);
        }
    }
}
