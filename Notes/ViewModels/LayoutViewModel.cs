using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Notes.Models;

namespace Notes.ViewModels;

public class LayoutViewModel : ViewModelBase
{
    private MainWindowViewModel _mainWindowViewModel;
    
    public Layout Layout { get; }

    public Guid Guid => Layout.Guid;
    
    public string Name
    {
        get => Layout.Name;
        set
        {
            if (value.Equals(Layout.Name)) return;
            Layout.Name = value;
            base.OnPropertyChanged();
        }
    }
    
    public double ViewX
    {
        get => Layout.ViewX;
        set
        {
            if (value.Equals(Layout.ViewX)) return;
            Layout.ViewX = value;
            base.OnPropertyChanged();
        }
    }
    
    public double ViewY
    {
        get => Layout.ViewY;
        set
        {
            if (value.Equals(Layout.ViewY)) return;
            Layout.ViewY = value;
            base.OnPropertyChanged();
        }
    }
    
    public double CanvasWidth
    {
        get => Layout.CanvasWidth;
        set
        {
            if (value.Equals(Layout.CanvasWidth)) return;
            Layout.CanvasWidth = value;
            base.OnPropertyChanged();
        }
    }
    
    public double CanvasHeight
    {
        get => Layout.CanvasHeight;
        set
        {
            if (value.Equals(Layout.CanvasHeight)) return;
            Layout.CanvasHeight = value;
            base.OnPropertyChanged();
        }
    }

    private ObservableCollection<FloatingItemViewModel> _floatingItemViewModelList;

    public ObservableCollection<FloatingItemViewModel> FloatingItemViewModelList
    {
        get => _floatingItemViewModelList;
        set
        {
            _floatingItemViewModelList = value;
            OnPropertyChanged();
        }
    }

    private bool _isRenaming;
    public bool IsRenaming
    {
        get => _isRenaming;
        set
        {
            _isRenaming = value;
            OnPropertyChanged();
        }
    }

    private string _bufferName;
    public string BufferName
    {
        get => _bufferName;
        set
        {
            _bufferName = value;
            OnPropertyChanged();
        }
    }

    public bool CanMoveLeft => _mainWindowViewModel.LayoutViewModelList.IndexOf(this) > 0;
    public bool CanMoveRight => _mainWindowViewModel.LayoutViewModelList.IndexOf(this) < _mainWindowViewModel.LayoutViewModelList.Count - 1;
    
    

    private bool _initialized;
    private bool _updateViewXY;

    public LayoutViewModel(Layout layout, MainWindowViewModel mainWindowViewModel)
    {
        _initialized = false;
        
        _mainWindowViewModel = mainWindowViewModel;
        
        Layout = layout;
        
        BufferName = Name;

        FloatingItemViewModelList = new ObservableCollection<FloatingItemViewModel>();
        FloatingItemViewModelList.CollectionChanged += OnFloatingItemViewModelListChanged;
        foreach (FloatingItem floatingItem in Layout.FloatingItems)
        {
            FloatingItemViewModelList.Add(new FloatingItemViewModel(this, floatingItem, _mainWindowViewModel.ItemViewModelList));
        }

        _updateViewXY = true;
        
        _initialized = true;
    }

    public override void Dispose()
    {
        _initialized = false;

        FloatingItemViewModelList.CollectionChanged -= OnFloatingItemViewModelListChanged;
        foreach (var viewModel in FloatingItemViewModelList)
            viewModel.Dispose();
        FloatingItemViewModelList.Clear();

        _mainWindowViewModel = null;
    }

    private RelayCommand _removeCommand;
    public ICommand RemoveCommand
    {
        get
        {
            if (_removeCommand == null)
                _removeCommand = new RelayCommand(param => Remove());
            return _removeCommand;
        }
    }
    public void Remove()
    {
        MessageBoxResult result = MessageBox.Show("Are you sure?\r\nThis action cannot be undone", "Delete Layout",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result is not MessageBoxResult.Yes)
            return;
        
        FloatingItemViewModelList.Clear();
        _mainWindowViewModel.RemoveLayout(this);
    }

