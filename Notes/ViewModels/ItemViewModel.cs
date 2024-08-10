using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Notes.Models;
using Notes.Views;

namespace Notes.ViewModels;

public class ItemViewModel : ViewModelBase
{ 
    public MainWindowViewModel MainWindowViewModel { get; }
    
    public Item Item { get; }
    
    public Guid Id => Item.Guid;
    
    public string Name
    {
        get => Item.Name;
        set
        {
            if (value.Equals(Item.Name)) return;
            Item.Name = value;
            base.OnPropertyChanged();
        }
    }
    
    //public IconViewModel IconViewModel { get; private set; }
    public ImageViewModel ImageViewModel { get; private set; }

    private ObservableCollection<TagViewModel> _tagViewModelList;

    public ObservableCollection<TagViewModel> TagViewModelList
    {
        get => _tagViewModelList;
        set
        {
            _tagViewModelList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<NoteViewModel> _noteViewModelList;
    public ObservableCollection<NoteViewModel> NoteViewModelList 
    { 
        get => _noteViewModelList;
        set
        {
            _noteViewModelList = value;
            OnPropertyChanged();
        } 
    }
    
    private bool _isSelected;
    public bool IsSelected 
    { 
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;
            _isSelected = value;
            OnPropertyChanged();
        } 
    }

    private bool _initialized;
    
    public ItemViewModel(Item item, MainWindowViewModel mainWindowViewModel)
    {
        _initialized = false;

        MainWindowViewModel = mainWindowViewModel;
        
        Item = item;

        TagViewModelList = new ObservableCollection<TagViewModel>();
        TagViewModelList.CollectionChanged += OnTagViewModelListChanged;
        foreach (Tag tag in Item.Tags)
        {
            if (!MainWindowViewModel.TryGetTagViewModel(tag, out TagViewModel? tagViewModel))
                continue;
            TagViewModelList.Add(tagViewModel);
        }

        NoteViewModelList = new ObservableCollection<NoteViewModel>();
        NoteViewModelList.CollectionChanged += OnNoteViewModelListChanged;
        foreach (Note note in Item.Notes)
        {
            NoteViewModelList.Add(new NoteViewModel(this, note));
        }
        
        _initialized = true;

        //IconViewModel = new IconViewModel(Item, MainWindowViewModel);
        //IconViewModel.IconUriString = item.IconUriString;

        ImageViewModel = new ImageViewModel(Item.IconUriString, MainWindowViewModel);
        ImageViewModel.PropertyChanged += OnImagePropertyChanged;
    }

    public override void Dispose()
    {
        base.Dispose();
        
        TagViewModelList.CollectionChanged -= OnTagViewModelListChanged;
        foreach (var viewModel in TagViewModelList)
            viewModel.Dispose();
        TagViewModelList.Clear();
        
        NoteViewModelList.CollectionChanged -= OnNoteViewModelListChanged;
        foreach (var viewModel in NoteViewModelList)
            viewModel.Dispose();
        NoteViewModelList.Clear();

        ImageViewModel.PropertyChanged -= OnImagePropertyChanged;
        ImageViewModel.Dispose();
    }
    
    private void OnImagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ImageViewModel.ImageUriString))
            return;

        if (sender is not ImageViewModel imageViewModel)
            return;
        
        string newUriString = imageViewModel.ImageUriString;
        Item.IconUriString = newUriString;
    }
    
    private void OnTagViewModelListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_initialized)
            return;
        
        if (e.OldItems != null && e.OldItems.Count != 0)
            foreach (TagViewModel tagViewModel in e.OldItems)
                Item.Tags.Remove(tagViewModel.Tag);
        
        if (e.NewItems != null && e.NewItems.Count != 0)
            foreach (TagViewModel tagViewModel in e.NewItems)
                Item.Tags.Add(tagViewModel.Tag);
    }
    
    private void OnNoteViewModelListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_initialized)
            return;

        if (e.OldItems != null && e.OldItems.Count != 0)
            foreach (NoteViewModel noteViewModel in e.OldItems)
                Item.Notes.Remove(noteViewModel.Note);
        
        if (e.NewItems != null && e.NewItems.Count != 0)
            foreach (NoteViewModel noteViewModel in e.NewItems)
                Item.Notes.Add(noteViewModel.Note);
    }

    private RelayCommand _editIconCommand;
    public ICommand EditIconCommand
    {
        get
        {
            if (_editIconCommand == null)
                _editIconCommand = new RelayCommand(param => EditIcon());
            return _editIconCommand;
        }
    }
    public void EditIcon()
    {
        ImageViewModel.OpenEditor();
    }

    private RelayCommand _editTagsCommand;
    public ICommand EditTagsCommand
    {
        get
        {
            if (_editTagsCommand == null)
                _editTagsCommand = new RelayCommand(param => EditTags());
            return _editTagsCommand;
        }
    }
    public void EditTags()
    {
        EditTagListViewModel editTagListViewModel = new EditTagListViewModel(MainWindowViewModel, this);
        EditTagListWindow editTagListWindow = new EditTagListWindow();
        editTagListWindow.DataContext = editTagListViewModel;
        editTagListWindow.ShowDialog();
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
        MessageBoxResult result = MessageBox.Show("Are you sure?\r\nThis action cannot be undone", "Delete Item",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result is not MessageBoxResult.Yes)
            return;
        
        MainWindowViewModel.RemoveItem(this);
    }
    
    private RelayCommand _newNoteCommand;
    public ICommand NewNoteCommand
    {
        get
        {
            if (_newNoteCommand == null)
                _newNoteCommand = new RelayCommand(param => NewNote());
            return _newNoteCommand;
        }
    }
    public void NewNote()
    {
        NoteViewModel noteViewModel = new NoteViewModel(this, new Note());
        NoteViewModelList.Add(noteViewModel);
        if (NoteViewModelList.Count > 1)
            NoteViewModelList[^2].UpdateCanMoves();
    }
    
}