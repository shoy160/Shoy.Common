using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Shoy.Utility
{
    /// <summary>
    /// 图片处理辅助类
    /// </summary>
    public class ImageCls:IDisposable
    {
        private static Image _oldImg;
        private static Graphics _newGraphics;
        private static string _ext = ".jpeg";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oldPath">图片路径</param>
        public ImageCls(string oldPath)
        {
            _ext = Path.GetExtension(oldPath);
            _oldImg = Image.FromFile(oldPath);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oldStream">图片流</param>
        public ImageCls(Stream oldStream)
        {
            _oldImg = Image.FromStream(oldStream);
        }

        /// <summary>
        /// 压缩图片，返回map
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public Bitmap ResizeImg(int width, int height)
        {
            if (_oldImg == null)
                return null;
            int ow = _oldImg.Width, oh = _oldImg.Height; //原始大小
            if (height == -2)
            {
                var rale = (double)width / Math.Max(ow, oh); //压缩比例
                width = (int)Math.Ceiling(ow * rale);
                height = (int)Math.Ceiling(oh * rale);
            }
            else
            {
                width = (width != -1 ? width : ow);
                height = (height != -1 ? height : oh);
            }
            var bm = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            _newGraphics = Graphics.FromImage(bm);
            //呈现质量
            _newGraphics.CompositingQuality = CompositingQuality.HighQuality;
            //像素偏移方式
            _newGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //平滑处理
            _newGraphics.SmoothingMode = SmoothingMode.HighQuality;
            //插补模式,双三次插值法
            _newGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            _newGraphics.Clear(Color.Transparent);

            _newGraphics.DrawImage(_oldImg, new Rectangle(0, 0, width, height), new Rectangle(0, 0, ow, oh),
                                  GraphicsUnit.Pixel);
            //return "处理成功";
            return bm;
        }

        /// <summary>
        /// 压缩图片(简化)，返回map
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public Bitmap ResizeImg(int width)
        {
            return ResizeImg(width, -2);
        }

        /// <summary>
        /// 压缩图片，返回结果
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="newPath">新路径</param>
        /// <param name="qt">压缩质量(0-100)</param>
        /// <returns></returns>
        public bool ResizeImg(int width, int height, string newPath, int qt)
        {
            try
            {
                var bm = ResizeImg(width, height);
                var encoder = GetEncoderInfo(_ext);
                if (encoder == null)
                    return false;
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, qt);
                bm.Save(newPath, encoder, encoderParameters);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 压缩图片(简化)，返回结果
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="newPath">新路径</param>
        /// <param name="qt">压缩质量(0-100)</param>
        /// <returns></returns>
        public bool ResizeImg(int width,string newPath,int qt)
        {
            return ResizeImg(width, -2, newPath, qt);
        }

        /// <summary>
        /// 压缩图片(简化)，返回结果
        /// </summary>
        /// <param name="newPath">新路径</param>
        /// <param name="qt">压缩质量(0-100)</param>
        /// <returns></returns>
        public bool ResizeImg(string newPath, int qt)
        {
            return ResizeImg(-1, -1, newPath, qt);
        }

        /// <summary>
        /// 压缩图片(简化)，返回结果
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="newPath">新路径</param>
        /// <returns></returns>
        public bool ResizeImg(int width, string newPath)
        {
            return ResizeImg(width, -2, newPath, 80);
        }

        /// <summary>
        /// 剪切图片
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="width">保存图片宽</param>
        /// <param name="height">保存图片高</param>
        /// <param name="newPath">新路径</param>
        /// <param name="qt"></param>
        /// <returns></returns>
        public bool CutImage(int x, int y, int w, int h, int width, int height, string newPath, int qt)
        {
            if (_oldImg == null)
                return false;
            int ow = _oldImg.Width, oh = _oldImg.Height; //原始大小
            if (x > ow || y > oh)
                return false;
            if (x + w > ow)
                w = ow - x;
            if (y + h > oh)
                h = oh - y;
            if (width <= 0)
                width = w;
            if (height <= 0)
                height = h;
            var bm = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            _newGraphics = Graphics.FromImage(bm);
            //呈现质量
            _newGraphics.CompositingQuality = CompositingQuality.HighQuality;
            //像素偏移方式
            _newGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //平滑处理
            _newGraphics.SmoothingMode = SmoothingMode.HighQuality;
            //插补模式,双三次插值法
            _newGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            _newGraphics.Clear(Color.Transparent);

            _newGraphics.DrawImage(_oldImg, new Rectangle(0, 0, width, height), new Rectangle(x, y, w, h),
                                   GraphicsUnit.Pixel);
            //return "处理成功";
            var encoder = GetEncoderInfo(_ext);
            if (encoder == null)
                return false;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, qt);
            bm.Save(newPath, encoder, encoderParameters);
            return true;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //根据 mime 类型，返回编码器
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            mimeType = "image/" + mimeType.Replace(".", string.Empty).ToLower();
            mimeType = mimeType.Replace("jpg", "jpeg");
            return encoders.FirstOrDefault(t => t.MimeType == mimeType);
        }

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            if (_oldImg != null)
                _oldImg.Dispose();
            if (_newGraphics != null)
                _newGraphics.Dispose();
        }

        #endregion
    }
}
