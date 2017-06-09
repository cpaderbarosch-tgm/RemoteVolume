using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RemoteVolume.Server;

namespace RemoteVolume.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void ChangeVolume()
        {
            VolumeControl.Do(JsonConvert.SerializeObject(new Command(Server.Action.ChangeVolume, 10, "System Sounds")));

            Assert.AreEqual(10, VolumeControl.GetApplicationVolume("System Sounds"));
        }
    }
}
