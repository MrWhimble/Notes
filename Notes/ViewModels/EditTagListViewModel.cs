using System.Collections.ObjectModel;

namespace Notes.ViewModels;

public class EditTagListViewModel : ViewModelBase
{
    private MainWindowViewModel _mainWindowViewModel;
    private ItemViewModel _itemViewModel;

    public string Name => _itemViewModel.Name;

    public ObservableCollection<ToggleTagViewModel> ToggleTagViewModelList { get; } = new ();

    public EditTagListViewModel(MainWindowViewModel mainWindowViewModel, ItemViewModel itemViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _itemViewModel = itemViewModel;

        foreach (TagViewModel tagViewModel in _mainWindowViewModel.TagViewModelList)
        {
            ToggleTagViewModel toggleTagViewModel = new ToggleTagViewModel(tagViewModel, _itemViewModel);
            ToggleTagViewModelList.Add(toggleTagViewModel);
        }
    }
    
    public override void Dispose()
    {
        _mainWindowViewModel = null;
        _itemViewModel = null;
    }
}