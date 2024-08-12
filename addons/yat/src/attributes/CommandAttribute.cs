using System;

namespace YAT.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CommandAttribute : Attribute
{
    public CommandAttribute(string name, string description = "", string manual = "", string
        group = "", params string[] aliases)
    {
        Name = name;
        Description = description;
        Manual = manual;
        Aliases = aliases;
        Group = group;
    }

    public string Name { get; private set; }

    /// <summary>
    ///     Note: Supports BBCode.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    ///     Note: Supports BBCode.
    /// </summary>
    public string Manual { get; private set; }

    public string[] Aliases { get; private set; }
    public string Group { get; private set; }
}
