using System;
using System.Collections.Generic;
using Godot;
using YAT.Classes;
using YAT.Helpers;
using YAT.Types;

namespace YAT.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CommandInputAttribute : Attribute
{
    public CommandInputAttribute(StringName name, string type, string description = "")
    {
        Name = name;
        Description = description;

        if (string.IsNullOrEmpty(type))
        {
            GD.PushError(Messages.InvalidCommandInputType(type, name));
        }

        foreach (string t in type.Split('|'))
        {
            if (Parser.TryParseCommandInputType(t, out CommandInputType commandInputType))
            {
                Types.Add(commandInputType);
            }
            else
            {
                GD.PushError(Messages.InvalidCommandInputType(t, name));
            }
        }
    }

    public StringName Name { get; private set; }
    public List<CommandInputType> Types { get; } = new();
    public string Description { get; private set; }
}
