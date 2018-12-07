using Microsoft.Graphics.Canvas;
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
    class Note : IGameObject
    {
        public int number { get; set; }
        public int timing { get; set; }
        public int length { get; set; }

        public CanvasBitmap Bitmap { get; set; }
        public Vector2 Location { get; set; }
        public BitmapSize Size { get; set; }

        //Creating a note can only happen if you know the number, timing and the length.
        public Note(int n, int t, int l)
        {
            number = n;
            timing = t;
            length = l;
        }

        public void Draw(CanvasDrawingSession cds)
        {
            if (Bitmap == null)
                return;

            cds.DrawImage(Bitmap, Location);
        }

        public bool SetBitmap(string key)
        {
            CanvasBitmap cb = null;
            if (ContentPipeline.ImageDictionary.TryGetValue(key, out cb))
            {
                this.Bitmap = cb;
                this.Size = this.Bitmap.SizeInPixels;
                return true;
            }

            return false;
        }

        public void SetSize(uint width, uint height)
        {
            BitmapSize new_size;
            new_size.Width = width;
            new_size.Height = height;
            this.Size = new_size;
        }
    }
}
