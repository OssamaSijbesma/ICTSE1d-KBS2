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
    public class MusicSheet
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public MidiFile MF { get; set; }

        private Clef clef;
        private Staff staff;
        private List<Note> notes;
        public bool multipleClefs { get; set; }

        //If there are multiple clefs there should be a List of clefs and staffs
        private List<Clef> clefs;
        private List<Staff> staffs;

        public MusicSheet(MidiFile MiFi)
        {
            Id = 1;
            Title = "test";
            MF = MiFi;
        }

        public MusicSheet(int id, string title, string path)
        {
            Id = id;
            Title = title;
            Path = path;
        }
    }
}
