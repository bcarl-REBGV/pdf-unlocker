using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PdfUnlockerGui;

/// <summary>
/// DataContext for MainWindow.xaml controls
/// </summary>
internal sealed class GuiData : DataBindingObject
{
    private string _message = "Drag and drop PDF files here";

    public string Message
    {
        get => _message;
        set => RaiseAndSet(ref _message, value);
    }

    private BitmapImage _imageSource = new BitmapImage(new Uri("/Resources/locked-padlock.png", UriKind.Relative));

    public BitmapImage ImageSource
    {
        get => _imageSource;
        set => RaiseAndSet(ref _imageSource, value);
    }

    private Brush _dropBorderBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 255));

    public Brush DropBorderBrush
    {
        get => _dropBorderBrush;
        set => RaiseAndSet(ref _dropBorderBrush, value);
    }

    private int _dropBorderThickness;

    public int DropBorderThickness
    {
        get => _dropBorderThickness;
        set => RaiseAndSet(ref _dropBorderThickness, value);
    }

    private Visibility _buttonIsVisible = Visibility.Hidden;

    public Visibility ButtonIsVisible
    {
        get => _buttonIsVisible;
        set => RaiseAndSet(ref _buttonIsVisible, value);
    }

    private string _unlockedFileLocation = string.Empty;

    public string UnlockedFileLocation
    {
        get => _unlockedFileLocation;
        set => RaiseAndSet(ref _unlockedFileLocation, value);
    }

    private Visibility _messageVisibility = Visibility.Visible;

    public Visibility MessageVisibility
    {
        get => _messageVisibility;
        set => RaiseAndSet(ref _messageVisibility, value);
    }

    private Visibility _linkVisibility = Visibility.Collapsed;
    public Visibility LinkVisibility
    {
        get => _linkVisibility;
        set => RaiseAndSet(ref _linkVisibility, value);
    }
}