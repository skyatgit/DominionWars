using System;
using System.Collections.Generic;
using Godot;
using YAT.Helpers;
using YAT.Resources;
using YAT.Types;

namespace YAT.Scenes;

public partial class GameTerminal : YatWindow
{
    public BaseTerminal CurrentTerminal { get; private set; }
    public TerminalSwitcher TerminalSwitcher { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        TerminalSwitcher = GetNode<TerminalSwitcher>("%TerminalSwitcher");

        CurrentTerminal = TerminalSwitcher.CurrentTerminal;
        CurrentTerminal.TitleChangeRequested += title => Title = title;
        CurrentTerminal.PositionResetRequested += ResetPosition;
        CurrentTerminal.SizeResetRequested += () => Size = InitialSize;

        CloseRequested += Yat.TerminalManager.CloseTerminal;

#if GODOT4_3_OR_GREATER
		ContextMenu.AddSubmenuNodeItem(
			"QuickCommands",
			GetNode<QuickCommandsContext>("ContextMenu/QuickCommandsContext")
		);
#else
        ContextMenu.AddSubmenuItem("QuickCommands", "QuickCommandsContext");
#endif
        ContextMenu.AddItem("Preferences", 1);
        ContextMenu.IdPressed += ContextMenuItemSelected;

        MoveToCenter();
    }

    private void ContextMenuItemSelected(long id)
    {
        if (id == 1)
        {
            Commands.Preferences prefs = new Commands.Preferences();
            prefs.Execute(new CommandData(
                Yat, CurrentTerminal, prefs, Array.Empty<string>(), new Dictionary<StringName, object>(),
                new Dictionary<StringName, object>(), default
            ));
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(Keybindings.TerminalToggle))
        {
            CallDeferred("emit_signal", Window.SignalName.CloseRequested);
        }
    }

    private new void UpdateOptions(YatPreferences prefs)
    {
        Size = new Vector2I(prefs.DefaultWidth, prefs.DefaultHeight);

        base.UpdateOptions(prefs);
    }
}
