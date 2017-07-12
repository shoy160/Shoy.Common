using Shoy.Wpf.Converter;
using Shoy.Wpf.Helper;
using System.Windows.Controls;

namespace Shoy.Wpf.Controls
{
    /// <summary>
    /// 常用转换器的静态引用
    /// 使用实例：Converter={x:Static local:DConverter.BooleanToVisibilityConverter}
    /// </summary>
    public sealed class DConverter
    {
        public static BooleanToVisibilityConverter BooleanToVisibilityConverter =>
            Singleton<BooleanToVisibilityConverter>.Instance ?? (Singleton<BooleanToVisibilityConverter>.Instance =
                new BooleanToVisibilityConverter());

        public static ThicknessToDoubleConverter ThicknessToDoubleConverter =>
            Singleton<ThicknessToDoubleConverter>.Instance ?? (Singleton<ThicknessToDoubleConverter>.Instance =
                new ThicknessToDoubleConverter());

        public static PercentToAngleConverter PercentToAngleConverter =>
            Singleton<PercentToAngleConverter>.Instance ??
            (Singleton<PercentToAngleConverter>.Instance = new PercentToAngleConverter());
    }
}
