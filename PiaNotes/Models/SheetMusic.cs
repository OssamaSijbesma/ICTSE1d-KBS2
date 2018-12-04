using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiaNotes.ViewModels;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;

namespace PiaNotes.Models
{
    class SheetMusic
    {
        public MidiFile MF { get; set; }
        public bool multipleClefs { get; set; }

        private Clef clef;
        private Staff staff;
        private List<Note> notes;
        private List<Bar> bars;

        //If there are multiple clefs there should be a List of clefs and staffs
        private List<Clef> clefs;
        private List<Staff> staffs;

        public SheetMusic(MidiFile MiFi, List<Note> Notes, bool MC)
        {
            MF = MiFi;
            notes = Notes;
            multipleClefs = MC;

            if (multipleClefs)
            {
                clefs = new List<Clef>();
                staffs = new List<Staff>();

                Clef cl = new Clef(Clef.ClefTypes.Treble);
                Clef ef = new Clef(Clef.ClefTypes.Bass);
                Staff st = new Staff();
                Staff aff = new Staff();

                clefs.Add(cl);
                clefs.Add(ef);
                staffs.Add(st);
                staffs.Add(aff);
            } else
            {
                clef = new Clef(Clef.ClefTypes.Treble);
                staff = new Staff();
            }
        }
    }
}
