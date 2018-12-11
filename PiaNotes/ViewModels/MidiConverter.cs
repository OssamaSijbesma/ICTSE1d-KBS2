using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.ViewModels
{
    public class MidiConverter
    {
        private MidiFile midiFile;
        private StringBuilder sb = new StringBuilder();

        public void SetMidiFile(Stream midiFilePath)
        {
            midiFile = MidiFile.Read(midiFilePath);
        }

        public string MidiToString()
        {
            IEnumerable<string> items = midiFile.GetNotes()
                .Select(n => $"{n.NoteNumber} {n.Time} {n.Length}");
            int count = items.Count();
            int current = 0;
            sb.Clear();
            // Load each notenumber, time and length
            foreach (string i in items)
            {
                if (current != 0)
                {
                    sb.Append("-");
                }
                sb.Append(i);

                current++;
                // Do something with i
            }

            return sb.ToString();
        }

        public string MidiTempoMapToString()
        {
            TempoMap tempoMap = midiFile.GetTempoMap();
            //Tempo tempo = new Tempo(); //microsecondsPerQuarterNote!!
            string ppqn = tempoMap.TimeDivision.ToString();
            /*
            StringBuilder sb2 = new StringBuilder();

            foreach (object element in tempoMap.Tempo)
            {
                sb2.Append(element.ToString());
            }
            */
            return $"ppqn: {ppqn} tmepo: ";
            
        }
    }
}