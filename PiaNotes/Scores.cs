using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace PiaNotes
{
    static class Scores
    {
        //Settings for the keyboard, such as: if feedback is turned on, what volume it is or what the set velocity is
        public static int score = 0;
        public static int notesAmount = 0;
        public static int notesHit = 0;
    }
}
