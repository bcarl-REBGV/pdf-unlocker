using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PdfUnlockerGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "UseNegatedPatternMatching")]
    public partial class MainWindow

    {
        private readonly MainWindowViewModel? _data;
        private readonly RotateTransform? _imageTransform;

        public Action CleanupFunction => UpdateUiOnError;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            _data = this.DataContext as MainWindowViewModel;
            _imageTransform = FindName("ImageAnimatedTransform") as RotateTransform;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (_data is null) return;
            foreach (string path in _data.UnlockedFileLocations)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(path)
                    { UseShellExecute = true };
                Process.Start(startInfo);
            }
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (_data is null) return;
            if (!TryGetDroppedPdfFiles(e, out string[] fileNames))
            {
                throw new Exception("Invalid file was dropped.");
            }

            _data.LinkVisibility = Visibility.Collapsed;

            Uri[] fileNameUris = fileNames.Select((s) => new Uri(s)).ToArray();
            _data.ImageSource = new BitmapImage(new Uri("/Resources/spinner.gif", UriKind.Relative));

            StartRotateAnim(_imageTransform);

            _data.UnlockedFileLocations = new List<string>();
            if (fileNameUris.Length > 1)
            {
                _data.Message = "Unlocking multiple files";
            }
            else
            {
                _data.Message = $"Unlocking pdf: {Uri.UnescapeDataString(fileNameUris[0].Segments[^1])}";
            }

            for (int index = 0; index < fileNameUris.Length; index++)
            {
                Uri file = fileNameUris[index];
                Task<Uri?> unlockTask = PdfHandlerWrapper.UnlockPdf(file, CleanupFunction);


                HandlePdfTaskComplete(unlockTask, index == fileNameUris.Length - 1);
            }

            e.Handled = true;
        }

        private void MainWindow_OnDragEnterOver(object sender, DragEventArgs e)
        {
            if (!TryGetDroppedPdfFiles(e, out string[] fileNames))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Copy;
            }

            e.Handled = true;
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (_data is null) return;
            Process.Start(new ProcessStartInfo("explorer", "/select," + _data.UnlockedFileLocation));
        }

        private void StartRotateAnim(RotateTransform? transform)
        {
            if (transform is null) return;
            DoubleAnimation anim = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(2)));
            anim.RepeatBehavior = RepeatBehavior.Forever;
            transform.BeginAnimation(RotateTransform.AngleProperty, anim);
        }

        private void EndRotateAnim(RotateTransform? transform)
        {
            if (transform is null) return;
            transform.BeginAnimation(RotateTransform.AngleProperty, null);
        }

        private static bool TryGetDroppedPdfFile(DragEventArgs e, out string fileName)
        {
            fileName = string.Empty;
            string[]? dataText = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (dataText is null) return false;
            if (new FileInfo(dataText[0]).Extension != ".pdf") return false;
            fileName = dataText[0];
            return true;
        }

        private static bool TryGetDroppedPdfFiles(DragEventArgs e, out string[] fileNames)
        {
            fileNames = new string[] { };
            string[]? dataText = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (dataText is null) return false;
            IEnumerable<string> filteredData = dataText.Where((String s) =>
            {
                return new FileInfo(s).Extension == ".pdf";
            });

            fileNames = filteredData.ToArray();

            if (fileNames.Length == 0) return false;
            return true;
        }

        private async void HandlePdfTaskComplete(Task<Uri?> task, bool finalFile = true)
        {
            if (_data is null) throw new Exception("DataContext _data is null");

            Uri? newMessage = await task;
            if (newMessage is null)
            {
                UpdateUiOnError();
            }
            else
            {
                _data.UnlockedFileLocations.Add(newMessage.LocalPath);
            }

            if (finalFile)
            {
                UpdateUiOnSuccess();
                EndRotateAnim(_imageTransform);
            }
        }

        private void UpdateUiOnSuccess()
        {
            if (_data is null) return;
            if (_data.UnlockedFileLocations.Count > 1)
            {
                _data.MessageVisibility = Visibility.Visible;
                _data.Message = "Multiple files unlocked...";
            }
            else
            {
                _data.MessageVisibility = Visibility.Collapsed;
                _data.UnlockedFileLocation = _data.UnlockedFileLocations[0];
                _data.LinkVisibility = Visibility.Visible;
            }

            _data.ImageSource = new BitmapImage(new Uri("/Resources/unlocked-padlock.png", UriKind.Relative));
            _data.ButtonIsVisible = Visibility.Visible;
        }

        private void UpdateUiOnError()
        {
            if (_data is null) return;
            _data.MessageVisibility = Visibility.Visible;
            _data.Message = _data.DefaultMessage;
            _data.LinkVisibility = Visibility.Collapsed;
            _data.ImageSource = new BitmapImage(new Uri("/Resources/locked-padlock.png", UriKind.Relative));
            _data.ButtonIsVisible = Visibility.Hidden;
        }
    }
}