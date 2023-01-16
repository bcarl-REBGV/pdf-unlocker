﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PdfUnlockerGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "UseNegatedPatternMatching")]
    public partial class MainWindow

    {
        private readonly GuiData? _data;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new GuiData();
            _data = this.DataContext as GuiData;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (_data is null) return;
            ProcessStartInfo startInfo = new(_data.UnlockedFileLocation) { UseShellExecute = true };
            Process.Start(startInfo);
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (_data is null) return;
            if (!TryGetDroppedPdfFile(e, out string fileName)) return;

            if (new FileInfo(fileName).Extension != ".pdf") return;

            Uri firstFileName = new Uri(fileName);

            Task<Uri?> unlockTask = PdfHandlerWrapper.UnlockPdf(firstFileName);
            _data.Message = $"Unlocking pdf: {firstFileName.Segments[^1]}";
            _data.ImageSource = new BitmapImage(new Uri("/Resources/spinner.gif", UriKind.Relative));
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

        private static bool TryGetDroppedPdfFile(DragEventArgs e, out string fileName )
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
            if (_data is null) return;
            Uri? newMessage = await task;
            if (newMessage is null)
            {
                UpdateUiOnError();
            }
            else
            {
                UpdateUiOnSuccess(newMessage);
            }
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
            _data.LinkVisibility = Visibility.Collapsed;
            _data.ImageSource = new BitmapImage(new Uri("/Resources/locked-padlock.png", UriKind.Relative));
            _data.ButtonIsVisible = Visibility.Hidden;
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (_data is null) return;
            Process.Start(new ProcessStartInfo("explorer", "/select," +  _data.UnlockedFileLocation));
        }
    }
}