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
        private bool multipleClefs;
        private int amountBars;

        public MidiParser(MidiFile MF)
        {
            //new Models.Note(Int32.Parse(MF.GetNotes().Select(n => $"n.NoteNumber")));
            //Get every note from the list
            List<string> notesNumbers = (MF.GetNotes().Select(n => $"{n.NoteNumber}")).ToList();
            List<string> notesTimes = (MF.GetNotes().Select(n => $"{n.Time}")).ToList();
            List<string>  notesLengths = (MF.GetNotes().Select(n => $"{n.Length}")).ToList();

            //Set the notes in an array and sends the array to SheetMusic
            for (int i = 0; i < notesNumbers.Count() - 1; i++)
            {
                notes.Add(new Models.Note(Int32.Parse(notesNumbers[i]), Int32.Parse(notesTimes[i]), Int32.Parse(notesLengths[i])));
            }

            //Check if multiple clefs are needed
            CheckClefs(notes);

            //Check how many bars are needed
            CheckBars(notes);

            //Create SheetMusic with every element MidiParser calculated
            SheetMusic SM = new SheetMusic(MF, notes, multipleClefs);

        }

        public void CheckClefs(List<Models.Note> notes)
        {
            foreach(Models.Note n in notes)
            {
                //Check if a note is lower than the treble clef supports, if not the bass cleff will be added
                if(n.number <= 60)
                {
                    multipleClefs = true;
                }
            }
            multipleClefs = false;
        }

        public void CheckBars(List<Models.Note> notes)
        {
            //Check how many bars are needed in the song
            //TO DO see how many ticks there are per bar
            int ticks = 0;

            foreach (Models.Note n in notes)
            {
                ticks += n.length;
            }
            
            //bars = ticks/(tickamount)
        }
    }
}
