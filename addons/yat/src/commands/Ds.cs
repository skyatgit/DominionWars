using System;
using System.Linq;
using System.Text;
using YAT.Attributes;
using YAT.Helpers;
using YAT.Interfaces;
using YAT.Scenes;
using YAT.Types;

namespace YAT.Commands;

[Command("ds", "Displays items in the debug screen.", "[b]Usage[/b]: ds")]
[Option("-h", "bool", "Displays this help message.")]
[Option("-i", "string...", "Items to display.", new string[] { })]
public sealed class Ds : ICommand
{
    public CommandResult Execute(CommandData data)
    {
        bool h = (bool)data.Options["-h"];
        string[] i = ((object[])data.Options["-i"]).Cast<string>().ToArray();

        if (h)
        {
            Help(data.Terminal);
            return ICommand.Success();
        }

        if (i.Contains("all"))
        {
            data.Yat.DebugScreen.RunAll();
        }
        else
        {
            data.Yat.DebugScreen.RunSelected(i);
        }

        return ICommand.Success();
    }

    private static void Help(BaseTerminal terminal)
    {
        StringBuilder message = new StringBuilder();

        message.AppendLine("Registered items:");

        foreach (Tuple<string, Type> item in DebugScreen.RegisteredItems.Values.SelectMany(x => x))
        {
            message.AppendLine(
                string.Format(
                    "- [b]{0}[/b] ({1}): {2}",
                    item.Item2.GetAttribute<TitleAttribute>()?.Title ?? item.Item2.Name,
                    item.Item2.Name,
                    item.Item1
                )
            );
        }

        terminal.Print(message);
    }
}
