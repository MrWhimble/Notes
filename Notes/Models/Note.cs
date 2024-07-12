namespace Notes.Models;

public class Note
{
    public string Heading { get; set; }
    public string Content { get; set; }
    public bool Collapse { get; set; }

    public Note()
    {
        Heading = "";
        Content = "";
        Collapse = false;
    }
    
    public Note(string heading)
    {
        Heading = heading;
        Content = "";
        Collapse = false;
    }

    public Note(string heading, string content)
    {
        Heading = heading;
        Content = content;
        Collapse = false;
    }

    public Note(string heading, string content, bool collapse)
    {
        Heading = heading;
        Content = content;
        Collapse = collapse;
    }
}