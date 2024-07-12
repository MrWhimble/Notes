using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Notes.Models;

public class Layout : IIdentifiable<Guid>
{
    [JsonIgnore]
    public Guid Id => Guid;
    public Guid Guid { get; }

    public string Name { get; set; }
    
    public double ViewX { get; set; }
    public double ViewY { get; set; }
    public double CanvasWidth { get; set; }
    public double CanvasHeight { get; set; }
    
    public List<FloatingItem> FloatingItems { get; set; }

    public Layout(string name)
    {
        Guid = Guid.NewGuid();
        Name = name;
        ViewX = 0;
        ViewY = 0;
        CanvasWidth = 4000;
        CanvasHeight = 4000;
        FloatingItems = new List<FloatingItem>();
    }

    [JsonConstructor]
    public Layout(Guid guid, string name, double viewX, double viewY, double canvasWidth, double canvasHeight, FloatingItem[] floatingItems)
    {
        Guid = guid == Guid.Empty ? Guid.NewGuid() : guid;
        Name = name;
        ViewX = viewX;
        ViewY = viewY;
        CanvasWidth = canvasWidth == 0 ? 2000 : canvasWidth;
        CanvasHeight = canvasHeight == 0 ? 1200 : canvasHeight;
        FloatingItems = new List<FloatingItem>(floatingItems);
    }

    public override string ToString()
    {
        string s = $"{Name}\nFloatingItems:\n";
        foreach (FloatingItem floatingItem in FloatingItems)
            s += $"{floatingItem}\n";
        return s;
    }
}