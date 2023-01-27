using System;
using System.Windows;

namespace PdfUnlockerGui
{
    public static class ExceptionHandling
    {
        public static void GlobalExceptionHandler(object sender, Exception e, Action? cleanupAction)
        {
            string messageBoxText = e.Message;
            MessageBox.Show(messageBoxText, "Error", MessageBoxButton.OK);
            if (cleanupAction != null)
            {
                cleanupAction();
            }
        }
    }
}