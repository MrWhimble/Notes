using System;
using System.Windows;
using System.Windows.Controls;

namespace Notes.Controls;

public class AutoFocusTextBox : TextBox
{
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(IsVisible))
        {
            if ((bool)e.NewValue)
            {
                this.Focus();
                this.SelectAll();
            }
        }
        
        base.OnPropertyChanged(e);
    }
}