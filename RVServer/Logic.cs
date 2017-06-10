using System;
using System.Media;
using Newtonsoft.Json;

namespace RemoteVolume.Server
{
    public class Logic
    {
        public static void Do(string json)
        {
            SystemSounds.Hand.Play();

            Command command = JsonConvert.DeserializeObject<Command>(json);

            switch (command.Action)
            {
                case Action.ChangeVolume:
                    break;
                case Action.ToggleMute:
                    break;
            }
        }
    }

    public class Command
    {
        public Action Action { get; set; }

        private int _volume;
        public int Volume
        {
            get => _volume;
            set
            {
                if (value >= 0 && value <= 100) _volume = value;
            }
        }

        public string App { get; set; }

        public Command(Action Action, int Volume, string App)
        {
            this.Action = Action;
            this.Volume = Volume;
            this.App = App;
        }
    }

    public enum Action
    {
        ChangeVolume,
        ToggleMute,
        Mute,
        Unmute
    }
}
