using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Shoy.Wpf.DesignControls.Adorners;

namespace Shoy.Wpf.DesignControls
{
    /// <summary> 设计器 </summary>
    public class DesignerItemDecorator : Control
    {
        /// <summary> 缩放旋转修饰 </summary>
        private Adorner _adorner;

        public bool ShowDecorator
        {
            get { return (bool)GetValue(ShowDecoratorProperty); }
            set { SetValue(ShowDecoratorProperty, value); }
        }

        public static readonly DependencyProperty ShowDecoratorProperty =
            DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(DesignerItemDecorator),
            new FrameworkPropertyMetadata(false, ShowDecoratorProperty_Changed));

        public DesignerItemDecorator()
        {
            Unloaded += DesignerItemDecorator_Unloaded;
        }

        private void HideAdorner()
        {
            if (_adorner != null)
            {
                _adorner.Visibility = Visibility.Hidden;
            }
        }

        private void ShowAdorner()
        {
            if (_adorner == null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer == null)
                    return;
                var designerItem = DataContext as ContentControl;
                if (designerItem == null)
                    return;
                //var canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
                _adorner = new ResizeRotateAdorner(designerItem);
                adornerLayer.Add(_adorner);

                _adorner.Visibility = ShowDecorator ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                _adorner.Visibility = Visibility.Visible;
            }
        }

        private void DesignerItemDecorator_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this._adorner == null) return;
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            adornerLayer?.Remove(_adorner);
            _adorner = null;
        }

        private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var decorator = (DesignerItemDecorator)d;
            var showDecorator = (bool)e.NewValue;

            if (showDecorator)
            {
                decorator.ShowAdorner();
            }
            else
            {
                decorator.HideAdorner();
            }
        }
    }
}
