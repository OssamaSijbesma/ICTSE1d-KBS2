using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace PiaNotes
{
    public static class Settings
    {
        public static MidiInPort midiInPort;
        public static IMidiOutPort midiOutPort;

        public static int velocity = 0;

    }
}
