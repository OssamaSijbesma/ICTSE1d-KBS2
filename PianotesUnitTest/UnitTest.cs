
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiaNotes;
using Windows.Devices.Midi;

namespace PianotesUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod10()
        {
            byte channel = 0;
            byte note = 60;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            Settings.midiOutPort.SendMessage(midiMessageToSend);
        }
    }
}
