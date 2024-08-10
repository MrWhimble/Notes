using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Notes.Views;

namespace Notes.ViewModels;

public class ImageViewModel : ViewModelBase
{
    private MainWindowViewModel _mainWindowViewModel;

    private string _imageUriString;
    public string ImageUriString
    {
        get => _imageUriString;
        set
        {
            _imageUriString = value;
            OnPropertyChanged();
            UpdateImage();
        }
    }

    private ImageState _imageState;
    public ImageState ImageState
    {
        get => _imageState;
        private set
        {
            _imageState = value;
            OnPropertyChanged();
        }
    }
    
    public BitmapImage Image { get; private set; }

    private int _downloadingProgress;

    public int DownloadingProgress
    {
        get => _downloadingProgress;
        private set
        {
            _downloadingProgress = value;
            OnPropertyChanged();
        }
    }

    private bool _hasEvents;

    public ImageViewModel(string initialUri, MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _imageUriString = initialUri;
        UpdateImage();
    }

    public override void Dispose()
    {
        UpdateImageEvents(false);
        Image = null;
        ImageState = ImageState.Blank;
    }

    private void UpdateImage()
    {
        DownloadingProgress = 0;
        UpdateImageEvents(false);

        if (string.IsNullOrWhiteSpace(ImageUriString) ||
            !Uri.TryCreate(ImageUriString, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            Image = new BitmapImage();
            ImageState = string.IsNullOrWhiteSpace(ImageUriString) ? ImageState.Blank : ImageState.UriError;
            base.OnPropertyChanged(nameof(Image));
            return;
        }

        if (!uri.IsAbsoluteUri && !string.IsNullOrWhiteSpace(_mainWindowViewModel.LoadedFilePath))
        {
            uri = new Uri($"{_mainWindowViewModel.LoadedFilePath}/{uri}");
        }

        if (uri.IsFile && !File.Exists(uri.LocalPath))
        {
            Image = new BitmapImage();
            ImageState = ImageState.UriError;
            base.OnPropertyChanged(nameof(Image));
            return;
        }

        Image = new BitmapImage();
        ImageState = ImageState.Loading;
        UpdateImageEvents(true);
        Image.BeginInit();
        Image.UriSource = uri;
        Image.DecodePixelWidth = 512;
        Image.EndInit();
        base.OnPropertyChanged(nameof(Image));
        if (!Image.IsDownloading)
            ImageState = ImageState.Loaded;
    }
    
    private void UpdateImageEvents(bool newState)
    {
        if (_hasEvents)
        {
            Image.DownloadProgress -= OnDownloadProgress;
            Image.DownloadCompleted -= OnDownloadCompleted;
            Image.DownloadFailed -= OnDownloadFailed;
        }

        if (newState)
        {
            Image.DownloadProgress += OnDownloadProgress;
            Image.DownloadCompleted += OnDownloadCompleted;
            Image.DownloadFailed += OnDownloadFailed;
        }
        
        _hasEvents = newState;
    }

    private void OnDownloadProgress(object? sender, DownloadProgressEventArgs e)
    {
        DownloadingProgress = e.Progress;
    }
    
    private void OnDownloadCompleted(object? sender, EventArgs e)
    {
        ImageState = ImageState.Loaded;
        DownloadingProgress = 100;
    }

    private void OnDownloadFailed(object? sender, ExceptionEventArgs e)
    {
        ImageState = ImageState.UriError;
        DownloadingProgress = 0;
    }

    public void OpenEditor()
    {
        EditImageViewModel editIconViewModel = new EditImageViewModel(this, _mainWindowViewModel);
        EditImageWindow editImageWindow = new EditImageWindow();
        editImageWindow.DataContext = editIconViewModel;
        editImageWindow.ShowDialog();
    }
}