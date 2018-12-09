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
        public static Boolean feedback = true;
        public static double volume = 0;
        

        //Octave Settings
        //OctaveStart is the starting frequency of the octave
        public static int StartingOctave = Views.SettingsPages.MIDI_SettingsPage.StartingOctave;

        //OctaveAmount is the amount of octaves on your screen at once
        public static int OctaveAmount = Views.SettingsPages.MIDI_SettingsPage.OctaveAmount;

        //Feedback Colours in RGB
        public static byte R = 43;
        public static byte G = 60;
        public static byte B = 73;
    }
}
