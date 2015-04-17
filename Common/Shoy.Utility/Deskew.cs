using System;
using System.Drawing;

namespace Shoy.Utility
{
    /// <summary> 图像纠偏辅助类 </summary>
    public class Deskew
    {
        /// <summary> 图像 </summary>
        private readonly Bitmap _internalBmp;
        const double AlphaStart = -20;
        const double AlphaStep = 0.2;
        const int Steps = 40 * 5;
        const double Step = 1;
        double[] _sinA;
        double[] _cosA;
        // Range of d
        double _min;

        int _count;
        // Count of points that fit in a line.
        int[] _hMatrix;
        
        private class HougLine
        {
            public int Count;
            public int Index;
            public double Alpha;
        }

        /// <summary> 纠偏构造函数 </summary>
        /// <param name="internalBmp"></param>
        public Deskew(Bitmap internalBmp)
        {
            _internalBmp = internalBmp;
        }
        
        /// <summary> 获取图片偏移角度 </summary>
        /// <returns></returns>
        public double GetSkewAngle()
        {
            try
            {
                Calc();
                // Top 20 of the detected lines in the image.
                HougLine[] hl = GetTop(20);
                // Average angle of the lines
                double sum = 0;
                int count = 0;
                for (int i = 0; i <= 19; i++)
                {
                    sum += hl[i].Alpha;
                    count += 1;
                }
                return sum/count;
            }
            catch
            {
                return 0;
            }
        }

        // Calculate the Count lines in the image with most points.
        private HougLine[] GetTop(int count)
        {
            var hl = new HougLine[count];
            int i;
            for (i = 0; i <= count - 1; i++)
            {
                hl[i] = new HougLine();
            }
            for (i = 0; i <= _hMatrix.Length - 1; i++)
            {
                if (_hMatrix[i] <= hl[count - 1].Count)
                    continue;
                hl[count - 1].Count = _hMatrix[i];
                hl[count - 1].Index = i;
                int j = count - 1;
                while (j > 0 && hl[j].Count > hl[j - 1].Count)
                {
                    HougLine tmp = hl[j];
                    hl[j] = hl[j - 1];
                    hl[j - 1] = tmp;
                    j -= 1;
                }
            }
            for (i = 0; i <= count - 1; i++)
            {
                int dIndex = hl[i].Index / Steps;
                int alphaIndex = hl[i].Index - dIndex * Steps;
                hl[i].Alpha = GetAlpha(alphaIndex);
            }
            return hl;
        }

        // Hough Transforamtion:
        private void Calc()
        {
            int hMin = _internalBmp.Height / 4;
            int hMax = _internalBmp.Height * 3 / 4;
            Init();
            for (int y = hMin; y <= hMax; y++)
            {
                for (int x = 1; x <= _internalBmp.Width - 2; x++)
                {
                    // Only lower edges are considered.
                    if (IsBlack(x, y))
                    {
                        if (!IsBlack(x, y + 1))
                        {
                            Calc(x, y);
                        }
                    }
                }
            }
        }
        // Calculate all lines through the point (x,y).
        private void Calc(int x, int y)
        {
            int alpha;
            for (alpha = 0; alpha <= Steps - 1; alpha++)
            {
                double d = y * _cosA[alpha] - x * _sinA[alpha];
                var calculatedIndex = (int)CalcDIndex(d);
                int index = calculatedIndex * Steps + alpha;
                try
                {
                    _hMatrix[index] += 1;
                }
                catch
                {}
            }
        }
        private double CalcDIndex(double d)
        {
            return Convert.ToInt32(d - _min);
        }

        private bool IsBlack(int x, int y)
        {
            Color c = _internalBmp.GetPixel(x, y);
            double luminance = (c.R * 0.299) + (c.G * 0.587) + (c.B * 0.114);
            return luminance < 140;
        }

        private void Init()
        {
            _cosA = new double[Steps];
            _sinA = new double[Steps];
            for (int i = 0; i < Steps; i++)
            {
                double angle = GetAlpha(i) * Math.PI / 180.0;
                _sinA[i] = Math.Sin(angle);
                _cosA[i] = Math.Cos(angle);
            }   
            _min = -_internalBmp.Width;
            _count = (int)(2 * (_internalBmp.Width + _internalBmp.Height) / Step);
            _hMatrix = new int[_count * Steps];
        }

        private static double GetAlpha(int index)
        {
            return AlphaStart + index * AlphaStep;
        }
    }
}
