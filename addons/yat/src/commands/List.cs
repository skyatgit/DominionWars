using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YAT.Attributes;
using YAT.Interfaces;
using YAT.Scenes;
using YAT.Types;

namespace YAT.Commands;

[Command("list", "List all available commands", "[b]Usage[/b]: list", aliases: "help")]
[Option("-f", "bool", "Flush the cache before listing commands.")]
public sealed class List : ICommand
{
    private static string s_cache = string.Empty;

    public CommandResult Execute(CommandData data)
    {
        if ((bool)data.Options["-f"])
        {
            s_cache = string.Empty;
        }

        if (string.IsNullOrEmpty(s_cache))
        {
            GenerateList(data.Terminal);
        }

        return ICommand.Ok(s_cache);
    }

    private static void GenerateList(BaseTerminal terminal)
    {
        StringBuilder sb = new();

        sb.AppendLine("Available commands:");
        int maxHeadSize = 0;
        foreach (KeyValuePair<string, Type> command in RegisteredCommands.Registered)
        {
            maxHeadSize = command.Key.Length > maxHeadSize ? command.Key.Length : maxHeadSize;
        }

        string linkStr = "  ";
        int tabSpaceNum = 2;
        int indentation = tabSpaceNum + maxHeadSize + linkStr.Length;
        foreach (KeyValuePair<string, Type> command in RegisteredCommands.Registered)
        {
            if (command.Value.GetCustomAttribute<CommandAttribute>() is not { } attribute)
            {
                continue;
            }

            DescriptionAttribute description = command.Value.GetCustomAttribute<DescriptionAttribute>();

            // Skip aliases
            if (attribute.Aliases.Contains(command.Key))
            {
                continue;
            }

            string header =
                $"{new string(' ', tabSpaceNum)}[b]{command.Key}[/b]{new string(' ', maxHeadSize - command.Key.Length)}{linkStr}";
            sb.Append(header);
            string descriptionStr = description?.Description ?? attribute.Description;
            sb.Append(descriptionStr.Replace("\n", $"\n{new string(' ', indentation)}"));
            sb.AppendLine();
        }

        s_cache = sb.ToString();
    }
}
