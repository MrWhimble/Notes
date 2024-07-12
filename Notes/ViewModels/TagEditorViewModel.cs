using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Notes.ViewModels;

public class TagEditorViewModel : ViewModelBase
{
    private MainWindowViewModel _mainWindowViewModel;
    
    public ObservableCollection<TagViewModel> TagViewModelList => _mainWindowViewModel.TagViewModelList;

    private TagViewModel _selectedTagViewModel;
    public TagViewModel SelectedTagViewModel
    {
        get => _selectedTagViewModel;
        set
        {
            if (Equals(value, _selectedTagViewModel)) return;
            _selectedTagViewModel = value;
            OnPropertyChanged();
        }
    }

    public TagEditorViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        SelectedTagViewModel = null;
    }

    private RelayCommand _closedCommand;
    public ICommand ClosedCommand
    {
        get
        {
            if (_closedCommand == null)
                _closedCommand = new RelayCommand(param => HandleClose());
            return _closedCommand;
        }
    }
    public void HandleClose()
    {
        _mainWindowViewModel.TagEditorWindow = null;
    }

    private RelayCommand _addTagCommand;
    public ICommand AddTagCommand
    {
        get
        {
            if (_addTagCommand == null)
                _addTagCommand = new RelayCommand(param => AddTag());
            return _addTagCommand;
        }
    }
    public void AddTag()
    {
        _mainWindowViewModel.AddTag();
        SelectedTagViewModel = TagViewModelList[^1];
    }
    
    private RelayCommand _removeTagCommand;
    public ICommand RemoveTagCommand
    {
        get
        {
            if (_removeTagCommand == null)
                _removeTagCommand = new RelayCommand(param => RemoveTag());
            return _removeTagCommand;
        }
    }
    public void RemoveTag()
    {
        MessageBoxResult result = MessageBox.Show("Are you sure?\r\nThis action cannot be undone", "Delete Tag",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result is not MessageBoxResult.Yes)
            return;
        
        _mainWindowViewModel.RemoveTag(SelectedTagViewModel);
        SelectedTagViewModel = null;
    }
}