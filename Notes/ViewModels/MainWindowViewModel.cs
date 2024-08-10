using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using Notes.Models;
using Notes.Views;

namespace Notes.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Repository<Guid, Tag> _tagRepository = new ();
    private readonly Repository<Guid, Item, Tag> _itemRepository = new ();
    private readonly Repository<Guid, Layout, Item> _layoutRepository = new ();
    
    private readonly OpenFolderDialog _openFolderDialog = new();

    public string LoadedFilePath { get; private set; } = string.Empty;
    
    public TagEditorWindow? TagEditorWindow { get; set; }
    
    public ObservableCollection<TagViewModel> TagViewModelList { get; } = new();
    public ObservableCollection<FilterTagViewModel> FilterTagViewModelList { get; } = new();
    public ObservableCollection<ItemViewModel> ItemViewModelList { get; } = new();

    private ObservableCollection<SelectableItemViewModel> _selectableItemViewModelList;

    public ObservableCollection<SelectableItemViewModel> SelectableItemViewModelList
    {
        get
        {
            if (_selectableItemViewModelList == null)
            {
                _selectableItemViewModelList = new ObservableCollection<SelectableItemViewModel>();
            }
            return _selectableItemViewModelList;
        }
    }

    public CollectionViewSource SelectableItemViewModelListViewSource { get; set; }

    private ObservableCollection<LayoutViewModel> _layoutViewModelList;
    public ObservableCollection<LayoutViewModel> LayoutViewModelList
    {
        get
        {
            if (_layoutViewModelList == null)
            {
                _layoutViewModelList = new ObservableCollection<LayoutViewModel>();
                IEditableCollectionView layoutViewModelListView =
                    (IEditableCollectionView)CollectionViewSource.GetDefaultView(_layoutViewModelList);
                layoutViewModelListView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            }
            return _layoutViewModelList;
        }
    }

    private LayoutViewModel _currentLayoutViewModel;

    public LayoutViewModel CurrentLayoutViewModel
    {
        get => _currentLayoutViewModel;
        set
        {
            _currentLayoutViewModel = value;
            OnPropertyChanged();
        }
    }

    private SelectableItemViewModel _selectedItemViewModel;

    public SelectableItemViewModel SelectedItemViewModel
    {
        get => _selectedItemViewModel;
        set
        {
            
            SelectableItemViewModel oldSelectableItemViewModel = _selectedItemViewModel;
            _selectedItemViewModel = value;
            OnSelectedItemViewModelChanged(oldSelectableItemViewModel, _selectedItemViewModel);
        }
    }

    private FloatingItemViewModel _staticFloatingItemViewModel;

    public FloatingItemViewModel StaticFloatingItemViewModel
    {
        get => _staticFloatingItemViewModel;
        set
        {
            _staticFloatingItemViewModel = value;
            OnPropertyChanged();
        }
    }

    private string _searchQuery = string.Empty;

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (Equals(value, _searchQuery))
                return;
            _searchQuery = value;
            OnPropertyChanged();
            UpdateVisibleItems();
        }
    }

    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public void SetTitle(string msg)
    {
        Title = Title = $"Notes | {DateTime.Now.ToShortTimeString()} | {msg}";
    }

    

    private bool _canInteract;
    
    public MainWindowViewModel()
    {
        SetTitle("Hello :)");
        
        _canInteract = true;
    }
    
    private RelayCommand _closingCommand;
    public ICommand ClosingCommand
    {
        get
        {
            if (_closingCommand == null)
                _closingCommand = new RelayCommand(param => HandleClosing(param as CancelEventArgs));
            return _closingCommand;
        }
    }
    public void HandleClosing(CancelEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("Are you sure?\r\nAny unsaved changes will be lost", "Close",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        e.Cancel = result is not MessageBoxResult.Yes;
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
        if (TagEditorWindow != null)
            TagEditorWindow.Close();
    }
    
    private RelayCommand _newCommand;
    public ICommand NewCommand
    {
        get
        {
            if (_newCommand == null)
                _newCommand = new RelayCommand(param => NewBoard());
            return _newCommand;
        }
    }
    public void NewBoard()
    {
        MessageBoxResult result = MessageBox.Show("Are you sure?\r\nAny unsaved changes will be lost", "Create new Board",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result is not MessageBoxResult.Yes)
            return;
        
        _canInteract = false;
        
        _tagRepository.List.Clear();
        _itemRepository.List.Clear();
        _layoutRepository.List.Clear();
        
        ClearViewModelLists();

        CurrentLayoutViewModel = null;

        LoadedFilePath = String.Empty;

        SetTitle("Create New Board");
        
        _canInteract = true;
    }
    
    private RelayCommand _openCommand;
    public ICommand OpenCommand
    {
        get
        {
            if (_openCommand == null)
                _openCommand = new RelayCommand(param => OpenBoard());
            return _openCommand;
        }
    }
    public void OpenBoard()
    {
        _openFolderDialog.Title = "Open";
        _openFolderDialog.Multiselect = false;

        bool? result = _openFolderDialog.ShowDialog();
        if (result == null || !result.Value)
            return;

        _canInteract = false;
        
        string folderPath = _openFolderDialog.FolderName;
        MethodResult loadResult = _tagRepository.Load($"{folderPath}\\TAGS.json");
        loadResult = loadResult.Combine(_itemRepository.Load($"{folderPath}\\ITEMS.json", _tagRepository.List));
        loadResult = loadResult.Combine(_layoutRepository.Load($"{folderPath}\\LAYOUTS.json", _itemRepository.List));
        
        LoadedFilePath = folderPath;
        
        ClearViewModelLists();
        
        foreach (var obj in _tagRepository.List)
        {
            TagViewModel tagViewModel = new TagViewModel(obj);
            TagViewModelList.Add(tagViewModel);
            FilterTagViewModelList.Add(new FilterTagViewModel(tagViewModel, this));
        }

        FilterTagViewModel.TotalRequiredFilters = 0;

        foreach (var obj in _itemRepository.List)
        {
            ItemViewModel itemViewModel = new ItemViewModel(obj, this);
            ItemViewModelList.Add(itemViewModel);
            SelectableItemViewModelList.Add(new SelectableItemViewModel(itemViewModel));
        }

        foreach (var obj in _layoutRepository.List)
        {
            LayoutViewModelList.Add(new LayoutViewModel(obj, this));
        }
        
        CurrentLayoutViewModel = LayoutViewModelList.Count > 0 ? LayoutViewModelList[0] : null;

        _canInteract = true;

        SearchQuery = string.Empty;
        
        SetTitle($"Opened {LoadedFilePath.Substring(LoadedFilePath.LastIndexOf('\\')+1)}");

        if (!loadResult.Success)
        {
            MessageBox.Show(loadResult.Message, "Load Errors");
        }
    }
    
    private RelayCommand _saveAsCommand;
    public ICommand SaveAsCommand
    {
        get
        {
            if (_saveAsCommand == null)
                _saveAsCommand = new RelayCommand(param => SaveAsBoard());
            return _saveAsCommand;
        }
    }
    public void SaveAsBoard()
    {
        _openFolderDialog.Title = "Open";
        _openFolderDialog.Multiselect = false;

        bool? result = _openFolderDialog.ShowDialog();
        if (result == null || !result.Value)
            return;

        _canInteract = false;

        string folderPath = _openFolderDialog.FolderName;

        _tagRepository.Save($"{folderPath}\\TAGS.json");
        _itemRepository.Save($"{folderPath}\\ITEMS.json");
        _layoutRepository.Save($"{folderPath}\\LAYOUTS.json");
        
        LoadedFilePath = folderPath;
        
        SetTitle($"Saved {LoadedFilePath.Substring(LoadedFilePath.LastIndexOf('\\')+1)}");

        _canInteract = true;
    }
    
    private RelayCommand _saveCommand;
    public ICommand SaveCommand
    {
        get
        {
            if (_saveCommand == null)
                _saveCommand = new RelayCommand(param => SaveBoard());
            return _saveCommand;
        }
    }
    public void SaveBoard()
    {
        _canInteract = false;

        if (string.IsNullOrWhiteSpace(LoadedFilePath))
        {
            SaveAsBoard();
            return;
        }
        
        _tagRepository.Save($"{LoadedFilePath}\\TAGS.json");
        _itemRepository.Save($"{LoadedFilePath}\\ITEMS.json");
        _layoutRepository.Save($"{LoadedFilePath}\\LAYOUTS.json");
        
        SetTitle($"Saved {LoadedFilePath.Substring(LoadedFilePath.LastIndexOf('\\')+1)}");

        _canInteract = true;
    }

    private RelayCommand _openTagEditorCommand;
    public ICommand OpenTagEditorCommand
    {
        get
        {
            if (_openTagEditorCommand == null)
                _openTagEditorCommand = new RelayCommand(param => OpenTagEditor());
            return _openTagEditorCommand;
        }
    }
    public void OpenTagEditor()
    {
        if (TagEditorWindow != null)
        {
            TagEditorWindow.Focus();
            return;
        }
        
        TagEditorViewModel tagEditorViewModel = new TagEditorViewModel(this);
        TagEditorWindow = new TagEditorWindow();
        TagEditorWindow.DataContext = tagEditorViewModel;
        TagEditorWindow.Show();
    }
    
    private RelayCommand _convertAbsoluteFilePathsCommand;
    public ICommand ConvertAbsoluteFilePathsCommand
    {
        get
        {
            if (_convertAbsoluteFilePathsCommand == null)
                _convertAbsoluteFilePathsCommand = new RelayCommand(param => ConvertAbsoluteFilePaths());
            return _convertAbsoluteFilePathsCommand;
        }
    }
    public void ConvertAbsoluteFilePaths()
    {
        if (string.IsNullOrWhiteSpace(LoadedFilePath))
            return;
        foreach (ItemViewModel itemViewModel in ItemViewModelList)
        {
            string uriString = itemViewModel.ImageViewModel.ImageUriString;
            if (string.IsNullOrWhiteSpace(uriString))
                continue;
            if (!itemViewModel.ImageViewModel.Image.BaseUri.IsFile)
                continue;
            if (uriString.StartsWith(LoadedFilePath))
                uriString = uriString.Substring(LoadedFilePath.Length + 1);
            itemViewModel.ImageViewModel.ImageUriString = uriString;
        }
    }

    private RelayCommand _searchFilterCommand;
    public ICommand SearchFilterCommand
    {
        get
        {
            if (_searchFilterCommand == null)
                _searchFilterCommand = new RelayCommand(param => SearchFilter(param as FilterEventArgs));
            return _searchFilterCommand;
        }
    }
    public void SearchFilter(FilterEventArgs e)
    {
        SelectableItemViewModel selectableItemviewModel = (SelectableItemViewModel)e.Item;
        if (selectableItemviewModel == null)
            return;

        if (!selectableItemviewModel.Name.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase))
        {
            e.Accepted = false;
            return;
        }

        int requiredFilterTags = 0;
        foreach (TagViewModel tagViewModel in selectableItemviewModel.TagViewModelList)
        {
            if (!TryGetFilterTagViewModel(tagViewModel, out FilterTagViewModel filterTagViewModel))
                continue; // filter tag doesnt exist (shouldn't happen)

            switch (filterTagViewModel.Filter)
            {
                case FilterTypes.Ignored:
                    continue;
                case FilterTypes.Unwanted:
                    e.Accepted = false;
                    return;
                case FilterTypes.Required:
                    ++requiredFilterTags;
                    continue;
            }
        }

        e.Accepted = requiredFilterTags == FilterTagViewModel.TotalRequiredFilters;
    }

    private RelayCommand _layoutSelectionChangedCommand;
    public ICommand LayoutSelectionChangedCommand
    {
        get
        {
            if (_layoutSelectionChangedCommand == null)
                _layoutSelectionChangedCommand = new RelayCommand(param => LayoutSelectionChanged(param as SelectionChangedEventArgs));
            return _layoutSelectionChangedCommand;
        }
    }
    public void LayoutSelectionChanged(SelectionChangedEventArgs obj)
    {
        if (!_canInteract)
            return;
        if (obj.AddedItems[0] != CollectionView.NewItemPlaceholder)
            return;
        CreateNewLayout();
    }

    private RelayCommand _addItemCommand;
    public ICommand AddItemCommand
    {
        get
        {
            if (_addItemCommand == null)
                _addItemCommand = new RelayCommand(param => AddItem());
            return _addItemCommand;
        }
    }
    public void AddItem()
    {
        Item newItem = new Item();
        _itemRepository.List.Add(newItem);
        ItemViewModel itemViewModel = new ItemViewModel(newItem, this);
        ItemViewModelList.Add(itemViewModel);
        SelectableItemViewModel selectableItemViewModel = new SelectableItemViewModel(itemViewModel);
        SelectableItemViewModelList.Add(selectableItemViewModel);
        if (SelectableItemViewModelList.Count > 1)
        {
            SelectableItemViewModelList[^2].UpdateCanMoves();
            SelectableItemViewModelList[^1].UpdateCanMoves();
        }
        SelectItem(selectableItemViewModel);
    }

    public void MoveItem(SelectableItemViewModel selectableItemViewModel, int offset)
    {
        int index = SelectableItemViewModelList.IndexOf(selectableItemViewModel);
        int newIndex = index + offset;
        if (newIndex >= SelectableItemViewModelList.Count || newIndex < 0)
            return;
        
        SelectableItemViewModel target = SelectableItemViewModelList[newIndex];
        SelectableItemViewModelList.Move(index, newIndex);

        target.UpdateCanMoves();
        selectableItemViewModel.UpdateCanMoves();
        
        ItemViewModelList.Move(index, newIndex);
        _itemRepository.List.Move(index, newIndex);
    }

    public void AddTag()
    {
        Tag newTag = new Tag();
        _tagRepository.List.Add(newTag);
        TagViewModel tagViewModel = new TagViewModel(newTag);
        TagViewModelList.Add(tagViewModel);
        FilterTagViewModel filterTagViewModel = new FilterTagViewModel(tagViewModel, this);
        FilterTagViewModelList.Add(filterTagViewModel);
    }

    public void RemoveTag(TagViewModel tagViewModel)
    {
        foreach (ItemViewModel itemViewModel in ItemViewModelList)
        {
            itemViewModel.TagViewModelList.Remove(tagViewModel);
        }

        if (TryGetFilterTagViewModel(tagViewModel, out FilterTagViewModel? filterTagViewModel))
            FilterTagViewModelList.Remove(filterTagViewModel);

        TagViewModelList.Remove(tagViewModel);

        _tagRepository.List.Remove(tagViewModel.Tag);
    }

    private void CreateNewLayout()
    {
        Layout newLayout = new Layout("New Layout");
        _layoutRepository.List.Add(newLayout);
        LayoutViewModel newLayoutViewModel = new LayoutViewModel(newLayout, this);
        LayoutViewModelList.Add(newLayoutViewModel);
        CurrentLayoutViewModel = newLayoutViewModel;

        if (LayoutViewModelList.Count == 2)
        {
            LayoutViewModelList[0].UpdateCanMoves();
        } else if (LayoutViewModelList.Count > 2)
        {
            LayoutViewModelList[^2].UpdateCanMoves();
        }
        CurrentLayoutViewModel.UpdateCanMoves();
    }

    public void RemoveLayout(LayoutViewModel layoutViewModel)
    {
        _canInteract = false;
        int index = LayoutViewModelList.IndexOf(layoutViewModel);
        bool isCurrentLayout = (CurrentLayoutViewModel == layoutViewModel);
        LayoutViewModelList.RemoveAt(index);
        if (isCurrentLayout)
        {
            index = Math.Min(Math.Max(index-1, 0), LayoutViewModelList.Count-1);
            CurrentLayoutViewModel = LayoutViewModelList.Count > 0 ? LayoutViewModelList[index] : null;
        }
        _canInteract = true;
        _layoutRepository.List.Remove(layoutViewModel.Layout);

        if (LayoutViewModelList.Count > 0)
        {
            LayoutViewModelList[0].UpdateCanMoves();
            LayoutViewModelList[^1].UpdateCanMoves();
        }
    }
    
    public void MoveLayout(LayoutViewModel layoutViewModel, int offset)
    {
        int index = LayoutViewModelList.IndexOf(layoutViewModel);
        int newIndex = index + offset;
        if (newIndex >= LayoutViewModelList.Count || newIndex < 0)
            return;
        
        LayoutViewModel target = LayoutViewModelList[newIndex];
        LayoutViewModelList.Move(index, newIndex);

        target.UpdateCanMoves();
        layoutViewModel.UpdateCanMoves();
        
        _layoutRepository.List.Move(index, newIndex);
    }

    public bool TryGetTagViewModel(Tag tag, out TagViewModel? tagViewModel)
    {
        foreach (TagViewModel tvm in TagViewModelList)
        {
            if (tvm.Tag != tag)
                continue;
            tagViewModel = tvm;
            return true;
        }

        tagViewModel = null;
        return false;
    }

    public bool TryGetFilterTagViewModel(TagViewModel tagViewModel, out FilterTagViewModel? filterTagViewModel)
    {
        foreach (FilterTagViewModel ftvm in FilterTagViewModelList)
        {
            if (ftvm.Guid != tagViewModel.Guid)
                continue;
            filterTagViewModel = ftvm;
            return true;
        }

        filterTagViewModel = null;
        return false;
    }

    public void SelectItem(SelectableItemViewModel? selectableItemViewModel)
    {
        if (selectableItemViewModel == null)
        {
            SelectedItemViewModel = null;
            StaticFloatingItemViewModel = null;
            return;
        }
        
        SelectedItemViewModel = selectableItemViewModel;
        FloatingItem newFloatingItem = new FloatingItem(selectableItemViewModel.Item);
        if (StaticFloatingItemViewModel != null)
        {
            newFloatingItem.Width = StaticFloatingItemViewModel.Width;
            newFloatingItem.Height = StaticFloatingItemViewModel.Height;
        }
        StaticFloatingItemViewModel = new FloatingItemViewModel(
            this,
            newFloatingItem, 
            ItemViewModelList);
    }
    
    public void RemoveItem(ItemViewModel itemViewModel)
    {
        foreach (LayoutViewModel layoutViewModel in LayoutViewModelList)
        {
            layoutViewModel.RemoveItem(itemViewModel);
        }

        for (var i = SelectableItemViewModelList.Count - 1; i >= 0; i--)
        {
            var selectableItemViewModel = SelectableItemViewModelList[i];
            if (selectableItemViewModel.Item == itemViewModel.Item)
            {
                SelectableItemViewModelList.RemoveAt(i);
            }
        }

        ItemViewModelList.Remove(itemViewModel);

        _itemRepository.List.Remove(itemViewModel.Item);

        SelectedItemViewModel = null;
        
        StaticFloatingItemViewModel.Dispose();
        StaticFloatingItemViewModel = null;
    }

    public void UpdateVisibleItems()
    {
        SelectableItemViewModelListViewSource.View.Refresh();
    }

    private void ClearViewModelLists()
    {
        foreach (var viewModel in TagViewModelList)
            viewModel.Dispose();
        TagViewModelList.Clear();
        
        foreach (var viewModel in FilterTagViewModelList)
            viewModel.Dispose();
        FilterTagViewModelList.Clear();
        
        foreach (var viewModel in ItemViewModelList)
            viewModel.Dispose();
        ItemViewModelList.Clear();
        
        foreach (var viewModel in SelectableItemViewModelList)
            viewModel.Dispose();
        SelectableItemViewModelList.Clear();
        
        foreach (var viewModel in LayoutViewModelList)
            viewModel.Dispose();
        LayoutViewModelList.Clear();
    }

    private void OnSelectedItemViewModelChanged(SelectableItemViewModel? oldSelectedItemViewModel,
        SelectableItemViewModel? newSelectedItemViewModel)
    {
        if (oldSelectedItemViewModel != null)
            oldSelectedItemViewModel.UpdateIsSelected();

        if (newSelectedItemViewModel != null)
            newSelectedItemViewModel.UpdateIsSelected();
    }
}