using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphicsLibrary
{
    public abstract class IShape
    {
        public virtual string Name { get; set; }
        public double StrokeThickness { get; set; } = DrawOptions.thickness;
        public Color Color { get; set; } = DrawOptions.previewColor;
        public string DashStyle { get; set; } = "";
        public abstract void HandleStart(Point2D point);
        public abstract void HandleEnd(Point2D point);

        public abstract void HandleShiftMode();

        public abstract UIElement Draw();
    }
}
