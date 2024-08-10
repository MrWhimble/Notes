using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Notes.ViewModels;

namespace Notes.Converters;

public class DockToCursorConverter : IValueConverter
{
    public Cursor LeftValue { get; set; }
    public Cursor TopValue { get; set; }
    public Cursor RightValue { get; set; }
    public Cursor BottomValue { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Dock dockValue = (Dock)value;
        switch (dockValue)
        {
            case Dock.Left: return LeftValue;
            case Dock.Top: return TopValue;
            case Dock.Right: return RightValue;
            case Dock.Bottom: return BottomValue;
        }

        return Dock.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}