using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace PiaNotes
{
    class MidiParser
    {
        private byte[] bytes;
        private int offset;

        public MidiParser(byte[] file)
        {
            bytes = file;
        }

        public string ReadHeaderChunk() {
            string chunkType = Encoding.ASCII.GetString(bytes, 0, 4);
            offset += 4;

            if (chunkType != "MThd")
                return null;

            //int dataLength = BitConverter.ToInt32(bytes, offset);
            int dataLength = BitConverter.ToInt32(bytes, offset);
            //int dataLength = Int32.Parse(Encoding.UTF32.GetString(bytes, offset, 6));
            offset += 4;

            //string data = Encoding.Unicode.GetString(bytes, offset, dataLength);

            return "nut";
        }

    }
}
