//#define TESTING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace RemoteVolume
{
    public partial class App : Application
    {
        private Server _server;

        private List<AppVolume> _apps = new List<AppVolume>();
        private Thread _check;

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

                        GetMixer();
                        _server.Send(JsonConvert.SerializeObject(_apps.ToArray()));

                        _check = new Thread(Check);
                        _check.Start();

                        while (_server.UserConnected)
                        {
                            string command = _server.Receive();

                            if (command != null)
                            {
                                string[] splittedCommands = command.Split('\n');

                                foreach (string tempCommand in splittedCommands) {
                                    new Thread(() =>
                                    {
                                        Do(JsonConvert.DeserializeObject<Command>(tempCommand));
                                    }).Start();
                                }
                            }
                        }
                        _server.Log("User disconnected");

                        _check.Abort();
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
            Console.WriteLine(JsonConvert.SerializeObject(command));

            switch (command.Action)
            {
                case Action.ChangeVolume:
                    if (command.Id == null)
                    {
                        VolumeControl.SetMasterVolume(command.Volume);
                    }
                    else
                    {
                        VolumeControl.SetApplicationVolume((int) command.Id, command.Volume);
                    }
                    break;
                case Action.ChangeMute:
                    if (command.Id == null)
                    {
                        VolumeControl.SetMasterMute(command.Mute);
                    }
                    else
                    {
                        VolumeControl.SetApplicationMute((int) command.Id, command.Mute);
                    }
                    break;
            }
        }

        private void GetMixer()
        {
            _apps = new List<AppVolume>();

            _apps.Add(new AppVolume { Name = "Master Volume", Id = null, Mute = VolumeControl.GetMasterMute(), Volume = VolumeControl.GetMasterVolume() });

            IList<AudioSession> sessions = AudioUtilities.GetAllSessions();

            foreach (AudioSession session in sessions)
            {
                if (session.Process != null && session.Process.Responding)
                {
                    int pid = session.ProcessId;

                    if (_apps.Find(app => app.Id == pid) == null)
                    {
                        string pname = session.Process.MainWindowTitle;

                        if (pname.Length > 20)
                        {
                            pname = pname.Substring(0, 17) + "...";
                        }

                        _apps.Add(new AppVolume { Name = pname, Id = pid, Mute = VolumeControl.GetApplicationMute(pid), Volume = VolumeControl.GetApplicationVolume(pid) });
                    }
                }
            }
        }

        public void Check()
        {
            try
            {
                while (true)
                {
                    List<AppVolume> oldApps = _apps;

                    GetMixer();

                    bool change = false;

                    if (oldApps.Count != _apps.Count)
                        change = true;

                    if (!change)
                    {
                        for (int i = 0; i < oldApps.Count; ++i)
                        {
                            if (oldApps[i].Volume != _apps[i].Volume || oldApps[i].Mute != _apps[i].Mute)
                            {
                                change = true;
                                break;
                            }
                        }
                    }

                    if (change) _server.Send(JsonConvert.SerializeObject(_apps.ToArray()));

                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException) { }
        }
    }
}
