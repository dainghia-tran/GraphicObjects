using GraphicsLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        bool isDrawing;
        ShapeType currentShapeType = ShapeType.Ellipse2D;
        List<IShape> shapes = new List<IShape>();
        Dictionary<int, List<Image>> images = new Dictionary<int, List<Image>>();
        IShape preview;
        Color selectedColor = DrawOptions.Color;

        private double thickness;
        public double Thickness
        {
            get { return thickness; }
            set
            {
                thickness = value;
                OnPropertyChanged("Thickness");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DllLoader.execute();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
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
            preview = (IShape)GetInstance($"{currentShapeType}");
            preview.StrokeThickness = thickness;
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
                preview.Color = selectedColor;
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

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            selectedColor = (Color)ColorPicker.SelectedColor;
        }

        private void penThicknessChooser_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Thickness = (double)penThicknessChooser.Value;
        }
    }
}
