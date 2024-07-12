using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Notes.Converters;

public class BoolToColorConverter : IValueConverter
{
    public Color FalseColor { get; set; }
    public Color TrueColor { get; set; }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool state) 
            return Colors.Black;

        return state ? TrueColor : FalseColor;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}