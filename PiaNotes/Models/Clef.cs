using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.Models
{
    class Clef
    {
        public enum ClefTypes {Treble, Bass, Alto, Tenor}

        public ClefTypes CT;

        public Clef(ClefTypes ct)
        {
            CT = ct;
        }
    }
}
