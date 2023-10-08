using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace PixelPaint
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection AdornerVisuals;
        
        Thumb thumb1, thumb2;
        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(adornedElement);
           

            thumb1 = new Thumb() { Background = Brushes.ForestGreen,Height = 20,Width = 20};
            thumb2 = new Thumb() { Background = Brushes.ForestGreen, Height = 20, Width = 20 };

            thumb1.DragDelta += Thumb1_DragDelta;
            thumb2.DragDelta += Thumb2_DragDelta;

            AdornerVisuals.Add(thumb1);
            AdornerVisuals.Add(thumb2);
            

        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;
            element.Height = element.Height + e.VerticalChange < 0 ? 0 : element.Height + e.VerticalChange;
            element.Width = element.Width + e.HorizontalChange < 0 ? 0 : element.Width + e.HorizontalChange;
            /*Canvas.SetLeft(element, Canvas.GetLeft(element) - e.HorizontalChange);
            Canvas.SetTop(element, Canvas.GetTop(element) - e.VerticalChange);*/
        }

        private void Thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;
            element.Height = element.Height - e.VerticalChange < 0 ? 0 : element.Height - e.VerticalChange;
            element.Width = element.Width - e.HorizontalChange < 0 ? 0 : element.Width - e.HorizontalChange;

            Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
            Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);



        }

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            thumb1.Arrange(new Rect(0, 0, 10, 10));
            thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height, 10, 10));
            return base.ArrangeOverride(finalSize);
        }
    }
}
