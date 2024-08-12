#nullable enable
using System;

namespace YAT.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class OptionAttribute : CommandInputAttribute
{
    public OptionAttribute(string name, string type, string description = "", object? defaultValue = null)
        : base(name, type, description) =>
        DefaultValue = defaultValue;

    public object? DefaultValue { get; private set; }
}
