using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace PiaNotes
{
    static class Settings
    {
        //Input port and Output port 
        public static MidiInPort midiInPort;
        public static IMidiOutPort midiOutPort;

        //Settings for the keyboard, such as: if feedback is turned on, what volume it is or what the set velocity is
        public static double velocity = 90;
        public static bool disableUserFeedback = false;
        public static double volume = 0;


        //Octave Settings
        //OctaveStart is the starting frequency of the octave
        public static int startingOctave = 4;

        //OctaveAmount is the amount of octaves on your screen at once
        public static int octaveAmount = 2;

        //Feedback Colours in RGB
        public static byte redPrimary = 43;
        public static byte greenPrimary = 60;
        public static byte bluePrimary = 73;

        //Feedback Colours in RGB Secondary
        public static byte redSecondary = 43;
        public static byte greenSecondary = 60;
        public static byte blueSecondary = 73;
    }
}
