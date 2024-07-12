using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Notes.Converters;

public class BoolToCursorConverter : IValueConverter
{
    public Cursor FalseCursor { get; set; }
    public Cursor TrueCursor { get; set; }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool state) 
            return Cursors.Arrow;

        return state ? TrueCursor : FalseCursor;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}