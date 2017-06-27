using Shoy.Wpf.Helper;
using System.Windows.Controls;

namespace Shoy.Wpf.Converter
{
    /// <summary>
    /// 常用转换器的静态引用
    /// 使用实例：Converter={x:Static local:DConverter.BooleanToVisibilityConverter}
    /// </summary>
    public sealed class DConverter
    {
        public static BooleanToVisibilityConverter BooleanToVisibilityConverter => Singleton<BooleanToVisibilityConverter>.Instance;

        public static ThicknessToDoubleConverter ThicknessToDoubleConverter => Singleton<ThicknessToDoubleConverter>.Instance;

        public static PercentToAngleConverter PercentToAngleConverter => Singleton<PercentToAngleConverter>.Instance;
    }
}
