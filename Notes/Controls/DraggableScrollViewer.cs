using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notes.Controls;

public class DraggableScrollViewer : ScrollViewer
{
    public static readonly DependencyProperty TwoWayHorizontalOffsetProperty =
        DependencyProperty.Register(
            nameof(TwoWayHorizontalOffset),
            typeof(double),
            typeof(DraggableScrollViewer),
            new FrameworkPropertyMetadata(0d,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTwoWayHorizontalOffsetPropertyChanged));

    public double TwoWayHorizontalOffset
    {
        get => (double)GetValue(TwoWayHorizontalOffsetProperty);
        set => SetValue(TwoWayHorizontalOffsetProperty, value);
    }
    
    public static readonly DependencyProperty TwoWayVerticalOffsetProperty =
        DependencyProperty.Register(
            nameof(TwoWayVerticalOffset),
            typeof(double),
            typeof(DraggableScrollViewer),
            new FrameworkPropertyMetadata(0d,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTwoWayVerticalOffsetPropertyChanged));

    public double TwoWayVerticalOffset
    {
        get => (double)GetValue(TwoWayVerticalOffsetProperty);
        set => SetValue(TwoWayVerticalOffsetProperty, value);
    }
    
    private Point _mouseDragStart;
    private double _horizontalOffsetStart;
    private double _verticalOffsetStart;
    
    protected override void OnScrollChanged(ScrollChangedEventArgs e)
    {
        base.OnScrollChanged(e);

        if (e.HorizontalChange != 0)
            TwoWayHorizontalOffset = e.HorizontalOffset;

        if (e.VerticalChange != 0)
            TwoWayVerticalOffset = e.VerticalOffset;
    }

    private static void OnTwoWayHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DraggableScrollViewer dsv)
            return;
        
        dsv.ScrollToHorizontalOffset((double)e.NewValue);
    }
    
    private static void OnTwoWayVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DraggableScrollViewer dsv)
            return;
        
        dsv.ScrollToVerticalOffset((double)e.NewValue);
    }
    
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        _mouseDragStart = e.GetPosition(this);
        _horizontalOffsetStart = HorizontalOffset;
        _verticalOffsetStart = VerticalOffset;
        
        CaptureMouse();
        
        Cursor = Cursors.Hand;
        
        base.OnMouseLeftButtonDown(e);
    }
    
    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!IsMouseCaptured)
        {
            base.OnMouseMove(e);
            return;
        }
        
        e.Handled = true;
        
        Point newMousePosition = e.GetPosition(this);
        ScrollToHorizontalOffset(_horizontalOffsetStart + (_mouseDragStart.X - newMousePosition.X));
        ScrollToVerticalOffset(_verticalOffsetStart + (_mouseDragStart.Y - newMousePosition.Y));
        
        base.OnMouseMove(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (!IsMouseCaptured)
        {
            base.OnMouseLeftButtonUp(e);
            return;
        }
        
        ReleaseMouseCapture();
        Cursor = Cursors.Arrow;
        
        base.OnMouseLeftButtonUp(e);
    }

    
}