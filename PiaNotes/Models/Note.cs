using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.Models
{
    class Note
    {
        private int number;
        private int timing;
        private int length;

        //Creating a note can only happen if you know the number, timing and the length.
        public Note(int n, int t, int l)
        {
            number = n;
            timing = t;
            length = l;
        }
    }
}
