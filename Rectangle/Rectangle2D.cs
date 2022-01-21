using GraphicsLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rectangle2D
{
    public class Rectangle2D : IShape
    {
        public Point2D Start = new Point2D();
        public Point2D End = new Point2D();
        public new string Name { get => "Rectangle2D"; set => throw new NotImplementedException(); }
        public override UIElement Draw()
        {
            double tWidth = End.X - Start.X;
            double tHeight = End.Y - Start.Y;
            var rect = new Rectangle()
            {
                Width = Math.Abs(tWidth),
                Height = Math.Abs(tHeight),
                StrokeThickness = StrokeThickness,
                Stroke = new SolidColorBrush(Color),
                StrokeDashArray = DoubleCollection.Parse(DashStyle),
            };
            if (tWidth > 0)
            {
                Canvas.SetLeft(rect, Start.X);
            }
            else
            {
                Canvas.SetLeft(rect, End.X);
            }
            if (tHeight > 0)
            {
                Canvas.SetTop(rect, Start.Y);

            }
            else
            {
                Canvas.SetTop(rect, End.Y);
            }
            return rect;
        }

        public override void HandleEnd(Point2D point)
        {
            End = new Point2D() { X = point.X, Y = point.Y };
        }

        public override void HandleShiftMode()
        {
            throw new NotImplementedException();
        }

        public override void HandleStart(Point2D point)
        {
            Start = new Point2D() { X = point.X, Y = point.Y };
        }
    }
}
