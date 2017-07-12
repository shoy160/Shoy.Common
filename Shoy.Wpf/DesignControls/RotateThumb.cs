using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Shoy.Wpf.DesignControls
{
    /// <summary> 旋转组件 </summary>
    public class RotateThumb : Thumb
    {
        private double _initialAngle;
        private RotateTransform _rotateTransform;
        private Vector _startVector;
        private Point _centerPoint;
        private ContentControl _designerItem;
        private Canvas _canvas;

        public RotateThumb()
        {
            DragDelta += RotateThumb_DragDelta;
            DragStarted += RotateThumb_DragStarted;
        }

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _designerItem = DataContext as ContentControl;

            if (_designerItem == null)
                return;
            _canvas = VisualTreeHelper.GetParent(_designerItem) as Canvas;

            if (_canvas == null)
                return;
            _centerPoint = _designerItem.TranslatePoint(
                new Point(_designerItem.Width * _designerItem.RenderTransformOrigin.X,
                    _designerItem.Height * _designerItem.RenderTransformOrigin.Y),
                _canvas);

            var startPoint = Mouse.GetPosition(_canvas);
            _startVector = Point.Subtract(startPoint, _centerPoint);

            _rotateTransform = _designerItem.RenderTransform as RotateTransform;
            if (_rotateTransform == null)
            {
                _designerItem.RenderTransform = new RotateTransform(0);
                _initialAngle = 0;
            }
            else
            {
                _initialAngle = _rotateTransform.Angle;
            }
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_designerItem == null || _canvas == null)
                return;
            var currentPoint = Mouse.GetPosition(_canvas);
            var deltaVector = Point.Subtract(currentPoint, _centerPoint);

            var angle = Vector.AngleBetween(_startVector, deltaVector);

            var rotateTransform = _designerItem.RenderTransform as RotateTransform;
            if (rotateTransform == null)
                return;
            rotateTransform.Angle = _initialAngle + Math.Round(angle, 0);
            _designerItem.InvalidateMeasure();
        }
    }
}
