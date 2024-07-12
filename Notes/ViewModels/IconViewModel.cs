using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Notes.Models;
using Notes.Views;

namespace Notes.ViewModels;

public class IconViewModel : ViewModelBase
{
    private Item _item;
    private MainWindowViewModel _mainWindowViewModel;
    
    public string IconUriString
    {
        get => _item.IconUriString;
        set
        {
            _item.IconUriString = value;
            OnPropertyChanged();
            UpdateIcon();
        }
    }

    private IconState _iconState;
    public IconState IconState
    {
        get => _iconState;
        private set
        {
            _iconState = value; 
            OnPropertyChanged();
        }
    }

    public BitmapImage Icon { get; private set; }

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

    public IconViewModel(Item item, MainWindowViewModel mainWindowViewModel)
    {
        _item = item;
        _mainWindowViewModel = mainWindowViewModel;
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        DownloadingProgress = 0;
        UpdateIconEvents(false);
        
        if (String.IsNullOrWhiteSpace(IconUriString) || !Uri.TryCreate(IconUriString, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            Icon = new BitmapImage();
            IconState = string.IsNullOrWhiteSpace(IconUriString) ? IconState.Blank : IconState.UriError;
            base.OnPropertyChanged(nameof(Icon));
            return;
        }
        
        if (!uri.IsAbsoluteUri && !String.IsNullOrWhiteSpace(_mainWindowViewModel.LoadedFilePath))
        {
            uri = new Uri($"{_mainWindowViewModel.LoadedFilePath}/{uri}");
        }
        
        if (uri.IsFile && !File.Exists(uri.LocalPath))
        {
            Icon = new BitmapImage();
            IconState = IconState.UriError;
            base.OnPropertyChanged(nameof(Icon));
            return;
        }
        
        Icon = new BitmapImage();
        IconState = IconState.Loading;
        UpdateIconEvents(true);
        Icon.BeginInit();
        Icon.UriSource = uri;
        Icon.DecodePixelWidth = 512;
        Icon.EndInit();
        base.OnPropertyChanged(nameof(Icon));
        if (!Icon.IsDownloading)
            IconState = IconState.Loaded;
    }

    private void UpdateIconEvents(bool newState)
    {
        if (_hasEvents)
        {
            Icon.DownloadProgress -= OnDownloadProgress;
            Icon.DownloadCompleted -= OnDownloadCompleted;
            Icon.DownloadFailed -= OnDownloadFailed;
        }

        if (newState)
        {
            Icon.DownloadProgress += OnDownloadProgress;
            Icon.DownloadCompleted += OnDownloadCompleted;
            Icon.DownloadFailed += OnDownloadFailed;
        }
        
        _hasEvents = newState;
    }

    private void OnDownloadProgress(object? sender, DownloadProgressEventArgs e)
    {
        DownloadingProgress = e.Progress;
    }
    
    private void OnDownloadCompleted(object? sender, EventArgs e)
    {
        IconState = IconState.Loaded;
        DownloadingProgress = 100;
    }

    private void OnDownloadFailed(object? sender, ExceptionEventArgs e)
    {
        IconState = IconState.UriError;
        DownloadingProgress = 0;
    }

    public void OpenEditor()
    {
        EditIconViewModel editIconViewModel = new EditIconViewModel(this, _mainWindowViewModel);
        EditIconWindow editIconWindow = new EditIconWindow();
        editIconWindow.DataContext = editIconViewModel;
        editIconWindow.ShowDialog();
    }
}