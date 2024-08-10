using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Notes.Extensions;
using Notes.Models;

namespace Notes.ViewModels;

public class NoteViewModel : ViewModelBase
{
    private ItemViewModel _itemViewModel;
    
    public Note Note { get; }

    public string Heading
    {
        get => Note.Heading;
        set
        {
            if (Note.Heading.Equals(value))
                return;
            Note.Heading = value;
            OnPropertyChanged();
        }
    }
    
    public string Content
    {
        get => Note.Content;
        set
        {
            if (Note.Content.Equals(value))
                return;
            Note.Content = value;
            OnPropertyChanged();
        }
    }
    
    public bool Collapse
    {
        get => Note.Collapse;
        set
        {
            if (Note.Collapse == value)
                return;
            Note.Collapse = value;
            OnPropertyChanged();
        }
    }
    
    public bool Expand
    {
        get => !Note.Collapse;
        set
        {
            if (Note.Collapse == !value)
                return;
            Note.Collapse = !value;
            OnPropertyChanged();
        }
    }

    public ImageViewModel ImageViewModel { get; private set; }

    public Dock ImageDisplayDock
    {
        get => Note.ImageDisplaySide.ToDock();
        set
        {
            if (Note.ImageDisplaySide.ToDock() == value)
                return;
            Note.ImageDisplaySide = value.ToSide();
            OnPropertyChanged();
            OnPropertyChanged(nameof(ImageMaxWidth));
            OnPropertyChanged(nameof(ImageMaxHeight));
        }
    }

    public double ImageSize
    {
        get => Note.ImageSize;
        set
        {
            Note.ImageSize = Math.Max(0, value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(ImageMaxWidth));
            OnPropertyChanged(nameof(ImageMaxHeight));
        }
    }
    
    public double ImageMaxWidth => ImageDisplayDock is Dock.Left or Dock.Right ? ImageSize : double.MaxValue;
    public double ImageMaxHeight => ImageDisplayDock is Dock.Top or Dock.Bottom ? ImageSize : double.MaxValue;

    public bool CanMoveUp => _itemViewModel.NoteViewModelList.IndexOf(this) > 0;
    public bool CanMoveDown => _itemViewModel.NoteViewModelList.IndexOf(this) < _itemViewModel.NoteViewModelList.Count - 1;

    public NoteViewModel(ItemViewModel itemViewModel, Note note)
    {
        _itemViewModel = itemViewModel;
        Note = note;
        ImageViewModel = new ImageViewModel(Note.ImageUriString, _itemViewModel.MainWindowViewModel);
        ImageViewModel.PropertyChanged += OnImagePropertyChanged;
    }

    public override void Dispose()
    {
        _itemViewModel = null;
        ImageViewModel.PropertyChanged -= OnImagePropertyChanged;
        ImageViewModel.Dispose();
        ImageViewModel = null;
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
        int index = _itemViewModel.NoteViewModelList.IndexOf(this);
        if (index == 0)
            return;
        
        NoteViewModel otherNoteViewModel = _itemViewModel.NoteViewModelList[index - 1];
        
        _itemViewModel.NoteViewModelList.Move(index, index - 1);

        otherNoteViewModel.UpdateCanMoves();
        UpdateCanMoves();
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
        int index = _itemViewModel.NoteViewModelList.IndexOf(this);
        if (index == _itemViewModel.NoteViewModelList.Count - 1)
            return;
        
        NoteViewModel otherNoteViewModel = _itemViewModel.NoteViewModelList[index + 1];
        
        _itemViewModel.NoteViewModelList.Move(index, index + 1);
        
        otherNoteViewModel.UpdateCanMoves();
        UpdateCanMoves();
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
        MessageBoxResult result = MessageBox.Show("Are you sure?\r\nThis action cannot be undone", "Delete Note",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result is not MessageBoxResult.Yes)
            return;
        
        _itemViewModel.NoteViewModelList.Remove(this);
        if (_itemViewModel.NoteViewModelList.Count == 0)
            return;
        
        _itemViewModel.NoteViewModelList[0].UpdateCanMoves();
        _itemViewModel.NoteViewModelList[^1].UpdateCanMoves();
    }

    private RelayCommand _editImageCommand;
    public ICommand EditImageCommand
    {
        get
        {
            if (_editImageCommand == null)
                _editImageCommand = new RelayCommand(param => EditImage());
            return _editImageCommand;
        }
    }
    public void EditImage()
    {
        ImageViewModel.OpenEditor();
    }

    private RelayCommand _setImageDockCommand;
    public ICommand SetImageDockCommand
    {
        get
        {
            if (_setImageDockCommand == null)
                _setImageDockCommand = new RelayCommand(param => SetImageDock((Dock)param));
            return _setImageDockCommand;
        }
    }
    public void SetImageDock(Dock dock)
    {
        ImageDisplayDock = dock;
    }

    private RelayCommand _imageResizeCommand;
    public ICommand ImageResizeCommand
    {
        get
        {
            if (_imageResizeCommand == null)
                _imageResizeCommand = new RelayCommand(param => ResizeImage(param as DragDeltaEventArgs));
            return _imageResizeCommand;
        }
    }

    public void ResizeImage(DragDeltaEventArgs e)
    {
        double change = ImageDisplayDock is Dock.Left or Dock.Right ? e.HorizontalChange : e.VerticalChange;
        if (ImageDisplayDock is Dock.Right)
            change *= -1;
        ImageSize += change;
    }

    public void UpdateCanMoves()
    {
        OnPropertyChanged(nameof(CanMoveDown));
        OnPropertyChanged(nameof(CanMoveUp));
    }
    
    private void OnImagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ImageViewModel.ImageUriString))
            return;

        if (sender is not ImageViewModel imageViewModel)
            return;
        
        string newUriString = imageViewModel.ImageUriString;
        Note.ImageUriString = newUriString;
    }
}