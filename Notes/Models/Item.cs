using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Notes.Models;

public class Item : IIdentifiable<Guid>
{
    [JsonIgnore]
    public Guid Id => Guid;
    public Guid Guid { get; }

    public string Name { get; set; }

    public string IconUriString { get; set; }

    public List<Tag> Tags { get; set; }

    public List<Note> Notes { get; set; }

    public Item()
    {
        Guid = Guid.NewGuid();
        Name = "";
        IconUriString = "";
        Tags = new List<Tag>();
        Notes = new List<Note>();
    }
    
    public Item(Guid guid, string name, string iconUriString, Tag[] tags, Note[] notes)
    {
        Guid = guid;
        Name = name;
        IconUriString = iconUriString;
        Tags = new List<Tag>(tags);
        Notes = new List<Note>(notes);
    }

    [JsonConstructor]
    public Item(Guid guid, string name, string uriString, Tag[] tags, Note[] notes, string? description)
    {
        Guid = guid == Guid.Empty ? Guid.NewGuid() : guid;
        Name = name;
        IconUriString = uriString;
        Tags = new List<Tag>(tags);
        Notes = new List<Note>(notes);
        
        if(description != null && !String.IsNullOrWhiteSpace(description))
            Notes.Insert(0, new Note("Description", description));
    }

    public override string ToString()
    {
        string s = $"ID: {Guid}\n\r";
        s += $"Name: {Name}\n\r";
        s += $"Tags: ";
        foreach (Tag t in Tags)
            s += $"{t.Name}, ";
        s += "\n\rNotes:\n\r";
        foreach (Note n in Notes)
        {
            s += $"{n.Heading}, ";
        }

        return s;
    }
}