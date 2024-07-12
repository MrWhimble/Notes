namespace Notes.Models;

public class FloatingItem
{
    public double X { get; set; }
    
    public double Y { get; set; }
    
    public double Width { get; set; }
    
    public double Height { get; set; }

    public Item Item { get; set; }

    public FloatingItem(Item item)
    {
        X = -1d;
        Y = -1d;
        Width = 400d;
        Height = 300d;
        Item = item;
    }

    public override string ToString()
    {
        return $"({X}, {Y}) [{Width}, {Height}]\n{Item}";
    }
}