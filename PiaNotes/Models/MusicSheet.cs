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
        public string File { get; set; }

        public MusicSheet(int id, string title, string file)
        {
            Id = id;
            Title = title;
            File = file;
        }
    }
}
