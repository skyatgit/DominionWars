using System;

namespace YAT.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ExtensionAttribute : Attribute
{
    public ExtensionAttribute(string name, string description = "", string manual = "", params string[] aliases)
    {
        Name = name;
        Description = description;
        Manual = manual;
        Aliases = aliases;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Manual { get; private set; }
    public string[] Aliases { get; private set; }
}
