using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Shoy.Wpf.Controls;

namespace Shoy.Wpf.DesignControls
{
    /// <summary> 移动组件 </summary>
    public class MoveThumb : Thumb
    {
        private RotateTransform _rotateTransform;
        private ContentControl _designerItem;

        //public static readonly DependencyProperty CanOverBoundsProperty = DependencyProperty.Register(
        //    "CanOverBounds", typeof(bool), typeof(MoveThumb), new PropertyMetadata(default(bool)));

        //public bool CanOverBounds
        //{
        //    get { return (bool)GetValue(CanOverBoundsProperty); }
        //    set { SetValue(CanOverBoundsProperty, value); }
        //}

        public MoveThumb()
        {
            DragStarted += MoveThumb_DragStarted;
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _designerItem = DataContext as ContentControl;

            if (_designerItem != null)
            {
                _rotateTransform = _designerItem.RenderTransform as RotateTransform;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_designerItem == null)
                return;

            var dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (_rotateTransform != null)
            {
                dragDelta = _rotateTransform.Transform(dragDelta);
            }

            var left = Canvas.GetLeft(_designerItem) + dragDelta.X;
            var top = Canvas.GetTop(_designerItem) + dragDelta.Y;
            //边际判断
            if (left < 0 || top < 0)
                return;
            var canvas = _designerItem.FindParent<Canvas>();
            if (top + _designerItem.Height > canvas.Height || left + _designerItem.Width > canvas.Width)
                return;
            //var dto = _designerItem.DataContext as DesignDto;
            //if (dto != null)
            //{
            //    dto.Left = left;
            //    dto.Top = top;
            //}
            Canvas.SetLeft(_designerItem, left);
            Canvas.SetTop(_designerItem, top);
        }
    }
}
