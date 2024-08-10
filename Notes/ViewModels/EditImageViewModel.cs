using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace Notes.ViewModels;

public class EditImageViewModel : ViewModelBase
{
    private MainWindowViewModel _mainWindowViewModel;
    public ImageViewModel ImageViewModel { get; }

    public string ImageUriString
    {
        get => ImageViewModel.ImageUriString;
        set
        {
            if (Equals(value, ImageViewModel.ImageUriString))
                return;
            ImageViewModel.ImageUriString = value;
            OnPropertyChanged();
        }
    }
    
    private static readonly OpenFileDialog OpenFileDialog = new ();
    
    public EditImageViewModel(ImageViewModel imageViewModel, MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        ImageViewModel = imageViewModel;
        
        OpenFileDialog.Multiselect = false;
        OpenFileDialog.Title = "Select Image";
        OpenFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
    }

    public override void Dispose()
    {
        _mainWindowViewModel = null;
    }

    private RelayCommand _confirmCommand;
    public ICommand ConfirmCommand
    {
        get
        {
            if (_confirmCommand == null)
                _confirmCommand = new RelayCommand(param => Confirm(param as TextBox));
            return _confirmCommand;
        }
    }
    public void Confirm(TextBox textBox)
    {
        textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
    }

    private RelayCommand _openExplorerCommand;
    public ICommand OpenExplorerCommand
    {
        get
        {
            if (_openExplorerCommand == null)
                _openExplorerCommand = new RelayCommand(param => OpenExplorer());
            return _openExplorerCommand;
        }
    }
    public void OpenExplorer()
    {
        bool? result = OpenFileDialog.ShowDialog();
        if (!result.HasValue || !result.Value)
            return;

        string filePath = OpenFileDialog.FileName;
        if (!string.IsNullOrWhiteSpace(_mainWindowViewModel.LoadedFilePath) &&
            filePath.StartsWith(_mainWindowViewModel.LoadedFilePath))
            filePath = filePath.Substring(_mainWindowViewModel.LoadedFilePath.Length + 1);

        ImageUriString = filePath;
    }
}