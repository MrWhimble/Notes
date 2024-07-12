using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Notes.Views;
using Notes.ViewModels;

namespace Notes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        // From: https://stackoverflow.com/questions/50645417/jetbrains-rider-doesnt-show-c-sharp-wpf-exception
        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            AttachToParentConsole();
            Console.WriteLine("Begin Init");
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
#endif // DEBUG
            
            base.OnStartup(e);
            
            MainWindowViewModel viewModel = new MainWindowViewModel();
            MainWindow = new MainWindow();
            MainWindow.DataContext = viewModel;
            viewModel.SelectableItemViewModelListViewSource = (CollectionViewSource)MainWindow.Resources["SelectableItemList"];
            MainWindow.Show();

        }
#if DEBUG
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"{sender} - {e.Exception}");
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"{sender} - {e.ExceptionObject}");
        }

        private const int AttachParentProcess = -1;

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        public static void AttachToParentConsole()
        {
            AttachConsole(AttachParentProcess);
        }
#endif // DEBUG
    }
}