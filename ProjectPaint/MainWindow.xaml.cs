using GraphicsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isDrawing;
        ShapeType currentShapeType = ShapeType.Rectangle2D;
        List<IShape> shapes = new List<IShape>();
        Dictionary<int, List<Image>> images = new Dictionary<int, List<Image>>();
        List<IShape> redos = new List<IShape>();
        IShape preview;

        public MainWindow()
        {
            InitializeComponent();
            DllLoader.execute();
        }

        public object GetInstance(string strFullyQualifiedName)
        {
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
                return Activator.CreateInstance(type);
            var types = DllLoader.Types;
            foreach (var t in types)
            {
                if (t.Name == strFullyQualifiedName)
                    return Activator.CreateInstance(t);
            }
            return null;
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            redos.Clear();
            preview = (IShape)GetInstance($"{currentShapeType}");
            isDrawing = true;
            Point currentCoordinate = e.GetPosition(DrawingCanvas);
            preview.HandleStart(new Point2D(currentCoordinate.X, currentCoordinate.Y));
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Point current = e.GetPosition(DrawingCanvas);

                Point2D newPoint = new Point2D(current.X, current.Y);
                preview.HandleEnd(newPoint);
                DrawingCanvas.Children.Clear();
                redraw();
                preview.Color = DrawOptions.PreviewColor;
                DrawingCanvas.Children.Add(preview.Draw());
            }
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            Point current = e.GetPosition(DrawingCanvas);
            Point2D newPoint = new Point2D(current.X, current.Y);
            if (preview != null)
            {
                preview.Color = DrawOptions.Color;
                preview.HandleEnd(newPoint);
                shapes.Add(preview);
                DrawingCanvas.Children.Clear();
                redraw();
            }
        }

        private void redraw()
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (images.ContainsKey(i))
                {
                    foreach (var image in images[i])
                    {
                        DrawingCanvas.Children.Add(image);
                    }
                }
                var element = shapes[i].Draw();
                DrawingCanvas.Children.Add(element);
            }
        }
    }
}
