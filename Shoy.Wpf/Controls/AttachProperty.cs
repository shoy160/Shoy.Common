using Shoy.Wpf.Core;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Shoy.Wpf.Controls
{
    /// <summary> 插件通用属性 </summary>
    public class AttachProperty : DNotifyPropertyChanged
    {
        #region 水印
        /// <summary> 水印 </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
            "Watermark", typeof(string), typeof(AttachProperty), new FrameworkPropertyMetadata(string.Empty));

        public static string GetWatermark(DependencyObject d)
        {
            return (string)d.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        /// <summary> 水印字体大小 </summary>
        public static readonly DependencyProperty WatermarkSizeProperty = DependencyProperty.RegisterAttached(
            "WatermarkSize", typeof(double), typeof(AttachProperty), new FrameworkPropertyMetadata(14D));

        public static double GetWatermarkSize(DependencyObject d)
        {
            return (double)d.GetValue(WatermarkSizeProperty);
        }

        public static void SetWatermarkSize(DependencyObject obj, double value)
        {
            obj.SetValue(WatermarkSizeProperty, value);
        }
        #endregion

        #region 旋转动画
        /// <summary> 启用旋转动画 </summary>
        public static readonly DependencyProperty RotateAnimationProperty = DependencyProperty.RegisterAttached("RotateAnimation"
            , typeof(bool), typeof(AttachProperty), new FrameworkPropertyMetadata(false, RotateAnimationChanged));

        public static bool GetRotateAnimation(DependencyObject d)
        {
            return (bool)d.GetValue(RotateAnimationProperty);
        }

        public static void SetRotateAnimation(DependencyObject obj, bool value)
        {
            obj.SetValue(RotateAnimationProperty, value);
        }

        /// <summary> 旋转动画刻度 </summary>
        private static readonly DoubleAnimation RotateAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(200)));

        /// <summary>
        /// 绑定动画事件
        /// </summary>
        private static void RotateAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as FrameworkElement;
            if (uc == null) return;
            if (uc.RenderTransformOrigin == new Point(0, 0))
            {
                uc.RenderTransformOrigin = new Point(0.5, 0.5);
                RotateTransform trans = new RotateTransform(0);
                uc.RenderTransform = trans;
            }
            var value = (bool)e.NewValue;
            if (value)
            {
                RotateAnimation.To = 180;
                uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            }
            else
            {
                RotateAnimation.To = 0;
                uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            }
        }
        #endregion

        #region FIconProperty 字体图标
        /// <summary>
        /// 字体图标
        /// </summary>
        public static readonly DependencyProperty FIconProperty = DependencyProperty.RegisterAttached(
            "FIcon", typeof(string), typeof(AttachProperty), new FrameworkPropertyMetadata(string.Empty));

        public static string GetFIcon(DependencyObject d)
        {
            return (string)d.GetValue(FIconProperty);
        }

        public static void SetFIcon(DependencyObject obj, string value)
        {
            obj.SetValue(FIconProperty, value);
        }
        /// <summary>
        /// 字体图标
        /// </summary>
        public static readonly DependencyProperty FIconSizeProperty = DependencyProperty.RegisterAttached(
            "FIconSize", typeof(double), typeof(AttachProperty), new FrameworkPropertyMetadata(12D));

        public static double GetFIconSize(DependencyObject d)
        {
            return (double)d.GetValue(FIconSizeProperty);
        }

        public static void SetFIconSize(DependencyObject obj, double value)
        {
            obj.SetValue(FIconSizeProperty, value);
        }
        /// <summary>
        /// 字体图标
        /// </summary>
        public static readonly DependencyProperty FIconMarginProperty = DependencyProperty.RegisterAttached(
            "FIconMargin", typeof(Thickness), typeof(AttachProperty), new FrameworkPropertyMetadata(null));

        public static Thickness GetFIconMargin(DependencyObject d)
        {
            return (Thickness)d.GetValue(FIconMarginProperty);
        }

        public static void SetFIconMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(FIconMarginProperty, value);
        }
        #endregion

        #region CornerRadiusProperty Border圆角
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(CornerRadius), typeof(AttachProperty), new FrameworkPropertyMetadata(null));

        public static CornerRadius GetCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region Hover
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.RegisterAttached(
            "HoverBackground", typeof(Brush), typeof(AttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetHoverBackground(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBackgroundProperty, value);
        }

        public static Brush GetHoverBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBackgroundProperty);
        }

        public static readonly DependencyProperty HoverBorderProperty = DependencyProperty.RegisterAttached(
            "HoverBorder", typeof(Brush), typeof(AttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetHoverBorder(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBorderProperty, value);
        }

        public static Brush GetHoverBorder(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBorderProperty);
        }

        public static readonly DependencyProperty HoverBrushProperty = DependencyProperty.RegisterAttached(
            "HoverBrush", typeof(Brush), typeof(AttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetHoverBrush(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBrushProperty, value);
        }

        public static Brush GetHoverBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBrushProperty);
        }
        #endregion
    }
}
