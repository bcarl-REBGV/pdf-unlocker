using System.Windows;
using System.Windows.Threading;

namespace PdfUnlockerGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionHandling.GlobalExceptionHandler(sender, e.Exception);
            e.Handled = true;
        }
    }
}