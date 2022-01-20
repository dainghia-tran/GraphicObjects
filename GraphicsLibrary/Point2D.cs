using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GraphicsLibrary
{
    public class Point2D : IShape
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public override string Name { get => "Point2D"; set => throw new NotImplementedException(); }
        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2D(Point2D src)
        {
            X = src.X;
            Y = src.Y;
        }

        public override void HandleStart(Point2D point)
        {
            X = point.X;
            Y = point.Y;
        }

        public override void HandleEnd(Point2D point)
        {
            X = point.X + 1;
            Y = point.Y + 1;
        }

        public override UIElement Draw()
        {
            return null;
        }

        public override void HandleShiftMode()
        {
        }
    }
}
