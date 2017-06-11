using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteVolume.Server;
using RemoteVolume;

namespace RemoteVolume.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CanChangeVolume()
        {
            foreach (AudioSession session in AudioUtilities.GetAllSessions())
            {
                if (session.Process != null)
                {
                    Console.WriteLine(session.ProcessId + ": " + VolumeControl.GetApplicationVolume(session.ProcessId));
                    VolumeControl.SetApplicationVolume(session.ProcessId, 50f);
                }
            }
        }
    }
}
