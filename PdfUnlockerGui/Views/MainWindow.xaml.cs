﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
            ProcessStartInfo startInfo = new ProcessStartInfo(_data.UnlockedFileLocation) { UseShellExecute = true };
            Process.Start(startInfo);
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (_data is null) return;
            if (!TryGetDroppedPdfFile(e, out string fileName))
            {
                throw new Exception("Invalid file was dropped.");
            }

            Uri firstFileName = new Uri(fileName);

            Task<Uri?> unlockTask = PdfHandlerWrapper.UnlockPdf(firstFileName);
            _data.Message = $"Unlocking pdf: {Uri.UnescapeDataString(firstFileName.Segments[^1])}";
            _data.ImageSource = new BitmapImage(new Uri("/Resources/spinner.gif", UriKind.Relative));
            StartRotateAnim(_imageTransform);

            HandlePdfTaskComplete(unlockTask);

            e.Handled = true;
        }

        private void MainWindow_OnDragEnterOver(object sender, DragEventArgs e)
        {
            if (!TryGetDroppedPdfFile(e, out string fileName))
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

        private async void HandlePdfTaskComplete(Task<Uri?> task)
        {
            if (_data is null) throw new Exception("DataContext _data is null");

            Uri? newMessage = await task;
            if (newMessage is null)
            {
                UpdateUiOnError();
            }
            else
            {
                UpdateUiOnSuccess(newMessage);
            }

            EndRotateAnim(_imageTransform);
        }

        private void UpdateUiOnSuccess(Uri newMessage)
        {
            if (_data is null) return;
            _data.MessageVisibility = Visibility.Collapsed;
            _data.LinkVisibility = Visibility.Visible;
            _data.UnlockedFileLocation = newMessage.LocalPath;
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