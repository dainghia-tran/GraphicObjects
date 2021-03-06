using GraphicsLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicsLibrary
{
    public class Ellipse2D : IShape
    {
        public override string Name => "Ellipse2D";
        public override UIElement Draw()
        {
            double tWidth = End.X - Start.X;
            double tHeight = End.Y - Start.Y;
            var ellipse = new Ellipse()
            {
                Width = Math.Abs(tWidth),
                Height = Math.Abs(tHeight),
                StrokeThickness = StrokeThickness,
                Stroke = new SolidColorBrush(Color),
                StrokeDashArray = DoubleCollection.Parse(StrokeStyle),
            };
            if (tWidth > 0)
            {
                Canvas.SetLeft(ellipse, Start.X);
            }
            else
            {
                Canvas.SetLeft(ellipse, End.X);
            }
            if (tHeight > 0)
            {
                Canvas.SetTop(ellipse, Start.Y);

            }
            else
            {
                Canvas.SetTop(ellipse, End.Y);
            }
            return ellipse;
        }

        public override void HandleEnd(Point2D point)
        {
            End = new Point2D() { X = point.X, Y = point.Y };
        }

        public override void HandleShiftMode()
        {
            double tWidth = Math.Abs(End.X - Start.X);
            double tHeight = Math.Abs(End.Y - Start.Y);
            double diff = tWidth < tHeight ? tWidth : tHeight;

            if (End.X > Start.X && End.Y > Start.Y)
            {
                if (tWidth > tHeight)
                {
                    End.X = Start.X + diff;
                }
                else
                {
                    End.Y = Start.Y + diff;
                }
            }
            else if (End.X > Start.X && End.Y < Start.Y)
            {
                if (tWidth > tHeight)
                {
                    End.X = Start.X + diff;
                }
                else
                {
                    End.Y = Start.Y - diff;
                }
            }
            else if (End.X < Start.X && End.Y > Start.Y)
            {
                if (tWidth > tHeight)
                {
                    End.X = Start.X - diff;
                }
                else
                {
                    End.Y = Start.Y + diff;
                }
            }
            else
            {
                if (tWidth > tHeight)
                {
                    End.X = Start.X - diff;
                }
                else
                {
                    End.Y = Start.Y - diff;
                }
            }
        }

        public override void HandleStart(Point2D point)
        {
            Start = new Point2D() { X = point.X, Y = point.Y };
        }
    }
}
