using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteVolume.Server;

namespace RemoteVolume.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CanSetAndGetVolume()
        {
            VolumeControl.SetMasterVolume(100);
            Assert.AreEqual(100, VolumeControl.GetMasterVolume());

            foreach (AudioSession session in AudioUtilities.GetAllSessions())
            {
                if (session.Process != null)
                {
                    VolumeControl.SetApplicationVolume(session.ProcessId, 50);
                    Assert.AreEqual(50, VolumeControl.GetApplicationVolume(session.ProcessId));
                }
            }
        }
    }
}
