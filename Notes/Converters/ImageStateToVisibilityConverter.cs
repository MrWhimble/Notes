using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Notes.ViewModels;

namespace Notes.Converters;

public class ImageStateToVisibilityConverter : IValueConverter
{
    public Visibility BlankValue { get; set; }
    public Visibility UriErrorValue { get; set; }
    public Visibility LoadingValue { get; set; }
    public Visibility LoadedValue { get; set; }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ImageState imageState = (ImageState)value;
        switch (imageState)
        {
            case ImageState.Blank: return BlankValue;
            case ImageState.UriError: return UriErrorValue;
            case ImageState.Loading: return LoadingValue;
            case ImageState.Loaded: return LoadedValue;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}