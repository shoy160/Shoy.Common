using System.Windows;
using System.Windows.Input;

namespace Shoy.Wpf.Controls
{
    public class DWindow : Window
    {
        public DWindow()
        {
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
