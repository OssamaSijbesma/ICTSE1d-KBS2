using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace PiaNotes
{
    class MidiConverter
    {
        private byte[] data;       /** The entire midi file data */
        private int parse_offset;  /** The current offset while parsing */
            
        public MidiConverter(string filename)
        {
            FileInfo info = new FileInfo(filename);
            if (!info.Exists)
            {
                // doesn't exist
                
            }
            if (info.Length == 0)
            {
                // empty
            }
            FileStream file = File.Open(filename, FileMode.Open,
                                        FileAccess.Read, FileShare.Read);

            /* Read the entire file into memory */
            data = new byte[info.Length];
            int offset = 0;
            int len = (int)info.Length;
            while (true)
            {
                if (offset == info.Length)
                    break;
                int n = file.Read(data, offset, (int)(info.Length - offset));
                if (n <= 0)
                    break;
                offset += n;
            }
            parse_offset = 0;
            file.Close();
        }
    }
}
