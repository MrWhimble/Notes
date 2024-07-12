using System.Windows.Input;
using System.Windows.Media;

namespace Notes.ViewModels;

public class ToggleTagViewModel : ViewModelBase
{
    private ItemViewModel _itemViewModel;

    private TagViewModel _tagViewModel;

    public string Name => _tagViewModel.Name;
    public int Importance => _tagViewModel.Importance;
    public Color Color => _tagViewModel.Color;
    
    public bool State => _itemViewModel.TagViewModelList.Contains(_tagViewModel);

    public ToggleTagViewModel(TagViewModel tagViewModel, ItemViewModel itemViewModel)
    {
        _itemViewModel = itemViewModel;
        _tagViewModel = tagViewModel;
    }

    public override void Dispose()
    {
        _tagViewModel = null;
        _itemViewModel = null;
    }

    private RelayCommand _toggleCommand;

    public ICommand ToggleCommand
    {
        get
        {
            if (_toggleCommand == null)
                _toggleCommand = new RelayCommand(param => ToggleState());
            return _toggleCommand;
        }
    }

    public void ToggleState()
    {
        if (State)
        {
            _itemViewModel.TagViewModelList.Remove(_tagViewModel);
        }
        else
        {
            _itemViewModel.TagViewModelList.Add(_tagViewModel);
        }
        
        OnPropertyChanged(nameof(State));
    }
}