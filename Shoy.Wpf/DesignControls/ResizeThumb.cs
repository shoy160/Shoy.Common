using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Shoy.Wpf.DesignControls.Adorners;

namespace Shoy.Wpf.DesignControls
{
    /// <summary> 缩放组件 </summary>
    public class ResizeThumb : Thumb
    {
        private RotateTransform _rotateTransform;
        private double _angle;
        private Adorner _adorner;
        private Point _transformOrigin;
        private ContentControl _designerItem;
        private Canvas _canvas;

        public ResizeThumb()
        {
            DragStarted += ResizeThumb_DragStarted;
            DragDelta += ResizeThumb_DragDelta;
            DragCompleted += ResizeThumb_DragCompleted;
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _designerItem = DataContext as ContentControl;

            if (_designerItem == null)
                return;
            _canvas = VisualTreeHelper.GetParent(_designerItem) as Canvas;

            if (_canvas == null)
                return;
            _transformOrigin = _designerItem.RenderTransformOrigin;

            _rotateTransform = _designerItem.RenderTransform as RotateTransform;
            if (_rotateTransform != null)
            {
                _angle = _rotateTransform.Angle * Math.PI / 180.0;
            }
            else
            {
                _angle = 0.0d;
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);
            if (adornerLayer == null)
                return;
            _adorner = new SizeAdorner(_designerItem);
            adornerLayer.Add(_adorner);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, _designerItem.ActualHeight - _designerItem.MinHeight);
                        Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + (_transformOrigin.Y * deltaVertical * (1 - Math.Cos(-_angle))));
                        Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) - deltaVertical * _transformOrigin.Y * Math.Sin(-_angle));
                        _designerItem.Height -= deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, _designerItem.ActualHeight - _designerItem.MinHeight);
                        Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + deltaVertical * Math.Cos(-_angle) + (_transformOrigin.Y * deltaVertical * (1 - Math.Cos(-_angle))));
                        Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + deltaVertical * Math.Sin(-_angle) - (_transformOrigin.Y * deltaVertical * Math.Sin(-_angle)));
                        _designerItem.Height -= deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, _designerItem.ActualWidth - _designerItem.MinWidth);
                        Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + deltaHorizontal * Math.Sin(_angle) - _transformOrigin.X * deltaHorizontal * Math.Sin(_angle));
                        Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + deltaHorizontal * Math.Cos(_angle) + (_transformOrigin.X * deltaHorizontal * (1 - Math.Cos(_angle))));
                        _designerItem.Width -= deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, _designerItem.ActualWidth - _designerItem.MinWidth);
                        Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) - _transformOrigin.X * deltaHorizontal * Math.Sin(_angle));
                        Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + (deltaHorizontal * _transformOrigin.X * (1 - Math.Cos(_angle))));
                        _designerItem.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }
                //var dto = _designerItem.DataContext as DesignDto;
                //if (dto != null)
                //{
                //    dto.Height = _designerItem.Height;
                //    dto.Width = _designerItem.Width;
                //    dto.Top = Canvas.GetTop(_designerItem);
                //    dto.Left = Canvas.GetLeft(_designerItem);
                //}
            }

            e.Handled = true;
        }

        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (_adorner == null) return;
            var adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);
            adornerLayer?.Remove(_adorner);

            _adorner = null;
        }
    }
}
