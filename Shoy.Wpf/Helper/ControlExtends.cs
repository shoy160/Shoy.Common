using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Shoy.Wpf.Helper
{
    public static class ControlExtends
    {
        /// <summary> 绑定命令和命令事件到宿主UI </summary>
        public static void BindCommand(this UIElement ui, ICommand com, Action<object, ExecutedRoutedEventArgs> call)
        {
            var bind = new CommandBinding(com);
            bind.Executed += new ExecutedRoutedEventHandler(call);
            ui.CommandBindings.Add(bind);
        }

        /// <summary> 绑定RelayCommand命令到宿主UI </summary>
        public static void BindCommand(this UIElement ui, ICommand com)
        {
            var bind = new CommandBinding(com);
            ui.CommandBindings.Add(bind);
        }

        //从Handle中获取Window对象
        private static Window GetWindowFromHwnd(IntPtr hwnd)
        {
            var hwndSource = HwndSource.FromHwnd(hwnd);
            return (Window)hwndSource?.RootVisual;
        }

        //调用GetForegroundWindow然后调用GetWindowFromHwnd
        /// <summary> 获取当前顶级窗体，若获取失败则返回主窗体 </summary>
        public static Window GetTopWindow()
        {
            var hwnd = NativeHelper.GetForegroundWindow();
            return hwnd == IntPtr.Zero ? Application.Current.MainWindow : GetWindowFromHwnd(hwnd);
        }
    }
}
