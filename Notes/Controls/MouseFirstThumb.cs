using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Notes.Controls;

public class MouseFirstThumb : Thumb
{
    public static readonly DependencyProperty PreDragCommandProperty = 
           DependencyProperty.Register(
               "PreDragCommand", 
               typeof(ICommand), 
               typeof(MouseFirstThumb), 
               new FrameworkPropertyMetadata(
                   null));
    public ICommand PreDragCommand
    {
        get => (ICommand)GetValue(PreDragCommandProperty);
        set => SetValue(PreDragCommandProperty, value);
    }
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        Point originThumbPoint = e.GetPosition(this);
        DragStartedEventArgs args = new DragStartedEventArgs(originThumbPoint.X, originThumbPoint.Y);

        if (PreDragCommand != null && PreDragCommand.CanExecute(args))
        {
            PreDragCommand.Execute(args);
        }

        base.OnMouseLeftButtonDown(e);
    }
}