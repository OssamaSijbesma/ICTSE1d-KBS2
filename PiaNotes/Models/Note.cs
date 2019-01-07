using Melanchall.DryWetMidi.Smf.Interaction;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using PiaNotes.Interfaces;
using PiaNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace PiaNotes.Models
{
    public class Note : IGameObject
    {
        public int Number { get; set; }
        public int Timing { get; set; }
        public int Length { get; set; }
        public double NoteType { get; set; }
        public MetricTimeSpan MetricTiming { get; set; }

        public CanvasBitmap Bitmap { get; set; }
        public Vector2 Location { get; set; }
        public BitmapSize Size { get; set; }


        //Creating a note can only happen if you know the number, timing and the length.
        public Note(int number, int timing, int length,  MetricTimeSpan metricTiming, double roundedLength)
        {
            Number = number;
            Timing = timing;
            Length = length;
            MetricTiming = metricTiming;
            NoteType = roundedLength;
        }

        public void Draw(CanvasControl cC)
        {
            if (Bitmap == null)
                return;

            //cC.DrawingSession.DrawImage(Bitmap, Location);
        }

        public bool SetBitmap(string key)
        {
            CanvasBitmap canvasBitmap = null;
            if (ContentPipeline.ImageDictionary.TryGetValue(key, out canvasBitmap))
            {
                Bitmap = canvasBitmap;
                Size = Bitmap.SizeInPixels;
                return true;
            }
            return false;
        }

        public void SetSize(uint width, uint height)
        {
            BitmapSize bitmapSize;
            bitmapSize.Width = width;
            bitmapSize.Height = height;
            Size = bitmapSize;
        }
    }
}
