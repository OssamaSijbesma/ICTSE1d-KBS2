using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using PiaNotes.Models;
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

        // Create a list with all flat keys.
        public List<int> GetFlatKeys()
        {
            // Create array with all flat keys in one octave.
            int[] keyFlatOctave = new int[] { 0, 2, 4, 5, 7, 9, 11 };
            // Create list for all flat keys.
            List<int> flatKeysAll = new List<int>();
            // Add all flat key numbers to list.
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < keyFlatOctave.Length; j++)
                {
                    flatKeysAll.Add(keyFlatOctave[j] + 12 * (i + 1));
                }
            }

            return flatKeysAll;
        }

        // Get highest and lowest notes in the SheetMusic.
        public (int, int) GetNotes(SheetMusic SM)
        {
            int max = 0;
            int min = 127;
            for (int i = 0; i < SM.notes.Count; i++)
            {
                if (SM.notes[i].Number > max)
                {
                    max = SM.notes[i].Number;
                }
                if (SM.notes[i].Number < min)
                {
                    min = SM.notes[i].Number;
                }
            }

            return (max, min);
        }
        
        // Retrieves Octave information: Highest, lowest octaves and the octaves used.
        // octavesUsed has to be calculated inside this method due to its dependency to the octaves' odd/even state.
        public (int, int, int) GetOctaveInfo(SheetMusic SM)
        {
            var notes = GetNotes(SM);
            
            int highestOctave = 0;
            int lowestOctave = 9;
            int octavesUsed = 0;
            for (int i = 0; i < 10; i++)
            {
                if (i * 12 < notes.Item1)
                {
                    if (i % 2 == 0)
                    {
                        highestOctave = i;
                    }
                    else
                    {
                        highestOctave = i + 1;
                    }
                }
            }
            for (int i = 10; i > 0; i--)
            {
                if (i * 12 > notes.Item2)
                {
                    if (i % 2 == 0)
                    {
                        lowestOctave = i;
                    }
                    else
                    {
                        lowestOctave = i + 1;
                    }

                    octavesUsed = Math.Abs(highestOctave - i);
                }
            }

            return (highestOctave, lowestOctave, octavesUsed);
        }
    }
}
