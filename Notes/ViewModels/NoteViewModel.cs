using System.Windows;
using System.Windows.Input;
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

    public bool CanMoveUp => _itemViewModel.NoteViewModelList.IndexOf(this) > 0;
    public bool CanMoveDown => _itemViewModel.NoteViewModelList.IndexOf(this) < _itemViewModel.NoteViewModelList.Count - 1;

    public NoteViewModel(ItemViewModel itemViewModel, Note note)
    {
        _itemViewModel = itemViewModel;
        Note = note;
    }

    public override void Dispose()
    {
        _itemViewModel = null;
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

    public void UpdateCanMoves()
    {
        OnPropertyChanged(nameof(CanMoveDown));
        OnPropertyChanged(nameof(CanMoveUp));
    }
}