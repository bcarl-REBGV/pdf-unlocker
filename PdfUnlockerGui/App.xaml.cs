using System;
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
            Action cleanupFunction = (MainWindow as MainWindow)?.CleanupFunction ?? delegate {  };
            ExceptionHandling.GlobalExceptionHandler(sender, e.Exception, cleanupFunction);
            e.Handled = true;
        }
    }
}