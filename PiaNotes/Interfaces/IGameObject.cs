using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace PiaNotes.Interfaces
{
    interface IGameObject
    {
        CanvasBitmap Bitmap { get; set; }
        Vector2 Location { get; set; }
        BitmapSize Size { get; set; }

        bool SetBitmap(string key);
        void SetSize(uint width, uint height);
    }
}
