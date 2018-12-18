using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiaNotes.ViewModels;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using System.Text.RegularExpressions;

namespace PiaNotes.Models
{
    class SheetMusic
    {
        public MidiFile MF { get; set; }
        public bool multipleClefs { get; set; }
        public List<Note> notes { get; set; }
        public List<Bar> bars { get; set; }
        public int TicksPerQuaterNote;

        //If there are multiple clefs there should be a List of clefs and staffs
        public List<Clef> clefs = new List<Clef>();
        public List<Staff> staffs = new List<Staff>();

        public SheetMusic(MidiFile midiFile, List<Note> Notes, bool MC, int AB)
        {
            MF = midiFile;
            notes = Notes;
            multipleClefs = MC;

            string ttq = MF.GetTempoMap().TimeDivision.ToString();
            TicksPerQuaterNote = Int32.Parse(Regex.Match(ttq, @"\d+").Value);

            if (AB > 1)
            {
                bars = new List<Bar>();
                for (int i = AB; i > 0; i--)
                {
                    bars.Add(new Bar());
                }
            } else
            {
                bars.Add(new Bar());
            }

            Clef cl = new Clef(Clef.ClefTypes.Treble);
            Clef ef = new Clef(Clef.ClefTypes.Bass);
            Staff st = new Staff();
            Staff aff = new Staff();

            clefs.Add(cl);
                staffs.Add(st);

            if (multipleClefs)
            {
                clefs.Add(ef);
                staffs.Add(aff);
            }
        }
    }
}
