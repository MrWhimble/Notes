using System.Windows;
using System.Windows.Controls;

namespace Notes.Controls;

public class HintedTextBox
{
    public static readonly DependencyProperty HintProperty = 
        DependencyProperty.RegisterAttached(
            "Hint", 
            typeof(string), 
            typeof(HintedTextBox), 
            new FrameworkPropertyMetadata("", 
                FrameworkPropertyMetadataOptions.AffectsRender)
        );

    public static string GetHint(TextBox target) => 
        (string)target.GetValue(HintProperty);
    public static void SetHint(TextBox target, string value) => 
        target.SetValue(HintProperty, value);
}