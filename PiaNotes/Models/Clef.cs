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
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace PiaNotes.Models
{
    public class Clef
    {
        public enum ClefTypes {Treble, Bass, Alto, Tenor}

        public ClefTypes CT;

        public CanvasBitmap Bitmap { get; set; }
        public Vector2 BitmapLocation { get; set; }
        public Size BitmapSize { get; set; }

        public Clef(Vector2 location)
        {
            BitmapLocation = location;
        }

        public async Task<bool> SetBitmap(CanvasControl ParentCanvas, string file_path)
        {
            // null check
            if (ParentCanvas == null)
                return false;

            Bitmap = await CanvasBitmap.LoadAsync(ParentCanvas, file_path);
            BitmapSize bitmapSize = Bitmap.SizeInPixels;
            BitmapSize = new Size(bitmapSize.Height/7, bitmapSize.Width/7);
            return true;
        }
    }
}
