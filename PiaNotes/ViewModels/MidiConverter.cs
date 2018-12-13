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
        private StringBuilder sb = new StringBuilder();

        public string MidiToString(Stream midiFilePath)
        {
            var midiFile = MidiFile.Read(midiFilePath);
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
            }
            
            return sb.ToString();
        }

        public Byte[] MidiToBytes(Stream stream)
        {
            var bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }
    }
}
