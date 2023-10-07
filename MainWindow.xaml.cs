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
using PixelPaint.ViewModels;

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
        Rectangle rectangle = new Rectangle();
        Point startPoint = new Point();

        Ellipse leftUpEdit = new Ellipse();
        Ellipse leftDownEdit = new Ellipse();
        Ellipse rightUpEdit = new Ellipse();
        Ellipse rightDownEdit = new Ellipse();

        Rectangle frameEdit = new Rectangle();

        bool inEdition = false;



        public MainWindow()
        {
            InitializeComponent();

            leftUpEdit.Stroke = Brushes.LightGray;
            leftUpEdit.Fill = Brushes.MediumSeaGreen;
            leftUpEdit.Width = 20;
            leftUpEdit.Height = 20;
            workField.Children.Add(leftUpEdit);
            leftUpEdit.Visibility = Visibility.Hidden;

            leftDownEdit.Stroke = Brushes.LightGray;
            leftDownEdit.Fill = Brushes.MediumSeaGreen;
            leftDownEdit.Width = 20;
            leftDownEdit.Height = 20;
            workField.Children.Add(leftDownEdit);
            leftDownEdit.Visibility = Visibility.Hidden;

            rightUpEdit.Stroke = Brushes.LightGray;
            rightUpEdit.Fill = Brushes.MediumSeaGreen;
            rightUpEdit.Width = 20;
            rightUpEdit.Height = 20;
            workField.Children.Add(rightUpEdit);
            rightUpEdit.Visibility = Visibility.Hidden;

            rightDownEdit.Stroke = Brushes.LightGray;
            rightDownEdit.Fill = Brushes.MediumSeaGreen;
            rightDownEdit.Width = 20;
            rightDownEdit.Height = 20;
            workField.Children.Add(rightDownEdit);
            rightDownEdit.Visibility = Visibility.Hidden;

            
            frameEdit.Stroke = Brushes.DarkGray;
            frameEdit.StrokeThickness = 2;
            frameEdit.StrokeDashArray = new DoubleCollection() { 3, 6 };
            workField.Children.Add(frameEdit);
            frameEdit.Visibility = Visibility.Hidden;


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
                    line.X1 = positon.X;
                    line.Y1 = positon.Y;
                    line.Stroke = Brushes.Black;
                    workField.Children.Add(line);
                    break;

                case "ELLIPSE":
                    if (!inEdition)
                    {
                        ellipse.Stroke = Brushes.Black;
                        Canvas.SetLeft(ellipse, positon.X);
                        Canvas.SetTop(ellipse, positon.Y);
                        workField.Children.Add(ellipse);
                        inEdition = true;
                    }
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
                    if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        line.X2 = positon.X;
                        line.Y2 = positon.Y;
                        line.Fill = Brushes.Black;
                    }
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
                    break;

                case "ELLIPSE":
                    Canvas.SetLeft(frameEdit, Canvas.GetLeft(ellipse));
                    Canvas.SetTop(frameEdit, Canvas.GetTop(ellipse));
                    frameEdit.Width = ellipse.Width;
                    frameEdit.Height = ellipse.Height;
                    frameEdit.Visibility = Visibility.Visible;

                    Canvas.SetLeft(leftUpEdit, Canvas.GetLeft(ellipse) - 10);
                    Canvas.SetTop(leftUpEdit , Canvas.GetTop(ellipse) - 10);
                    leftUpEdit.Visibility = Visibility.Visible;

                    Canvas.SetLeft(rightUpEdit, Canvas.GetLeft(ellipse) + ellipse.Width - 10);
                    Canvas.SetTop(rightUpEdit, Canvas.GetTop(ellipse) - 10);
                    rightUpEdit.Visibility = Visibility.Visible;

                    Canvas.SetLeft(leftDownEdit, Canvas.GetLeft(ellipse) - 10);
                    Canvas.SetTop(leftDownEdit, Canvas.GetTop(ellipse)+ ellipse.Height - 10);
                    leftDownEdit.Visibility = Visibility.Visible;

                    Canvas.SetLeft(rightDownEdit, Canvas.GetLeft(ellipse) + ellipse.Width - 10);
                    Canvas.SetTop(rightDownEdit, Canvas.GetTop(ellipse) + ellipse.Height - 10);
                    rightDownEdit.Visibility = Visibility.Visible;

                    break;

                case "RECTANGLE":
                    
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
