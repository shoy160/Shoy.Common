using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace Shoy.Laboratory
{
    /// <summary>
    /// 二维码生成帮助类
    /// </summary>
    public class QrCodeHelper
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="code">参数</param>
        /// <param name="eSize">二维码尺寸</param>
        /// <param name="img">小图</param>
        /// <param name="iSize">小图尺寸</param>
        /// <param name="bColor">二维码背景颜色</param>
        /// <param name="fColor">二维码颜色</param>
        /// <returns></returns>
        public static Image GetQrcode(string code, int eSize, Image img, int iSize, Brush bColor, Brush fColor)
        {
            var qrcoder = new QrEncoder(ErrorCorrectionLevel.H);
            var qrCode = qrcoder.Encode(code);

            var render = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Four), fColor, bColor);

            using (Stream stream = new MemoryStream())
            {
                eSize = eSize > 0 ? eSize : 200;

                render.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream, new Point(10, 10));

                var be = new Bitmap(stream);
                be = ResizeImage(be, eSize, eSize);
                Graphics ge = Graphics.FromImage(be);

                if (img != null)
                {
                    iSize = iSize > 0 ? iSize : 50;
                    img = ResizeImage(img, iSize, iSize);

                    //小图白色边框
                    var bi = new Bitmap(iSize + 10, iSize + 10);
                    Graphics gi = Graphics.FromImage(bi);
                    gi.Clear(Color.White);
                    gi.DrawImage(img, 5, 5, iSize, iSize);

                    //将小图插入到二维码图片中
                    ge.DrawImage(bi, (eSize - iSize) / 2, (eSize - iSize) / 2, iSize, iSize);
                }
                return be;
            }
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="code">字符串</param>
        /// <returns></returns>
        public static Image GetQrcode(string code)
        {
            return GetQrcode(code, 200, null, 0, Brushes.White, Brushes.Black);
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="code">字符串</param>
        /// <param name="size">尺寸</param>
        /// <returns></returns>
        public static Image GetQrcode(string code, int size)
        {
            return GetQrcode(code, size, null, 0, Brushes.White, Brushes.Black);
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="sourceImage">源图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Image sourceImage, int width, int height)
        {
            Graphics graphics = null;
            try
            {
                var b = new Bitmap(width, height);
                graphics = Graphics.FromImage(b);

                //呈现质量
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                //像素偏移方式
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //平滑处理
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                //插补模式,双三次插值法
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.Clear(Color.Transparent);

                graphics.DrawImage(sourceImage, new Rectangle(0, 0, width, height),
                                   new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), GraphicsUnit.Pixel);
                return b;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (graphics != null)
                    graphics.Dispose();
            }
        }
    }
}