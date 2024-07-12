using System;

namespace Notes.Models;

public interface IIdentifiable<out T> where T : IEquatable<T>, IParsable<T>
{
    public T Id { get; }
}