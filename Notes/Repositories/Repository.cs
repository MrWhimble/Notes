using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using Notes.Models;
using Notes.ViewModels;

namespace Notes;

public class Repository<TId, TRepo> 
    where TRepo : IIdentifiable<TId>
    where TId : IEquatable<TId>, IParsable<TId>
{
    protected readonly ObservableCollection<TRepo> _list = new();

    public ObservableCollection<TRepo> List => _list;

    public virtual MethodResult Load(string filePath)
    {
        MethodResult getJsonResult = TryGetJsonString(filePath, out string jsonString);
        if (!getJsonResult.Success)
            return getJsonResult;
        
        List<TRepo>? list = JsonConvert.DeserializeObject<List<TRepo>>(jsonString);

        MethodResult moveToListResult = MoveValuesToList(list);
        if (!moveToListResult.Success)
            return moveToListResult;
        
        return new MethodResult(true);
    }

    public virtual MethodResult Save(string filePath)
    {
        string jsonString = JsonConvert.SerializeObject(List, Formatting.Indented);
        return SaveStringToFile(filePath, jsonString);
    }

    protected MethodResult TryGetJsonString(string filePath, out string jsonString)
    {
        jsonString = String.Empty;
        
        if (!File.Exists(filePath))
            return new MethodResult(false, $"{filePath} does not exist");

        using StreamReader reader = new StreamReader(filePath);
        jsonString = reader.ReadToEnd();

        return new MethodResult(true);
    }

    protected MethodResult MoveValuesToList(List<TRepo>? list)
    {
        if (list == null)
            return new MethodResult(false, $"{GetType().BaseType.GenericTypeArguments[1].Name} data does not exist");
        
        List.Clear();
        foreach (var v in list)
            List.Add(v);

        return new MethodResult(true);
    }

    protected MethodResult SaveStringToFile(string filePath, string dataString)
    {
        using StreamWriter writer = new StreamWriter(filePath, false);
        writer.Write(dataString);
        writer.Close();
        return new MethodResult(true);
    }
}

public class Repository<TId, TRepo, TDataList> : Repository<TId, TRepo>
    where TId : IEquatable<TId>, IParsable<TId>
    where TRepo : IIdentifiable<TId>
    where TDataList : IIdentifiable<TId>
{
    public override MethodResult Load(string filePath)
    {
        throw new ArgumentException($"This Repo requires a {nameof(IEnumerable<TDataList>)} dataList");
    }

    public virtual MethodResult Load(string filePath, IEnumerable<TDataList> dataList)
    {
        MethodResult getJsonResult = TryGetJsonString(filePath, out string jsonString);
        if (!getJsonResult.Success)
        {
            List.Clear();
            return getJsonResult;
        }

        List<TRepo>? list = JsonConvert.DeserializeObject<List<TRepo>>(jsonString, new IdentifiableJsonConverter<TId, TDataList>(dataList));

        MethodResult moveToListResult = MoveValuesToList(list);
        if (!moveToListResult.Success)
            return moveToListResult;
        
        return new MethodResult(true);
    }
    
    public override MethodResult Save(string filePath)
    {
        string jsonString = JsonConvert.SerializeObject(List, Formatting.Indented, new IdentifiableJsonConverter<TId, TDataList>());
        return SaveStringToFile(filePath, jsonString);
    }
}

