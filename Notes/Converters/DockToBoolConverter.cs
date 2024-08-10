using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Notes.Converters;

public class DockToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not Dock dockParameter)
            return false;
        if (value is not Dock dockValue)
            return false;
        return dockValue == dockParameter;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not Dock dockParameter)
            return Dock.Left;
        if (value is not bool boolValue)
            return Dock.Left;

        return boolValue ? dockParameter : Dock.Left;
    }
}