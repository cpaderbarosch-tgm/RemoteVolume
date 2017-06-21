namespace RemoteVolume
{
    public class Command
    {
        public Action Action { get; set; }
        public int? Id { get; set; }
        public bool Mute { get; set; }
        public float Volume { get; set; }
    }

    public enum Action
    {
        ChangeVolume,
        ChangeMute,
    }

    public class AppVolume
    {
        public string Name { get; set; }
        public int? Id { get; set; }
        public bool Mute { get; set; }
        public float Volume { get; set; }
    }
}
