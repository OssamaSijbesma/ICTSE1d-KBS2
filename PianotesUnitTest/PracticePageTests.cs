using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace PianotesUnitTest
{
    [TestClass]
    class PracticePageTests
    {
        [TestMethod]
        public void MidiInPort_MessageReceived()
        {
            
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(0, 62, 99);
        }
    }
}
