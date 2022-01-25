using GraphicsLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
        List<IShape> shapes = new List<IShape>();
        Dictionary<int, List<Image>> images = new Dictionary<int, List<Image>>();
        IShape preview;
        Color selectedColor = DrawOptions.Color;
        bool onShift = false;
        bool onCtrl = false;
        List<IShape> history = new List<IShape>();
        string strokeStyle = DrawOptions.StrokeStyle;

        private ShapeType currentShapeType = ShapeType.Line2D;
        public ShapeType CurrentShapeType
        {
            get { return currentShapeType; }
            set
            {
                currentShapeType = value;
                OnPropertyChanged("CurrentShapeType");
            }
        }

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

            KeyDown += new KeyEventHandler(OnButtonKeyDown);
            KeyUp += new KeyEventHandler(OnButtonKeyUp);
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
                if (onShift)
                {
                    preview.HandleShiftMode();
                }
                preview.StrokeStyle = strokeStyle;

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
                if (onShift)
                {
                    preview.HandleShiftMode();
                }
                preview.StrokeStyle = strokeStyle;

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

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentShapeType = ShapeType.Line2D;
        }

        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentShapeType = ShapeType.Rectangle2D;
        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentShapeType = ShapeType.Ellipse2D;
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (shapes.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("You will lose all unsaved work\nAre you sure to create a new painting", "Waring", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        shapes.Clear();
                        DrawingCanvas.Children.Clear();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void OpenDraft_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Load preset";
            openFileDialog.DefaultExt = "json";
            openFileDialog.Filter = "Json files (*.json)|*.json";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string jsonString = File.ReadAllText(openFileDialog.FileName);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                List<Dictionary<string, object>> savedShapes = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString, options);
                foreach (var element in savedShapes)
                {
                    string shapeName = element["Name"].ToString();
                    switch (shapeName)
                    {
                        case "Line2D":
                            shapes.Add(JsonSerializer.Deserialize<Line2D>(JsonSerializer.Serialize(element), options));
                            break;
                        case "Rectangle2D":
                            shapes.Add(JsonSerializer.Deserialize<Rectangle2D>(JsonSerializer.Serialize(element), options));
                            break;
                        case "Ellipse2D":
                            shapes.Add(JsonSerializer.Deserialize<Ellipse2D>(JsonSerializer.Serialize(element), options));
                            break;
                    }
                }
                redraw();
            };
        }

        private void SaveDraft_Click(object sender, RoutedEventArgs e)
        {
            if (shapes.Count > 0)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Json files (*.json)|*.json";
                saveFileDialog.Title = "Save draft";
                saveFileDialog.RestoreDirectory = true;
                Nullable<bool> result = saveFileDialog.ShowDialog();
                if (result == true)
                {
                    String fileName = saveFileDialog.FileName;
                    string jsonString = JsonSerializer.Serialize(shapes);
                    string beautified = JToken.Parse(jsonString).ToString(Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(fileName, beautified);
                    MessageBox.Show($@"Saved to {saveFileDialog.FileName}", "Project Paint", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Your work is empty", "Project Paint", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Images|*.png";
            saveFileDialog.Title = "Save as PNG";
            saveFileDialog.RestoreDirectory = true;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string fileName = saveFileDialog.FileName;
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
                DrawingCanvas.Measure(new Size((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight));
                DrawingCanvas.Arrange(new Rect(new Size((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight)));

                renderBitmap.Render(DrawingCanvas);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (FileStream file = File.Create(fileName))
                {
                    encoder.Save(file);
                }
            }
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Open image";
            openFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                preview = null;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fileName, UriKind.Absolute);
                bitmap.EndInit();

                Image image = new Image();
                image.Source = bitmap;
                image.Width = bitmap.Width;
                image.Height = bitmap.Height;
                if (bitmap.Width > DrawingCanvas.Width || double.IsNaN(DrawingCanvas.Width))
                {
                    DrawingCanvas.Width = bitmap.Width > DrawingCanvas.ActualWidth ? bitmap.Width : double.NaN;
                }
                if (bitmap.Height > DrawingCanvas.Height || double.IsNaN(DrawingCanvas.Height))
                {
                    DrawingCanvas.Height = bitmap.Height > DrawingCanvas.ActualHeight ? bitmap.Height : double.NaN;
                }
                if (!images.ContainsKey(shapes.Count))
                {
                    images[shapes.Count] = new List<Image>();
                }
                images[shapes.Count].Add(image);
                DrawingCanvas.Children.Add(image);
            };
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                onShift = true;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                onCtrl = true;
            }
            else if (onCtrl && e.Key == Key.Z)
            {
                onUndo();
            }
            else if (onCtrl && e.Key == Key.Y)
            {
                onRedo();
            }
        }
        private void OnButtonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                onShift = false;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                onCtrl = false;
            }
        }

        private void onUndo()
        {
            if (images.ContainsKey(0) && shapes.Count == 0)
            {
                if (images[0].Count > 0)
                {
                    images[0].RemoveAt(images[0].Count - 1);
                    DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
                }
            }
            if (shapes.Count > 0)
            {
                if (images.ContainsKey(shapes.Count) && images[shapes.Count].Count > 0)
                {
                    images[shapes.Count].RemoveAt(images[shapes.Count].Count - 1);
                }
                else
                {
                    history.Add(shapes[shapes.Count - 1]);
                    shapes.RemoveAt(shapes.Count - 1);
                }
                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
            }
        }

        private void onRedo()
        {
            if (history.Count > 0)
            {
                shapes.Add(history[history.Count - 1]);
                DrawingCanvas.Children.Add(shapes[shapes.Count - 1].Draw());
                history.RemoveAt(history.Count - 1);
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            onUndo();
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            onRedo();
        }

        private void StrokeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StrokeType.SelectedItem;
            string value = selectedItem.Tag.ToString();

            if (value.Equals("Default"))
            {
                strokeStyle = DrawOptions.StrokeStyle;
            }
            else
            {
                strokeStyle = value;
            }
        }
    }
}
