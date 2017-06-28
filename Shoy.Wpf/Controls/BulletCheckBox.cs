using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Shoy.Wpf.Controls
{
    public class BulletCheckBox : CheckBox
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(BulletCheckBox), new PropertyMetadata("Off"));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty CheckedTextProperty = DependencyProperty.Register(
            "CheckedText", typeof(string), typeof(BulletCheckBox), new PropertyMetadata("On"));

        public string CheckedText
        {
            get { return (string)GetValue(CheckedTextProperty); }
            set { SetValue(CheckedTextProperty, value); }
        }

        public static readonly DependencyProperty CheckedForegroundProperty = DependencyProperty.Register(
            "CheckedForeground", typeof(Brush), typeof(BulletCheckBox), new PropertyMetadata(default(Brush)));

        public Brush CheckedForeground
        {
            get { return (Brush)GetValue(CheckedForegroundProperty); }
            set { SetValue(CheckedForegroundProperty, value); }
        }

        public static readonly DependencyProperty CheckedBackgroundProperty = DependencyProperty.Register(
            "CheckedBackground", typeof(Brush), typeof(BulletCheckBox), new PropertyMetadata(default(Brush)));

        public Brush CheckedBackground
        {
            get { return (Brush)GetValue(CheckedBackgroundProperty); }
            set { SetValue(CheckedBackgroundProperty, value); }
        }

        public BulletCheckBox()
        {
        }
    }
}
