using System;

namespace YAT.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class UsageAttribute : Attribute
{
    public UsageAttribute(string usage) => Usage = usage;

    public string Usage { get; private set; }
}
