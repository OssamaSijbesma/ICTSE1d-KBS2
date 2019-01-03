using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.Models
{
    public class Line
    {
        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }

        public Line(Vector2 startPoint, Vector2 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Line(int startX, int startY, int endX, int endY)
        {
            StartPoint = new Vector2(startX, startY);
            EndPoint = new Vector2(endX, endY);
        }
    }
}
