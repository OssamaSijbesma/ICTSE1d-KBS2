using PiaNotes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.Models
{
    class Clef : IGameObject
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Heigth { get; set; }
        public enum ClefTypes {Treble, Bass, Alto, Tenor}

        public ClefTypes CT;

        public Clef(ClefTypes ct)
        {
            CT = ct;
        }
    }
}
