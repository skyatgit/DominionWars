using System;
using System.Collections.Generic;
using Godot;
using YAT.Attributes;
using YAT.Commands;
using YAT.Enums;
using YAT.Helpers;
using YAT.Interfaces;
using QuickCommands = YAT.Resources.QuickCommands;
using Version = YAT.Commands.Version;

namespace YAT.Scenes;

public partial class RegisteredCommands : Node
{
    [Signal]
    public delegate void QuickCommandsChangedEventHandler();

    private const ushort MaxQuickCommands = 10;
    private const string QuickCommandsPath = "user://yat_qc.tres";

    private Yat _yat;

    [Export] public QuickCommands QuickCommands { get; set; } = new();

    public static Dictionary<string, Type> Registered { get; } = new();

    public override void _Ready()
    {
        _yat = GetNode<Yat>("..");
#if !TOOLS
        Yat.BaseCommandHide.AddRange(new[]
        {
            "cat", "sys", "cowsay", "crash", "cs", "dollar", "ds", "echo", "forcegc", "fov", "inspect", "tfs", "load",
            "ls", "ping", "preference", "quickcommands", "reset", "restarat", "set", "sr", "ss", "cn", "ip", "restart",
            "timescale", "traceroute", "view", "watch", "wenv", "whereami", "pause", "toggleaudio", "preferences"
        });
#endif
        RegisterBuiltinCommands();
    }

    public static ECommandAdditionStatus AddCommand(Type commandType)
    {
        if (Yat.BaseCommandHide.Contains(commandType.Name.ToLower()))
        {
            return ECommandAdditionStatus.HideCommand;
        }

        object commandInstance = Activator.CreateInstance(commandType);

        if (commandInstance is not ICommand command)
        {
            return ECommandAdditionStatus.UnknownCommand;
        }

        if (command.GetAttribute<CommandAttribute>()
            is not CommandAttribute attribute)
        {
            return ECommandAdditionStatus.MissingAttribute;
        }

        if (Registered.ContainsKey(attribute.Name))
        {
            return ECommandAdditionStatus.ExistentCommand;
        }

        Registered[attribute.Name] = commandType;
        foreach (string alias in attribute.Aliases)
        {
            if (Registered.ContainsKey(alias))
            {
                return ECommandAdditionStatus.ExistentCommand;
            }

            Registered[alias] = commandType;
        }

        return ECommandAdditionStatus.Success;
    }

    public static ECommandAdditionStatus[] AddCommand(params Type[] commands)
    {
        ECommandAdditionStatus[] results = new ECommandAdditionStatus[commands.Length];

        for (int i = 0; i < commands.Length; i++)
        {
            results[i] = AddCommand(commands[i]);
        }

        return results;
    }

    public bool AddQuickCommand(string name, string command)
    {
        if (QuickCommands.Commands.ContainsKey(name) ||
            QuickCommands.Commands.Count >= MaxQuickCommands
           )
        {
            return false;
        }

        QuickCommands.Commands.Add(name, command);

        bool status = Storage.SaveResource(QuickCommands, QuickCommandsPath);

        GetQuickCommands();

        EmitSignal(SignalName.QuickCommandsChanged);

        return status;
    }

    public bool RemoveQuickCommand(string name)
    {
        if (!QuickCommands.Commands.ContainsKey(name))
        {
            return false;
        }

        QuickCommands.Commands.Remove(name);

        bool status = Storage.SaveResource(QuickCommands, QuickCommandsPath);

        GetQuickCommands();

        EmitSignal(SignalName.QuickCommandsChanged);

        return status;
    }

    /// <summary>
    ///     Retrieves the quick commands from file and adds them to the list of quick commands.
    /// </summary>
    public bool GetQuickCommands()
    {
        QuickCommands qc = Storage.LoadResource<QuickCommands>(QuickCommandsPath);

        if (qc is not null)
        {
            QuickCommands = qc;
        }

        return qc is not null;
    }

    private void RegisterBuiltinCommands()
    {
        ECommandAdditionStatus[] results = AddCommand(typeof(Ls), typeof(Ip), typeof(Cn), typeof(Cs), typeof(Ds),
            typeof(Ss), typeof(Sr),
            typeof(Cls), typeof(Man), typeof(Set), typeof(Cat), typeof(Sys), typeof(Fov), typeof(Tfs), typeof(Lcr),
            typeof(Quit), typeof(Echo), typeof(List), typeof(View), typeof(Ping), typeof(Wenv), typeof(Load),
            typeof(Pause), typeof(Watch), typeof(Reset), typeof(Crash), typeof(Cowsay), typeof(Dollar), typeof(Restart),
            typeof(History), typeof(Inspect), typeof(Forcegc), typeof(Whereami), typeof(Timescale), typeof(TraceRoute),
            typeof(ToggleAudio), typeof(Commands.QuickCommands), typeof(Version), typeof(Commands.Preferences));

        for (int i = 0; i < results.Length; i++)
        {
            switch (results[i])
            {
                case ECommandAdditionStatus.UnknownCommand:
                    _yat.CurrentTerminal.Output.Error(
                        Messages.UnknownCommand(results[i].ToString())
                    );
                    break;
                case ECommandAdditionStatus.MissingAttribute:
                    _yat.CurrentTerminal.Output.Error(
                        Messages.MissingAttribute("CommandAttribute", results[i].ToString())
                    );
                    break;
                case ECommandAdditionStatus.ExistentCommand:
                    _yat.CurrentTerminal.Output.Error($"Command {results[i]} already exists.");
                    break;
            }
        }
    }
}
