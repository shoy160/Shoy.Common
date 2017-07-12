using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shoy.Wpf.DesignControls.Adorners
{
    public class ResizeRotateAdorner : Adorner
    {
        private readonly VisualCollection _visuals;
        private readonly ResizeRotateChrome _chrome;

        protected override int VisualChildrenCount => _visuals.Count;

        public ResizeRotateAdorner(UIElement designerItem)
            : base(designerItem)
        {
            SnapsToDevicePixels = true;
            _chrome = new ResizeRotateChrome { DataContext = designerItem };
            _visuals = new VisualCollection(this) { _chrome };
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
    }
}
