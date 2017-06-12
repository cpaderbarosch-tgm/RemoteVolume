using System;
using System.Media;
using Newtonsoft.Json;

namespace RemoteVolume.Server
{
    public class Command
    {
        public Action Action { get; set; }

        private float _volume;
        public float Volume
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

    public class AppVolume
    {
        public string Name { get; set; }
        public int? Id { get; set; }

        public bool Mute { get; set; }

        private float _volume;
        public float Volume
        {
            get => _volume;
            set
            {
                if (value >= 0 && value <= 100) _volume = value;
            }
        }

        public AppVolume(string Name, int? Id, bool Mute, float Volume)
        {
            this.Name = Name;
            this.Id = Id;
            this.Mute = Mute;
            _volume = Volume;
        }
    }
}
