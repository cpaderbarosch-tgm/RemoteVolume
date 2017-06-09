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
            string command = JsonConvert.SerializeObject(new Command(Server.Action.ChangeVolume, 10, "System Sounds"));
            Logic.Do(command);
        }
    }
}
