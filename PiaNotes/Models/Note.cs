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
                Size = this.Bitmap.SizeInPixels;
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
