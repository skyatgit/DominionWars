using System.Collections.Generic;
using YAT.Helpers;

namespace YAT.Scenes;

public partial class QuickCommandsContext : ContextSubmenu
{
    private TerminalSwitcher _terminalSwitcher;
    private Yat _yat;

    public override void _Ready()
    {
        base._Ready();

        _yat = GetNode<Yat>("/root/YAT");
        _yat.Commands.QuickCommandsChanged += GetQuickCommands;

        _terminalSwitcher = GetNode<TerminalSwitcher>("../../Content/TerminalSwitcher");

        IdPressed += OnQuickCommandsPressed;

        GetQuickCommands();
    }

    private void GetQuickCommands()
    {
        _yat.Commands.GetQuickCommands();
        Clear();

        foreach (KeyValuePair<string, string> qc in _yat.Commands.QuickCommands.Commands)
        {
            AddItem(qc.Key);
        }
    }

    private void OnQuickCommandsPressed(long id)
    {
        string key = GetItemText((int)id);

        if (!_yat.Commands.QuickCommands.Commands.TryGetValue(key, out string command))
        {
            return;
        }

        _terminalSwitcher.CurrentTerminal.CommandManager.Run(
            Text.SanitizeText(command), _terminalSwitcher.CurrentTerminal
        );
    }
}
