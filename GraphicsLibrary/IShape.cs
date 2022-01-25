using System.Windows;
using System.Windows.Media;

namespace GraphicsLibrary
{
    public abstract class IShape
    {
        public virtual string Name { get; set; }
        public double StrokeThickness { get; set; } = DrawOptions.Thickness;
        public Point2D Start { get; set; }
        public Point2D End { get; set; }
        public Color Color { get; set; } = DrawOptions.PreviewColor;
        public string StrokeStyle { get; set; } = "";

        public abstract void HandleStart(Point2D point);
        public abstract void HandleEnd(Point2D point);
        public abstract void HandleShiftMode();
        public abstract UIElement Draw();
    }
}
