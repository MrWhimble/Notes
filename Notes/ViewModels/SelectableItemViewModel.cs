using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Notes.Models;

namespace Notes.ViewModels;

public class SelectableItemViewModel : ViewModelBase
{
    private ItemViewModel _itemViewModel;
    private MainWindowViewModel _mainWindowViewModel;

    public string Name => _itemViewModel.Name;
    public IconViewModel IconViewModel => _itemViewModel.IconViewModel;
    public ObservableCollection<TagViewModel> TagViewModelList
    {
        get => _itemViewModel.TagViewModelList;
        set
        {
            _itemViewModel.TagViewModelList = value;
            OnPropertyChanged();
        }
    }

    public Item Item => _itemViewModel.Item;

    public bool IsSelected => _mainWindowViewModel.SelectedItemViewModel == this;
    public bool CanMoveUp => _mainWindowViewModel.SelectableItemViewModelList.IndexOf(this) > 0;
    public bool CanMoveDown => _mainWindowViewModel.SelectableItemViewModelList.IndexOf(this) < _mainWindowViewModel.SelectableItemViewModelList.Count - 1;

    public SelectableItemViewModel(ItemViewModel itemViewModel)
    {
        _itemViewModel = itemViewModel;
        _itemViewModel.PropertyChanged += ItemViewModelOnPropertyChanged;
        _mainWindowViewModel = _itemViewModel.MainWindowViewModel;
    }

    public override void Dispose()
    {
        _itemViewModel.PropertyChanged -= ItemViewModelOnPropertyChanged;
        _itemViewModel = null;
        _mainWindowViewModel = null;
    }

    private void ItemViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    public void UpdateIsSelected()
    {
        OnPropertyChanged(nameof(IsSelected));
    }

    private RelayCommand _selectCommand;
    public ICommand SelectCommand
    {
        get
        {
            if (_selectCommand == null)
                _selectCommand = new RelayCommand(param => Select());
            return _selectCommand;
        }
    }
    public void Select()
    {
        _mainWindowViewModel.SelectItem(this);
    }

    private RelayCommand _moveUpCommand;
    public ICommand MoveUpCommand
    {
        get
        {
            if (_moveUpCommand == null)
                _moveUpCommand = new RelayCommand(param => MoveUp());
            return _moveUpCommand;
        }
    }
    public void MoveUp()
    {
        _mainWindowViewModel.MoveItem(this, -1);
    }
    
    private RelayCommand _moveDownCommand;
    public ICommand MoveDownCommand
    {
        get
        {
            if (_moveDownCommand == null)
                _moveDownCommand = new RelayCommand(param => MoveDown());
            return _moveDownCommand;
        }
    }
    public void MoveDown()
    {
        _mainWindowViewModel.MoveItem(this, 1);
    }
    
    public void UpdateCanMoves()
    {
        OnPropertyChanged(nameof(CanMoveDown));
        OnPropertyChanged(nameof(CanMoveUp));
    }
}