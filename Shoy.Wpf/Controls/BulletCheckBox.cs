using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Shoy.Wpf.Controls
{
    public class BulletCheckBox : CheckBox
    {
        public static readonly DependencyProperty OnProperty = DependencyProperty.Register(
            "On", typeof(string), typeof(BulletCheckBox), new PropertyMetadata("On"));

        public string On
        {
            get { return (string)GetValue(OnProperty); }
            set { SetValue(OnProperty, value); }
        }

        public static readonly DependencyProperty OffProperty = DependencyProperty.Register(
            "Off", typeof(string), typeof(BulletCheckBox), new PropertyMetadata("Off"));

        public string Off
        {
            get { return (string)GetValue(OffProperty); }
            set { SetValue(OffProperty, value); }
        }

        public static readonly DependencyProperty OnForegroundProperty = DependencyProperty.Register(
            "OnForeground", typeof(Brush), typeof(BulletCheckBox), new PropertyMetadata(Brushes.White));

        public Brush OnForeground
        {
            get { return (Brush)GetValue(OnForegroundProperty); }
            set { SetValue(OnForegroundProperty, value); }
        }

        public static readonly DependencyProperty OnBackgroundProperty = DependencyProperty.Register(
            "OnBackground", typeof(Brush), typeof(BulletCheckBox), new PropertyMetadata(Brushes.White));

        public Brush OnBackground
        {
            get { return (Brush)GetValue(OnBackgroundProperty); }
            set { SetValue(OnBackgroundProperty, value); }
        }

        public BulletCheckBox()
        {
        }
    }
}
