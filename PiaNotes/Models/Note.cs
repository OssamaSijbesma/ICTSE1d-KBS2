using Melanchall.DryWetMidi.Smf.Interaction;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using PiaNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace PiaNotes.Models
{
    public class Note
    {
        public int Number { get; set; }
        public int Timing { get; set; }
        public int Length { get; set; }
        public double NoteType { get; set; }
        public MetricTimeSpan MetricTiming { get; set; }

        public bool Active { get; set; }
        public bool Played { get; set; }

        public CanvasBitmap Bitmap { get; set; }
        public Vector2 BitmapLocation { get; set; }
        public Size BitmapSize { get; set; }

        //Creating a note can only happen if you know the number, timing and the length.
        public Note(int number, int timing, int length,  MetricTimeSpan metricTiming, double roundedLength)
        {
            Number = number;
            Timing = timing;
            Length = length;
            MetricTiming = metricTiming;
            NoteType = roundedLength;
            Active = false;
        }

        public bool SetBitmap(string key)
        {
            CanvasBitmap canvasBitmap = null;
            if (ContentPipeline.ImageDictionary.TryGetValue(key, out canvasBitmap))
            {
                Bitmap = canvasBitmap;
                BitmapSize bitmapSize = Bitmap.SizeInPixels;
                BitmapSize = new Size(bitmapSize.Height, bitmapSize.Width);
                return true;
            }
            return false;
        }
    }
}
