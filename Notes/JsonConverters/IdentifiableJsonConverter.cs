using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Notes.Models;

namespace Notes;

public class IdentifiableJsonConverter<TId, T> : JsonConverter<T> 
    where T : IIdentifiable<TId>
    where TId : IEquatable<TId>, IParsable<TId>
{

    private List<T>? _list;

    public IdentifiableJsonConverter() { }
    
    public IdentifiableJsonConverter(IEnumerable<T> list)
    {
        _list = new List<T>(list);
    }
    
    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        writer.WriteValue(value == null ? default : value.Id);
    }

    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return default;

        string valueString = reader.Value.ToString();
        
        if (int.TryParse(valueString, out int index))
        {
            if (index == -1)
                return default;
            return _list[index];
        }
        
        if (!TId.TryParse(valueString, CultureInfo.InvariantCulture, out TId id))
            return default;
        
        return TryGetValue(_list, id, out T? value) ? value : default;
    }

    public bool TryGetValue(IEnumerable<T> list, TId id, out T? value)
    {
        foreach (T v in list)
        {
            if (!id.Equals(v.Id))
                continue;
            value = v;
            return true;
        }

        value = default;
        return false;
    }
}