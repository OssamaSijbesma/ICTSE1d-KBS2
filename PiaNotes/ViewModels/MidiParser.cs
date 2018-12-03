using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiaNotes.Models;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;

namespace PiaNotes.ViewModels
{
    class MidiParser
    {
        private List<Models.Note> notes = new List<Models.Note>();

        public MidiParser(MidiFile MF)
        {
            //new Models.Note(Int32.Parse(MF.GetNotes().Select(n => $"n.NoteNumber")));
            //Get every note from the list
            List<string> notesNumbers = (MF.GetNotes().Select(n => $"{n.NoteNumber}")).ToList();
            List<string> notesTimes = (MF.GetNotes().Select(n => $"{n.Time}")).ToList();
            List<string>  notesLengths = (MF.GetNotes().Select(n => $"{n.Length}")).ToList();

            MusicSheet MS = new MusicSheet(MF);

            for (int i = 0; i < notesNumbers.Count(); i++)
            {
                notes.Add(new Models.Note(Int32.Parse(notesNumbers[i]), Int32.Parse(notesTimes[i]), Int32.Parse(notesLengths[i])));
            }

            if(CheckNotes(notes)) MS.multipleClefs = true;
            else MS.multipleClefs = false;
        }

        public bool CheckNotes(List<Models.Note> notes)
        {
            foreach(Models.Note n in notes)
            {
                //Check if a note is lower than the treble clef supports
                //TO DO find the lowest key of the treble clef and what number it connects to
                if(n.number <= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
