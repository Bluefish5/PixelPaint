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

namespace PixelPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string state = "PENCIL";
        Line line = new Line();
        Ellipse ellipse = new Ellipse();
        Polygon rectangle = new Polygon();
        Point startPoint = new Point();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var positon = e.GetPosition(workField);
            startPoint = positon;
            switch (state)
            {
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
                    line.Width = 1;
                    line.X1 = positon.X;
                    line.Y1 = positon.Y;
                    Canvas.SetLeft(line, positon.X);
                    Canvas.SetTop(line, positon.Y);
                    break;

                case "ELLIPSE":
                    ellipse.Stroke = Brushes.Black;
                    Canvas.SetLeft(ellipse, positon.X);
                    Canvas.SetTop(ellipse, positon.Y);
                    workField.Children.Add(ellipse);
                    break;
                case "RECTANGLE":
                    rectangle.Stroke = Brushes.Black;
                    Canvas.SetLeft(rectangle, positon.X);
                    Canvas.SetTop(rectangle, positon.Y);
                    workField.Children.Add(rectangle);
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
                    line.X2 = positon.X;
                    line.Y2 = positon.Y;
                    line.Fill = Brushes.Black;
                    break;

                case "ELLIPSE":
                    if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed){
                        ellipse.Width = Math.Abs(startPoint.X - positon.X);
                        ellipse.Height = Math.Abs(startPoint.Y - positon.Y);
                    }
                    break;

                case "RECTANGLE":
                    if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
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
                    line.StrokeThickness = 100;
                    line.Stroke = Brushes.Black;
                    workField.Children.Add(line);
                    line = new Line();
                    break;

                case "ELLIPSE":
                    ellipse = new Ellipse();
                    break;

                case "RECTANGLE":
                    rectangle = new Polygon();
                break;

                default: break;

            }
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

        
    }
}
