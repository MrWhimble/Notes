using System;
using System.Windows.Media;
using Notes.Models;

namespace Notes.ViewModels;

public class TagViewModel : ViewModelBase, IComparable<TagViewModel>
{
    public Tag Tag { get; protected set; }

    public Guid Guid => Tag.Guid;
    
    public string Name
    {
        get => Tag.Name;
        set
        {
            if (value.Equals(Tag.Name)) return;
            Tag.Name = value;
            base.OnPropertyChanged();
        }
    }
    
    public int Importance
    {
        get => Tag.Importance;
        set
        {
            if (value.Equals(Tag.Importance)) return;
            Tag.Importance = value;
            base.OnPropertyChanged();
        }
    }
    
    public Color Color
    {
        get => Tag.Color;
        set
        {
            if (value.Equals(Tag.Color)) return;
            Tag.Color = value;
            base.OnPropertyChanged();
        }
    }

    public TagViewModel(Tag tag)
    {
        Tag = tag;
    }

    public override void Dispose()
    {
        Tag = null;
    }

    public int CompareTo(TagViewModel? other)
    {
        if (other == null)
            return -1;

        if (Tag.Importance == other.Importance)
            return String.Compare(Tag.Name, other.Name, StringComparison.Ordinal);

        return Tag.Importance > other.Importance ? 1 : -1;
    }
}