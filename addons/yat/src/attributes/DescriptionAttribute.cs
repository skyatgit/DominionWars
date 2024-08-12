using System;

namespace YAT.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string description) => Description = description;

    public string Description { get; private set; }
}