    private RelayCommand _renameCommand;
    public ICommand RenameCommand
    {
        get
        {
            if (_renameCommand == null)
                _renameCommand = new RelayCommand(param => Rename());
            return _renameCommand;
        }
    }
    public void Rename()
    {
        if (IsRenaming)
            return;
        BufferName = Name;
        IsRenaming = true;
    }
    
    private RelayCommand _confirmRenameCommand;
    public ICommand ConfirmRenameCommand
    {
        get
        {
            if (_confirmRenameCommand == null)
                _confirmRenameCommand = new RelayCommand(param => ConfirmRename());
            return _confirmRenameCommand;
        }
    }
    public void ConfirmRename()
    {
        if (!IsRenaming)
            return;
        if (string.IsNullOrWhiteSpace(BufferName))
        {
            System.Media.SystemSounds.Exclamation.Play();
            return;
        }
        Name = BufferName;
        IsRenaming = false;
    }
    
    private RelayCommand _cancelRenameCommand;
    public ICommand CancelRenameCommand
    {
        get
        {
            if (_cancelRenameCommand == null)
                _cancelRenameCommand = new RelayCommand(param => CancelRename());
            return _cancelRenameCommand;
        }
    }
    public void CancelRename()
    {
        if (!IsRenaming)
            return;
        IsRenaming = false;
    }

    private RelayCommand _moveLeftCommand;
    public ICommand MoveLeftCommand
    {
        get
        {
            if (_moveLeftCommand == null)
                _moveLeftCommand = new RelayCommand(param => MoveLeft());
            return _moveLeftCommand;
        }
    }
    public void MoveLeft()
    {
        _mainWindowViewModel.MoveLayout(this, -1);
    }
    
    private RelayCommand _moveRightCommand;
    public ICommand MoveRightCommand
    {
        get
        {
            if (_moveRightCommand == null)
                _moveRightCommand = new RelayCommand(param => MoveRight());
            return _moveRightCommand;
        }
    }
    public void MoveRight()
    {
        _mainWindowViewModel.MoveLayout(this, 1);
    }

    private RelayCommand _mouseWheelCommand;
    public ICommand MouseWheelCommand
    {
        get
        {
            if (_mouseWheelCommand == null)
                _mouseWheelCommand = new RelayCommand(param => MouseWheel(param as MouseWheelEventArgs));
            return _mouseWheelCommand;
        }
    }
    public void MouseWheel(MouseWheelEventArgs e)
    {
        ScrollViewer scrollViewer = (ScrollViewer)((FrameworkElement)e.Source).Parent;
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true;
        }
    }
    
    private void OnFloatingItemViewModelListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_initialized)
            return;
        
        if (e.OldItems != null && e.OldItems.Count != 0)
            foreach (FloatingItemViewModel floatingViewViewModel in e.OldItems)
                Layout.FloatingItems.Remove(floatingViewViewModel.FloatingItem);
        
        if (e.NewItems != null && e.NewItems.Count != 0)
            foreach (FloatingItemViewModel floatingViewViewModel in e.NewItems)
                Layout.FloatingItems.Add(floatingViewViewModel.FloatingItem);
    }

    public void RemoveItem(ItemViewModel itemViewModel)
    {
        for (var i = FloatingItemViewModelList.Count - 1; i >= 0; i--)
        {
            var floatingViewItemModel = FloatingItemViewModelList[i];
            if (floatingViewItemModel.ItemViewModel == itemViewModel)
                FloatingItemViewModelList.Remove(floatingViewItemModel);
        }
    }

    public void UpdateCanMoves()
    {
        OnPropertyChanged(nameof(CanMoveLeft));
        OnPropertyChanged(nameof(CanMoveRight));
    }
}