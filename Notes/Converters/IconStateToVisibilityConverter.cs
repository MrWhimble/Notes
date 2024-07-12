using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Notes.ViewModels;

namespace Notes.Converters;

public class IconStateToVisibilityConverter  : IValueConverter
{
    public Visibility BlankValue { get; set; }
    public Visibility UriErrorValue { get; set; }
    public Visibility LoadingValue { get; set; }
    public Visibility LoadedValue { get; set; }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        IconState iconState = (IconState)value;
        switch (iconState)
        {
            case IconState.Blank: return BlankValue;
            case IconState.UriError: return UriErrorValue;
            case IconState.Loading: return LoadingValue;
            case IconState.Loaded: return LoadedValue;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}