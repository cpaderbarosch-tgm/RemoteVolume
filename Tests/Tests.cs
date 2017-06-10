using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteVolume.Server;

namespace RemoteVolume.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CanChangeVolume()
        {
            //foreach (AudioSession session in AudioUtilities.GetAllSessions())
            //{
            //    if (session.Process != null)
            //    {
            //        Console.WriteLine(session.DisplayName + ": " + AudioManager.GetApplicationVolume(session.ProcessId));
            //    }
            //}

            Console.WriteLine(AudioManager.GetApplicationVolume(11988));
        }
    }
}
