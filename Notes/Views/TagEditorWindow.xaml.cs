using System;
using System.Windows;
using System.Windows.Interop;

namespace Notes.Views;

public partial class TagEditorWindow : Window
{
    public TagEditorWindow()
    {
        InitializeComponent();
    }
    
    // From https://stackoverflow.com/a/9802870
    protected override void OnSourceInitialized(EventArgs e)
    {
        var hwndSource = PresentationSource.FromVisual(this) as HwndSource;

        if (hwndSource != null)
            hwndSource.CompositionTarget!.RenderMode = RenderMode.SoftwareOnly;
            
        base.OnSourceInitialized(e);
    }
}