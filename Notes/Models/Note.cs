namespace Notes.Models;

public class Note
{
    public string Heading { get; set; }
    public string Content { get; set; }
    public bool Collapse { get; set; }
    public string ImageUriString { get; set; }
    public Side ImageDisplaySide { get; set; }
    public double ImageSize { get; set; }

    public Note()
    {
        Heading = "";
        Content = "";
        Collapse = false;
        ImageUriString = string.Empty;
        ImageDisplaySide = Side.Right;
        ImageSize = 50;
    }
    
    public Note(string heading)
    {
        Heading = heading;
        Content = "";
        Collapse = false;
        ImageUriString = string.Empty;
        ImageDisplaySide = Side.Right;
        ImageSize = 50;
    }

    public Note(string heading, string content)
    {
        Heading = heading;
        Content = content;
        Collapse = false;
        ImageUriString = string.Empty;
        ImageDisplaySide = Side.Right;
        ImageSize = 50;
    }

    public Note(string heading, string content, bool collapse)
    {
        Heading = heading;
        Content = content;
        Collapse = collapse;
        ImageUriString = string.Empty;
        ImageDisplaySide = Side.Right;
        ImageSize = 50;
    }

    public Note(string heading, string content, bool collapse, string imageUriString, Side imageDisplaySide, double imageSize)
    {
        Heading = heading;
        Content = content;
        Collapse = collapse;
        ImageUriString = imageUriString;
        ImageDisplaySide = imageDisplaySide;
        ImageSize = imageSize;
    }
}