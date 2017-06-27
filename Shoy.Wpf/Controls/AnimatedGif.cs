using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;

namespace Shoy.Wpf.Controls
{
    public class AnimatedGif : Image
    {
        public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register(
            "GifSource", typeof(string), typeof(AnimatedGif), new PropertyMetadata(OnSourcePropertyChanged));

        /// <summary>
        /// GIF图片源，支持相对路径、绝对路径
        /// </summary>
        public string GifSource
        {
            get { return (string)GetValue(GifSourceProperty); }
            set { SetValue(GifSourceProperty, value); }
        }

        internal Bitmap Bitmap; // Local bitmap member to cache image resource
        internal BitmapSource BitmapSource;
        public delegate void FrameUpdatedEventHandler();

        /// <summary>
        /// Delete local bitmap resource
        /// Reference: http://msdn.microsoft.com/en-us/library/dd183539(VS.85).aspx
        /// </summary>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Loaded += AnimatedGIF_Loaded;
            Unloaded += AnimatedGIF_Unloaded;
        }

        private void AnimatedGIF_Unloaded(object sender, RoutedEventArgs e)
        {
            StopAnimate();
        }

        private void AnimatedGIF_Loaded(object sender, RoutedEventArgs e)
        {
            BindSource(this);
        }

        /// <summary>
        /// Start animation
        /// </summary>
        public void StartAnimate()
        {
            ImageAnimator.Animate(Bitmap, OnFrameChanged);
        }

        /// <summary>
        /// Stop animation
        /// </summary>
        public void StopAnimate()
        {
            ImageAnimator.StopAnimate(Bitmap, OnFrameChanged);
        }

        /// <summary>
        /// Event handler for the frame changed
        /// </summary>
        private void OnFrameChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                   new FrameUpdatedEventHandler(FrameUpdatedCallback));
        }

        private void FrameUpdatedCallback()
        {
            ImageAnimator.UpdateFrames();

            BitmapSource?.Freeze();
            // Convert the bitmap to BitmapSource that can be display in WPF Visual Tree
            GetBitmapSource(this, Bitmap);
            Source = BitmapSource;
            InvalidateVisual();
        }

        /// <summary>
        /// 属性更改处理事件
        /// </summary>
        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var gif = sender as AnimatedGif;
            if (gif == null || !gif.IsLoaded) return;
            BindSource(gif);
        }

        private static void BindSource(AnimatedGif gif)
        {
            gif.StopAnimate();
            gif.Bitmap?.Dispose();
            var path = gif.GifSource;
            if (string.IsNullOrWhiteSpace(path))
                return;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            if (!File.Exists(path)) return;
            gif.Bitmap = new Bitmap(path);
            GetBitmapSource(gif, gif.Bitmap);
            gif.StartAnimate();
        }

        private static void GetBitmapSource(AnimatedGif gif, Bitmap bmap)
        {
            var handle = IntPtr.Zero;

            try
            {
                handle = bmap.GetHbitmap();
                gif.BitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                if (handle != IntPtr.Zero)
                    DeleteObject(handle);
            }
        }
    }
}
