using System;
using System.Windows;
using System.Windows.Interop;

namespace Notes.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
}