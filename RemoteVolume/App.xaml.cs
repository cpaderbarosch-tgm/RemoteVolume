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
            //new Thread(() =>
            //{
            //    _server = new Server();

            //    if (_server.Start())
            //    {
            //        do
            //        {
            //            _server.Accept();

            //            GetMixer();
            //            _server.Send(JsonConvert.SerializeObject(_apps.ToArray()));

            //            _check = new Thread(Check);
            //            _check.Start();

            //            while (_server.UserConnected)
            //            {

            //                string command = _server.Receive();

            //                if (command != null)
            //                {
            //                    new Thread(() =>
            //                    {
            //                        Do(JsonConvert.DeserializeObject<Command>(command));
            //                    }).Start();
            //                }
            //            }

            //            _check.Abort();
            //        } while (_server.Online);
            //    }

            //    this.Shutdown();
            //}).Start();

            GetMixer();
            Check();
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

        private void GetMixer()
        {
            _apps = new List<AppVolume>();

            _apps.Add(new AppVolume("master", null, VolumeControl.GetMasterMute(), VolumeControl.GetMasterVolume()));

            IList<AudioSession> sessions = AudioUtilities.GetAllSessions();

            foreach (AudioSession session in sessions)
            {
                if (session.Process != null)
                {
                    int pid = session.ProcessId;

                    if (_apps.Find(app => app.Id == pid) == null)
                    {
                        _apps.Add(new AppVolume(session.Process.ProcessName, pid, VolumeControl.GetApplicationMute(pid), VolumeControl.GetApplicationVolume(pid)));
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

                    //if (change) _server.Send(JsonConvert.SerializeObject(_apps.ToArray()));
                    if (change) Console.WriteLine(JsonConvert.SerializeObject(_apps.ToArray()));

                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException) { }
        }
    }
}
