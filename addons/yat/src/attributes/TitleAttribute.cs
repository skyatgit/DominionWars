using System;

namespace YAT.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class TitleAttribute : Attribute
{
    public TitleAttribute(string title) => Title = title;

    public string Title { get; private set; }
}
