using System.Linq;
using System.Reflection;
using System.Text;
using YAT.Attributes;
using YAT.Enums;
using YAT.Helpers;
using YAT.Types;

namespace YAT.Interfaces;

public interface ICommand
{
    public CommandResult Execute(CommandData data);

    public static CommandResult Success(string message = "") => new(ECommandResult.Success, message);

    public static CommandResult Failure(string message = "") => new(ECommandResult.Failure, message);

    public static CommandResult InvalidArguments(string message = "") => new(ECommandResult.InvalidArguments, message);

    public static CommandResult InvalidCommand(string message = "") => new(ECommandResult.InvalidCommand, message);

    public static CommandResult InvalidPermissions(string message = "") =>
        new(ECommandResult.InvalidPermissions, message);

    public static CommandResult InvalidState(string message = "") => new(ECommandResult.InvalidState, message);

    public static CommandResult NotImplemented(string message = "") => new(ECommandResult.NotImplemented, message);

    public static CommandResult UnknownCommand(string message = "") => new(ECommandResult.UnknownCommand, message);

    public static CommandResult UnknownError(string message = "") => new(ECommandResult.UnknownError, message);

    public static CommandResult Ok(string message = "") => new(ECommandResult.Ok, message);

    public StringBuilder GenerateCommandManual()
    {
        CommandAttribute command = this.GetAttribute<CommandAttribute>()!;
        UsageAttribute usage = this.GetAttribute<UsageAttribute>()!;
        DescriptionAttribute description = this.GetAttribute<DescriptionAttribute>()!;
        bool isThreaded = this.HasAttribute<ThreadedAttribute>();

        StringBuilder sb = new();

        sb.AppendLine(
            string.Format(
                "[p align=center][font_size=22]{0}[/font_size] [font_size=14]{1}[/font_size][/p]",
                command.Name,
                isThreaded ? "[threaded]" : string.Empty
            )
        );
        sb.AppendLine($"[p align=center]{description?.Description ?? command.Description}[/p]");
        sb.AppendLine(usage is null ? command.Manual : $"\n[b]Usage[/b]: {usage?.Usage}");
        sb.AppendLine("\n[b]Aliases[/b]:");
        sb.AppendLine(command.Aliases.Length > 0
            ? string.Join("\n", command.Aliases.Select(alias => $"[ul]\t{alias}[/ul]"))
            : "[ul]\tNone[/ul]");

        return sb;
    }

    public StringBuilder GenerateArgumentsManual()
    {
        ArgumentAttribute[] arguments = this.GetAttributes<ArgumentAttribute>();

        if (arguments is null || arguments.Length == 0)
        {
            return new StringBuilder("\nThis command does not have any arguments.");
        }

        StringBuilder sb = new();

        sb.AppendLine("[p align=center][font_size=18]Arguments[/font_size][/p]");

        foreach (ArgumentAttribute arg in arguments)
        {
            string types = string.Join(" | ", arg.Types.Select(t => t.Type));

            sb.AppendLine($"[b]{arg.Name}[/b]: {types} - {arg.Description}");
        }

        return sb;
    }

    public StringBuilder GenerateOptionsManual()
    {
        OptionAttribute[] options = this.GetAttributes<OptionAttribute>();

        if (options is null || options.Length == 0)
        {
            return new StringBuilder("\nThis command does not have any options.");
        }

        StringBuilder sb = new();

        sb.AppendLine("[p align=center][font_size=18]Options[/font_size][/p]");

        foreach (OptionAttribute opt in options)
        {
            string types = string.Join(" | ", opt.Types.Select(t => t.Type));

            sb.AppendLine($"[b]{opt.Name}[/b]: {types} - {opt.Description}");
        }

        return sb;
    }

    public StringBuilder GenerateSignalsManual()
    {
        EventInfo[] signals = this.GetEvents(BindingFlags.DeclaredOnly
                                             | BindingFlags.Instance
                                             | BindingFlags.Public
        );

        if (signals.Length == 0)
        {
            return new StringBuilder("\nThis command does not have any signals.");
        }

        StringBuilder sb = new();

        sb.AppendLine("[p align=center][font_size=18]Signals[/font_size][/p]");

        foreach (EventInfo signal in signals)
        {
            sb.AppendLine(signal.Name);
        }

        return sb;
    }
}
