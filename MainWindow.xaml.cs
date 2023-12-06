using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.Win32;
using Newtonsoft.Json;
using PixelPaint.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PixelPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polygon polygonSaved = new Polygon();

        Polygon polygon = new Polygon();
        PointCollection points = new PointCollection();

        Point lastPoint = new Point();
        Point actualPoint = new Point();
        Point beginningPoint = new Point();

        string state = "PENCIL";
        Line line = new Line();
        Ellipse ellipse = new Ellipse();
        Rectangle rectangle = new Rectangle();
        Point startPoint = new Point();

        ResizeAdorner adornerToRemove;

        private Point centerPoint;

        bool inEdition = false;

        Button newButton = new Button();


        private bool isDragging;
        public bool createPolyClicked = false;
        private Point startPointDrag;

        private bool isRotating;
        private Point lastPointRotation;
        private double lastAngle;


        public MainWindow()
        {
            InitializeComponent();

            upDownX.Text = "0"; 
            upDownY.Text = "0";
            upDownRotation.Text = "0";

            upDownScale.Text = "100";

            polygon.MouseLeftButtonDown += Polygon_MouseLeftButtonDown;
            polygon.MouseLeftButtonUp += Polygon_MouseLeftButtonUp;
            polygon.MouseMove += Polygon_MouseMove;

            TransformGroup transformGroup = new TransformGroup();
            TranslateTransform translation = new TranslateTransform();
            RotateTransform rotation = new RotateTransform();
            transformGroup.Children.Add(translation);
            transformGroup.Children.Add(rotation);
            polygon.RenderTransform = transformGroup;

            /*heightTextBox.TextChanged += heightTextBox_TextChanged;
            widthTextBox.TextChanged += widthTextBox_TextChanged;*/
            /*ellipse.SizeChanged += elipseSizeChange;
            rectangle.SizeChanged += rectangleSizeChanged;*/
        }

        /*private void rectangleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            heightTextBox.Text = rectangle.Height.ToString();
            widthTextBox.Text = rectangle.Width.ToString();
        }

        private void elipseSizeChange(object sender, SizeChangedEventArgs e)
        {
                heightTextBox.Text = ellipse.Height.ToString();
                widthTextBox.Text = ellipse.Width.ToString();
        }*/


        /*private void widthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number;
            switch (state)
            {
                case "ELLIPSE":
                    if (int.TryParse(widthTextBox.Text, out number)) ellipse.Width = number;
                    break;
                case "RECTANGLE":
                    if (int.TryParse(widthTextBox.Text, out number)) rectangle.Width = number;
                    break;

            }
        }
        private void heightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number;
            switch (state)
            {
                case "ELLIPSE":
                    if(int.TryParse(heightTextBox.Text, out number)) ellipse.Height = number;
                    break;
                case "RECTANGLE":
                    if (int.TryParse(heightTextBox.Text, out number)) rectangle.Height = number;
                    break;

            }
        }*/

        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            polygon.CaptureMouse();
            isRotating = true;
            isDragging = true;
            lastPointRotation = e.GetPosition(this);
            centerPoint = GetPolygonCenter(polygon);
            lastAngle = CalculateAngle(centerPoint, e.GetPosition(this));

        }

        private void Polygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            polygon.ReleaseMouseCapture();
            isDragging = false;
            isRotating = false;

        }

        private void Polygon_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && Keyboard.IsKeyDown(Key.LeftShift))
            {
                Point currentPosition = e.GetPosition(this);
                Vector movement = currentPosition - lastPointRotation;

                TransformGroup transformGroup = polygon.RenderTransform as TransformGroup;
                if (transformGroup != null)
                {
                    TranslateTransform translation = transformGroup.Children[0] as TranslateTransform;
                    if (translation != null)
                    {
                        translation.X += movement.X;
                        translation.Y += movement.Y;
                    }
                }

                lastPointRotation = currentPosition;
            }
            else if (isRotating && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Point currentPoint = e.GetPosition(this);
                double currentAngle = CalculateAngle(centerPoint, currentPoint);

                double angle = lastAngle - currentAngle;

                TransformGroup transformGroup = polygon.RenderTransform as TransformGroup;
                if (transformGroup != null)
                {
                    RotateTransform rotation = transformGroup.Children[1] as RotateTransform;
                    if (rotation != null)
                    {
                        rotation.CenterX = centerPoint.X;
                        rotation.CenterY = centerPoint.Y;
                        rotation.Angle -= angle;
                    }
                }

                lastAngle = currentAngle;
            }
        }

        private double CalculateAngle(Point center, Point point)
        {
            Vector vector = point - center;
            return Math.Atan2(vector.Y, vector.X) * (180 / Math.PI);
        }
        private Point GetPolygonCenter(Polygon poly)
        {
            Point center = new Point();
            foreach (Point point in poly.Points)
            {
                center.X += point.X;
                center.Y += point.Y;
            }
            center.X /= poly.Points.Count;
            center.Y /= poly.Points.Count;

            return center;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var positon = e.GetPosition(workField);
            startPoint = positon;
            switch (state)
            {

                case "POLYGON":
                    if (!inEdition && !Keyboard.IsKeyDown(Key.LeftCtrl) && createPolyClicked)
                    {
                        inEdition = true;
                        beginningPoint = positon;
                        newButton.Content = "end poly";
                        newButton.Click += ButtonPolygonEnd;
                        Canvas.SetLeft(newButton, positon.X);
                        Canvas.SetTop(newButton, positon.Y );
                        workField.Children.Add(newButton);
                        polygon.Stroke = Brushes.Black;
                        workField.Children.Add(polygon);
                    }
                    if (createPolyClicked)
                    {
                        lastPoint = actualPoint;
                        actualPoint = positon;
                        points.Add(actualPoint);
                        polygon.Points = points;
                    }
                    
                    break;

                case "PENCIL":
                    var pixel = new Rectangle();
                    pixel.Fill = Brushes.Black;
                    pixel.Width = 1;
                    pixel.Height = 1;
                    workField.Children.Add(pixel);
                    Canvas.SetLeft(pixel, positon.X);
                    Canvas.SetTop(pixel, positon.Y);
                    break;

                case "LINE":
                    if (!inEdition)
                    {
                        line.X1 = positon.X;
                        line.Y1 = positon.Y;
                        line.Stroke = Brushes.Black;
                        workField.Children.Add(line);
                    }
                    break;

                case "ELLIPSE":
                    if (!inEdition)
                    {
                        ellipse.Stroke = Brushes.Black;
                        ellipse.VerticalAlignment = VerticalAlignment.Center;
                        ellipse.HorizontalAlignment = HorizontalAlignment.Center;
                        Canvas.SetLeft(ellipse, positon.X);
                        Canvas.SetTop(ellipse, positon.Y);
                        workField.Children.Add(ellipse);
                    }
                    
                    break;
                case "RECTANGLE":
                    if (!inEdition)
                    {
                        rectangle.Stroke = Brushes.Black;
                        Canvas.SetLeft(rectangle, positon.X);
                        Canvas.SetTop(rectangle, positon.Y);
                        workField.Children.Add(rectangle);
                    }
                    break;
                default: break;
            }
            
        }

        private void workField_MouseMove(object sender, MouseEventArgs e)
        {
            var positon = e.GetPosition(workField);
            switch (state)
            {
                case "PENCIL":
                    if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        var pixel = new Rectangle();
                        pixel.Fill = Brushes.Black;
                        pixel.Width = 1;
                        pixel.Height = 1;
                        workField.Children.Add(pixel);
                        Canvas.SetLeft(pixel, positon.X);
                        Canvas.SetTop(pixel, positon.Y);
                    }
                    break;

                case "LINE":
                    if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed && !inEdition)
                    {
                        line.X2 = positon.X;
                        line.Y2 = positon.Y;
                        line.Fill = Brushes.Black;
                    }
                    break;

                case "ELLIPSE":
                    if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed && !inEdition)
                    {
                        ellipse.Width = Math.Abs(startPoint.X - positon.X);
                        ellipse.Height = Math.Abs(startPoint.Y - positon.Y);
                    }
                    break;

                case "RECTANGLE":
                    if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed && !inEdition)
                    {
                        rectangle.Width = Math.Abs(startPoint.X - positon.X);
                        rectangle.Height = Math.Abs(startPoint.Y - positon.Y);
                    }
                    break;
                default: break;
            }

        }
        private void workField_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var positon = e.GetPosition(workField);
            switch (state)
            {
                case "LINE":
                    if (inEdition)
                    {
                        inEdition = false;
                        line = new Line();
                    }
                    else
                    {
                        inEdition = true;   
                    }
                    break;

                case "ELLIPSE":
                    if (inEdition)
                    {
                        inEdition = false;
                        AdornerLayer adonerLayer = AdornerLayer.GetAdornerLayer(workField);
                        if(adonerLayer != null)adonerLayer.Remove(adornerToRemove);
                        ellipse = new Ellipse();
                        /*heightTextBox.Text = "";
                        widthTextBox.Text = "";*/
                        /*ellipse.SizeChanged += elipseSizeChange;*/
                    }
                    else
                    {
                        adornerToRemove = new ResizeAdorner(ellipse);
                        inEdition = true;
                        AdornerLayer.GetAdornerLayer(workField).Add(adornerToRemove);
                        /*heightTextBox.Text = ellipse.Height.ToString();
                        widthTextBox.Text = ellipse.Width.ToString();*/
                    }
                    break;

                case "RECTANGLE":
                    if (inEdition)
                    {
                        inEdition = false;
                        AdornerLayer adonerLayer = AdornerLayer.GetAdornerLayer(workField);
                        if (adonerLayer != null) adonerLayer.Remove(adornerToRemove);
                        rectangle = new Rectangle();
                        /*heightTextBox.Text = "";
                        widthTextBox.Text = "";*/
                        /*rectangle.SizeChanged += rectangleSizeChanged;*/
                    }
                    else
                    {
                        adornerToRemove = new ResizeAdorner(rectangle);
                        inEdition = true;
                        AdornerLayer.GetAdornerLayer(workField).Add(adornerToRemove);
                    }
                    break;

                default: break;

            }
        }

        private void ButtonPolygonEnd(object sender, RoutedEventArgs e)
        {
            

            createPolyClicked = false;
            polygon.Fill = Brushes.Gray;

            polygonSaved = polygon;


            points = new PointCollection();

            workField.Children.Remove(newButton);
            

            inEdition = false;

            lastPoint = new Point();
            actualPoint = new Point();
            beginningPoint = new Point();

            
        }

        private void ButtonLineClicked(object sender, RoutedEventArgs e)
        {
            state = "LINE";
        }

        private void ButtonElipseClicked(object sender, RoutedEventArgs e)
        {
            state = "ELLIPSE";
            
        }

        private void ButtonRectangleClicked(object sender, RoutedEventArgs e)
        {
            state = "RECTANGLE";
        }

        private void ButtonPencilClicked(object sender, RoutedEventArgs e)
        {
            state = "PENCIL";
        }

        private void ButtonPolygonClicked(object sender, RoutedEventArgs e)
        {
            state = "POLYGON";
            createPolyClicked = true;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                string mystrXAML = "";
                
                foreach ( var x in workField.Children)
                {
                    mystrXAML += XamlWriter.Save(x);
                }
                FileStream filestream = File.Create(saveFileDialog.FileName);
                StreamWriter streamwriter = new StreamWriter(filestream);
                streamwriter.Write(mystrXAML);
                streamwriter.Close();
                filestream.Close();
            }

           


            
                
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PPM files (*.ppm)|*.ppm";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                LoadAndDisplayPPMImage(filePath);
            }

        }
        private void LoadAndDisplayPPMImage(string filePath)
        {
            // Odczytaj całą zawartość pliku PPM P3
            string[] lines = File.ReadAllLines(filePath);

            // Przetwórz nagłówek, pierwsza linia powinna być "P3"
            if (lines.Length > 0 && lines[0] == "P3")
            {
                int width, height, maxColorValue;
                if (lines.Length >= 4 &&
                    int.TryParse(lines[1], out width) &&
                    int.TryParse(lines[2], out height) &&
                    int.TryParse(lines[3], out maxColorValue))
                {
                    // Przygotuj tablicę kolorów
                    Color[] colors = new Color[width * height];

                    int colorIndex = 0;
                    for (int i = 4; i < lines.Length; i++)
                    {
                        string[] colorValues = lines[i].Split(' ', '\t');
                        foreach (string colorValue in colorValues)
                        {
                            if (int.TryParse(colorValue, out int color))
                            {
                                int normalizedColor = (int)((color / (float)maxColorValue) * 255);
                                colors[colorIndex] = Color.FromRgb((byte)normalizedColor, (byte)normalizedColor, (byte)normalizedColor);
                                colorIndex++;
                            }
                        }
                    }

                    // Tworzenie bitmapy
                    BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, colors, width * 4);

                    // Wyświetlenie bitmapy w kontrolce Image

                    Image image = new Image();
                    image.Source = bitmap;
                    workField.Children.Add(image);
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowy format pliku PPM P3.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        
        private void SetPolygonPosition(double x, double y)
        {
            Canvas.SetLeft(polygon, x);
            Canvas.SetTop(polygon, y);
        }

        private void upDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (double.TryParse(upDownX.Text, out double newX) && double.TryParse(upDownY.Text, out double newY))
            {
                SetPolygonPosition(newX, newY);
            }
        }

        private void upDownRotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (double.TryParse(upDownRotation.Text, out double newAngle))
            {
                RotatePolygon(newAngle);
            }
        }
        private void RotatePolygon(double angle)
        {
            RotateTransform rotateTransform = new RotateTransform(angle, polygon.ActualWidth / 2, polygon.ActualHeight / 2);
            polygon.RenderTransform = rotateTransform;
        }

        private void upDownScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (double.TryParse(upDownScale.Text, out double scaleFactor) && scaleFactor != 0)
            {
                ScalePolygon(scaleFactor/100);
            }
        }
        private void ScalePolygon(double scaleFactor)
        {
            ScaleTransform scaleTransform = new ScaleTransform(scaleFactor, scaleFactor);
            polygon.RenderTransform = scaleTransform;
        }
    }
}
