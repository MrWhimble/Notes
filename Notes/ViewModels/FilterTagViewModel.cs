using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Notes.ViewModels;

public class FilterTagViewModel : ViewModelBase
{
    private MainWindowViewModel _mainWindowViewModel;
    private TagViewModel _tagViewModel;

    public static int TotalRequiredFilters = 0;

    public Guid Guid => _tagViewModel.Guid;
    public string Name => _tagViewModel.Name;
    public int Importance => _tagViewModel.Importance;
    public Color Color => _tagViewModel.Color;
    
    private FilterTypes _filter;
    public FilterTypes Filter
    {
        get => _filter;
        set
        {
            if (_filter == value)
                return;
            
            if (_filter is not FilterTypes.Required && value is FilterTypes.Required)
                ++TotalRequiredFilters;
            else if (_filter is FilterTypes.Required && value is not FilterTypes.Required)
                --TotalRequiredFilters;
            
            _filter = value;
            base.OnPropertyChanged();
        }
    }
    
    

    public FilterTagViewModel(TagViewModel tagViewModel, MainWindowViewModel mainWindowViewModel)
    {
        _filter = FilterTypes.Ignored;

        _mainWindowViewModel = mainWindowViewModel;
        _tagViewModel = tagViewModel;
        _tagViewModel.PropertyChanged += TagViewModelOnPropertyChanged;
    }

    public override void Dispose()
    {
        _tagViewModel.PropertyChanged -= TagViewModelOnPropertyChanged;
        _tagViewModel = null;
        _mainWindowViewModel = null;
    }

    private void TagViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    private RelayCommand _changeFilterCommand;

    public ICommand ChangeFilterCommand
    {
        get
        {
            if (_changeFilterCommand == null)
                _changeFilterCommand = new RelayCommand(param => ChangeFilter());
            return _changeFilterCommand;
        }
    }

    public void ChangeFilter()
    {
        Filter = (FilterTypes)(((int)Filter + 1) % 3);
        _mainWindowViewModel.UpdateVisibleItems();
    }
}