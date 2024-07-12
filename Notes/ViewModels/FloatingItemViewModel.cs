using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Notes.Models;

namespace Notes.ViewModels;

public class FloatingItemViewModel : ViewModelBase
{
    private MainWindowViewModel? _mainWindowViewModel;
    private LayoutViewModel? _layoutViewModel;

    private bool _isStatic;
    public bool IsStatic => _isStatic;

    public FloatingItem FloatingItem { get; }

    public double X
    {
        get => FloatingItem.X;
        set
        {
            if (Math.Abs(FloatingItem.X - value) < 0.0001f)
                return;
            if (_layoutViewModel == null)
                return;
            FloatingItem.X = Math.Clamp(value, 0, _layoutViewModel.CanvasWidth - Width);
            OnPropertyChanged();
        }
    }
    
    public double Y
    {
        get => FloatingItem.Y;
        set
        {
            if (Math.Abs(FloatingItem.Y - value) < 0.0001f)
                return;
            if (_layoutViewModel == null)
                return;
            FloatingItem.Y = Math.Clamp(value, 0, _layoutViewModel.CanvasHeight - Height);
            OnPropertyChanged();
        }
    }
    
    public double Width
    {
        get => FloatingItem.Width;
        set
        {
            if (Math.Abs(FloatingItem.Width - value) < 0.0001f)
                return;
            FloatingItem.Width = value;
            OnPropertyChanged();
        }
    }
    
    public double Height
    {
        get => FloatingItem.Height;
        set
        {
            if (Math.Abs(FloatingItem.Height - value) < 0.0001f)
                return;
            FloatingItem.Height = value;
            OnPropertyChanged();
        }
    }

    private ItemViewModel _itemViewModel;
    public ItemViewModel ItemViewModel => _itemViewModel;

    public FloatingItemViewModel(MainWindowViewModel mainWindowViewModel, FloatingItem floatingItem, IEnumerable<ItemViewModel> itemViewModelList)
    {
        _mainWindowViewModel = mainWindowViewModel;
        FloatingItem = floatingItem;
        _isStatic = true;

        foreach (var obj in itemViewModelList)
        {
            if (obj.Id != FloatingItem.Item.Guid) 
                continue;
            _itemViewModel = obj;
            break;
        }
    }
    
    public FloatingItemViewModel(LayoutViewModel layoutViewModel, FloatingItem floatingItem, IEnumerable<ItemViewModel> itemViewModelList)
    {
        _layoutViewModel = layoutViewModel;
        FloatingItem = floatingItem;
        _isStatic = false;

        foreach (var obj in itemViewModelList)
        {
            if (obj.Id != FloatingItem.Item.Guid) 
                continue;
            _itemViewModel = obj;
            break;
        }
    }

    public override void Dispose()
    {
        _layoutViewModel = null;
        _mainWindowViewModel = null;
    }

    private RelayCommand _preMoveCommand;
    public ICommand PreMoveCommand
    {
        get
        {
            if (_preMoveCommand == null)
                _preMoveCommand = new RelayCommand(param => StartMove(param as DragStartedEventArgs));
            return _preMoveCommand;
        }
    }
    public void StartMove(DragStartedEventArgs e)
    {
        if (!IsStatic)
        {
            if (_layoutViewModel == null)
                return; // Somehow
            int currentIndex = _layoutViewModel.FloatingItemViewModelList.IndexOf(this);
            _layoutViewModel.FloatingItemViewModelList.Move(currentIndex,
                _layoutViewModel.FloatingItemViewModelList.Count - 1);
            return;
        }

        
        if (_mainWindowViewModel.CurrentLayoutViewModel == null)
            return;
        _isStatic = false;
        
        _layoutViewModel = _mainWindowViewModel.CurrentLayoutViewModel;
        _mainWindowViewModel.SelectItem(null);
        _mainWindowViewModel.CurrentLayoutViewModel.FloatingItemViewModelList.Add(this);
        
        X = _layoutViewModel.ViewX;
        Y = _layoutViewModel.ViewY;
    }

    private RelayCommand _moveCommand;
    public ICommand MoveCommand
    {
        get
        {
            if (_moveCommand == null)
                _moveCommand = new RelayCommand(param => Move(param as DragDeltaEventArgs));
            return _moveCommand;
        }
    }
    public void Move(DragDeltaEventArgs e)
    {
        X += e.HorizontalChange;
        Y += e.VerticalChange;
    }
    
    private RelayCommand _resizeCommand;
    public ICommand ResizeCommand
    {
        get
        {
            if (_resizeCommand == null)
                _resizeCommand = new RelayCommand(param => Resize(param as DragDeltaEventArgs));
            return _resizeCommand;
        }
    }
    public void Resize(DragDeltaEventArgs e)
    {
        Width = Math.Clamp(Width + e.HorizontalChange, 250, 1000);
        Height = Math.Clamp(Height + e.VerticalChange, 250, 900);
    }

    private RelayCommand _closeCommand;
    public ICommand CloseCommand
    {
        get
        {
            if (_closeCommand == null)
                _closeCommand = new RelayCommand(param => Close());
            return _closeCommand;
        }
    }
    public void Close()
    {
        if (IsStatic)
        {
            _mainWindowViewModel.SelectItem(null);
        }
        else
        {
            _layoutViewModel.FloatingItemViewModelList.Remove(this);
        }
    }
}