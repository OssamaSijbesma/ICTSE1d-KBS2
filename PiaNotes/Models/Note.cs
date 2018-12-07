using PiaNotes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.Models
{
    class Note : IGameObject
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Heigth { get; set; }

        public int number { get; set; }
        public int timing { get; set; }
        public int length { get; set; }

        //Creating a note can only happen if you know the number, timing and the length.
        public Note(int n, int t, int l)
        {
            number = n;
            timing = t;
            length = l;
        }
    }
}
