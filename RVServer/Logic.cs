using Newtonsoft.Json;

namespace RemoteVolume.Server
{
    class Logic
    {
        public static void Do(string json)
        {
            Command command = JsonConvert.DeserializeObject<Command>(json);

            switch (command.Action)
            {
                case Action.ChangeVolume:
                    VolumeControl.SetApplicationVolume(command.App, command.Volume);
                    break;
                case Action.ToggleMute:
                    VolumeControl.SetApplicationMute(command.App, (bool) !VolumeControl.GetApplicationMute(command.App));
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
