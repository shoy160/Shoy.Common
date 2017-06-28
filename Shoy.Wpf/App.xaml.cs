using System.Windows;

namespace Shoy.Wpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.DispatcherUnhandledException += (sender, evt) =>
            {
                MessageBox.Show(evt.Exception.Message);
                evt.Handled = true;
            };
            base.OnStartup(e);
        }
    }
}
