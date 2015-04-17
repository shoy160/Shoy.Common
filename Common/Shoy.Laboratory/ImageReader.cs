using Shoy.Utility.Extend;
using System.ComponentModel;
using System.Drawing;
using Tesseract;

namespace Shoy.Laboratory
{
    /// <summary> 图片识别 辅助 </summary>
    public class ImageReader
    {
        private const string DataPath = @"D:\DayEz\ocr\tessdata";
        private readonly TesseractEngine _engine;

        public ImageReader(string dataPath = null, LanguageType type = LanguageType.English)
        {
            if (string.IsNullOrWhiteSpace(dataPath))
                dataPath = DataPath;
            _engine = new TesseractEngine(dataPath, type.GetText(), EngineMode.Default);
        }

        public string Read(Bitmap bmp, string whiteList = null, bool hasConfidence = false)
        {
            if (!string.IsNullOrWhiteSpace(whiteList))
                _engine.SetVariable("tessedit_char_whitelist", whiteList);
            using (var pix = PixConverter.ToPix(bmp))
            {
                using (var page = _engine.Process(pix))
                {
                    if (hasConfidence)
                        return string.Format("{0}:{1}", page.GetMeanConfidence(), page.GetText());
                    return page.GetText();
                }
            }
        }

        public const string Numbers = "0123456789";
        public const string Letter = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    }

    public enum LanguageType
    {
        /// <summary> 英语 </summary>
        [Description("eng")]
        English,
        /// <summary> 中文 </summary>
        [Description("chi_sim")]
        Chinese
    }
}
