using System;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Notes.Models;

public class Tag : IIdentifiable<Guid>
{
    private const int MinRgbValue = 160;
    private const int MaxRgbValue = 256;
    
    private static Random _random = new ();

    [JsonIgnore]
    public Guid Id => Guid;
    public Guid Guid { get; }
    public string Name { get; set; }
    public int Importance { get; set; }
    public Color Color { get; set; }

    [JsonConstructor]
    public Tag(Guid guid, string name, int importance, Color color)
    {
        Guid = guid == Guid.Empty ? Guid.NewGuid() : guid;
        Name = name;
        Importance = importance;
        Color = color;
    }

    public Tag()
    {
        Guid = Guid.NewGuid();
        Name = "";
        Importance = 0;
        Color = Color.FromRgb((byte)_random.Next(MinRgbValue, MaxRgbValue), (byte)_random.Next(MinRgbValue, MaxRgbValue), (byte)_random.Next(MinRgbValue, MaxRgbValue));
    }
    
    public Tag(string name)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Importance = 0;
        Color = Color.FromRgb((byte)_random.Next(MinRgbValue, MaxRgbValue), (byte)_random.Next(MinRgbValue, MaxRgbValue), (byte)_random.Next(MinRgbValue, MaxRgbValue));
    }

    public override string ToString()
    {
        return $"{Guid} | Name: {Name} -> {Importance}";
    }
}