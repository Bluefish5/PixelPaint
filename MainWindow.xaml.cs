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
        string state = "PENCIL";
        Line line = new Line();
        Ellipse ellipse = new Ellipse();
        Rectangle rectangle = new Rectangle();
        Point startPoint = new Point();

        ResizeAdorner adornerToRemove;

        bool inEdition = false;



        public MainWindow()
        {
            InitializeComponent();
            heightTextBox.TextChanged += heightTextBox_TextChanged;
            widthTextBox.TextChanged += widthTextBox_TextChanged;
            ellipse.SizeChanged += elipseSizeChange;
            rectangle.SizeChanged += rectangleSizeChanged;
        }

        private void rectangleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            heightTextBox.Text = rectangle.Height.ToString();
            widthTextBox.Text = rectangle.Width.ToString();
        }

        private void elipseSizeChange(object sender, SizeChangedEventArgs e)
        {
                heightTextBox.Text = ellipse.Height.ToString();
                widthTextBox.Text = ellipse.Width.ToString();
        }

        
        private void widthTextBox_TextChanged(object sender, TextChangedEventArgs e)
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
                        heightTextBox.Text = "";
                        widthTextBox.Text = "";
                        ellipse.SizeChanged += elipseSizeChange;
                    }
                    else
                    {
                        adornerToRemove = new ResizeAdorner(ellipse);
                        inEdition = true;
                        AdornerLayer.GetAdornerLayer(workField).Add(adornerToRemove);
                        heightTextBox.Text = ellipse.Height.ToString();
                        widthTextBox.Text = ellipse.Width.ToString();
                    }
                    break;

                case "RECTANGLE":
                    if (inEdition)
                    {
                        inEdition = false;
                        AdornerLayer adonerLayer = AdornerLayer.GetAdornerLayer(workField);
                        if (adonerLayer != null) adonerLayer.Remove(adornerToRemove);
                        rectangle = new Rectangle();
                        heightTextBox.Text = "";
                        widthTextBox.Text = "";
                        rectangle.SizeChanged += rectangleSizeChanged;
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
            if (openFileDialog.ShowDialog() == true)
            {
                
                string fielpath = openFileDialog.FileName;

                
            }

        }

    }
}
