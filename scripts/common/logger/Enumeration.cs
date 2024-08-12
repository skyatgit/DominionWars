using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DominionWars.Common;

public abstract class Enumeration : IComparable
{
    private readonly int _id;
    private readonly string _name;

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public string Name { get; }

    public int Id { get; }

    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        bool typeMatches = GetType() == obj.GetType();
        bool valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => _id.GetHashCode();
}
