using System.Collections.Generic;
using Godot;
using YAT.Classes.Managers;
using YAT.Enums;
using YAT.Helpers;
using YAT.Scenes;

namespace YAT;

public partial class Yat : Node
{
    [Signal]
    public delegate void YatReadyEventHandler();

    // config
    public static string Title = "YAT";
    public static string ScreenPrompt = "YAT - Yet Another Terminal\nMIT 2023 - MASSHUU12";
    public static bool IsShowPath = true;
    public static bool HasContextMenu = true;
    public static bool SupportMultipleTerminal = true;
    public static List<string> BaseCommandHide = new();

    public static Yat Instance;
    public static bool CanConsoleUsed = false;

    public bool YatEnabled = true;
    public Node Windows { get; private set; }
    public BaseTerminal CurrentTerminal { get; set; }

    public DebugScreen DebugScreen { get; private set; }
    public RegisteredCommands Commands { get; private set; }
    public TerminalManager TerminalManager { get; private set; }
    public PreferencesManager PreferencesManager { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        Windows = GetNode<Node>("./Windows");
        DebugScreen = GetNode<DebugScreen>("./Windows/DebugScreen");
        Commands = GetNode<RegisteredCommands>("./RegisteredCommands");
        PreferencesManager = GetNode<PreferencesManager>("%PreferencesManager");

        TerminalManager = GetNode<TerminalManager>("./TerminalManager");
        TerminalManager.GameTerminal.Ready += () =>
        {
            TerminalManager.GameTerminal.TerminalSwitcher.CurrentTerminalChanged +=
                terminal => { CurrentTerminal = terminal; };
            EmitSignal(SignalName.YatReady);
        };

        Keybindings.LoadDefaultActions();

        CheckYatEnableSettings();
    }

    private void CheckYatEnableSettings()
    {
        if (!PreferencesManager.Preferences.UseYatEnableFile)
        {
            return;
        }

        string path = PreferencesManager.Preferences.YatEnableLocation switch
        {
            EYatEnableLocation.UserData => "user://",
            EYatEnableLocation.CurrentDirectory => "res://",
            _ => "user://"
        };

        YatEnabled = FileAccess.FileExists(path + PreferencesManager.Preferences.YatEnableFile);
    }
}
