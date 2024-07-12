using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Notes.ViewModels;

namespace Notes.Converters;

public class FilterToColorConverter : IValueConverter
{
    public Color IgnoredColor { get; set; }
    public Color RequiredColor { get; set; }
    public Color UnwantedColor { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not FilterTypes filterType) 
            return Colors.Black;
        
        switch (filterType)
        {
            case FilterTypes.Ignored: return IgnoredColor;
            case FilterTypes.Required: return RequiredColor;
            case FilterTypes.Unwanted: return UnwantedColor;
        }
        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}