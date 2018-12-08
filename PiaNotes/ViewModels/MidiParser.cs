using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiaNotes.Models;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

namespace PiaNotes.ViewModels
{
    class MidiParser
    {
        private List<Models.Note> notes = new List<Models.Note>();
        private bool multipleClefs;
        private int amountBars;
        public SheetMusic sheetMusic;

        public MidiParser(MidiFile midiFile)
        {
            // Get the tempo of the midiFile
            TempoMap tempo = midiFile.GetTempoMap();

            //Get every note from the list
            List<int> noteNumbers = midiFile.GetNotes().Select(n => $"{n.NoteNumber}").Select(int.Parse).ToList();
            List<int> noteTimes = midiFile.GetNotes().Select(n => $"{n.Time}").Select(int.Parse).ToList();
            List<int> noteLengths = midiFile.GetNotes().Select(n => $"{n.Length}").Select(int.Parse).ToList();

            // Convert noteTimes to metric time
            List<MetricTimeSpan> noteMetricTimes = noteTimes
                .Select(t => TimeConverter.ConvertTo<MetricTimeSpan>(t, tempo))
                .ToList();

            // Convert noteLengths to metric time
            List<MetricTimeSpan> noteMetricLengths = noteLengths
                .Select(l => TimeConverter.ConvertTo<MetricTimeSpan>(l, tempo))
                .ToList();

            //BarBeatTimeSpan musicalTime = note.TimeAs<BarBeatTimeSpan>(tempo);

            //Set the notes in an array and sends the array to SheetMusic
            for (int i = 0; i < noteNumbers.Count() - 1; i++)
            {
                notes.Add(new Models.Note(
                    noteNumbers[i], 
                    noteTimes[i], 
                    noteLengths[i],
                    noteMetricTimes[i],
                    noteMetricLengths[i]));
            }

            //Check if multiple clefs are needed
            CheckClefs(notes);

            //Check how many bars are needed
            CheckBars(notes);

            //Create SheetMusic with every element MidiParser calculated
            sheetMusic = new SheetMusic(midiFile, notes, multipleClefs, amountBars);
        }

        public void CheckClefs(List<Models.Note> notes)
        {
            foreach(Models.Note n in notes)
            {
                //Check if a note is lower than the treble clef supports, if not the bass cleff will be added
                if(n.Number <= 60)
                {
                    multipleClefs = true;
                }
            }
            multipleClefs = false;
        }

        public void CheckBars(List<Models.Note> notes)
        {
            //Check how many bars are needed in the song
            //There are 96 ticks for a quarternote, so in a 4/4 beat there are 384
            int ticks = 0;

            foreach (Models.Note n in notes)
            {
                ticks += n.Length;
            }
            amountBars = ticks / 384;
        }
    }
}
