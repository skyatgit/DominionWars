using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using YAT.Attributes;
using YAT.Helpers;

namespace YAT.Scenes;

public partial class Autocompletion : Node
{
    private string _cachedInput = string.Empty;
    private LinkedListNode<string> _currentSuggestion;
    private Input _input;
    private LinkedList<string> _suggestions = new();

    private Yat _yat;
    [Export] public CommandInfo CommandInfo { get; set; }

    public override void _Ready()
    {
        _yat = GetNode<Yat>("/root/YAT");

        _input = CommandInfo.Input;
    }

    public override void _Input(InputEvent @event)
    {
        if (!_input.HasFocus())
        {
            return;
        }

        if (@event.IsActionPressed(Keybindings.TerminalAutocompletionPrevious))
        {
            Autocomplete(false);
            _input.CallDeferred("grab_focus"); // Prevent toggling the input focus
        }
        else if (@event.IsActionPressed(Keybindings.TerminalAutocompletionNext))
        {
            Autocomplete();
            _input.CallDeferred("grab_focus"); // Prevent toggling the input focus
        }
    }

    private void Autocomplete(bool next = true)
    {
        if (_suggestions.Count > 0 && (_input.Text == _cachedInput || _suggestions.Contains(_input.Text)))
        {
            if (next)
            {
                UseNextSuggestion();
            }
            else
            {
                UsePreviousSuggestion();
            }

            return;
        }

        _cachedInput = _input.Text;
        _suggestions = new LinkedList<string>();
        _currentSuggestion = null;

        string[] tokens = Text.SanitizeText(_input.Text);

        if (tokens.Length == 1)
        {
            _suggestions = GenerateCommandSuggestions(tokens[0]);

            if (_suggestions.Count > 0)
            {
                UseNextSuggestion();
            }
        }
    }

    private void UseNextSuggestion()
    {
        if (_suggestions.Count == 0)
        {
            return;
        }

        _currentSuggestion = _currentSuggestion?.Next ?? _suggestions.First;
        _input.Text = _currentSuggestion?.Value ?? string.Empty;

        _input.MoveCaretToEnd();

        CommandInfo.UpdateCommandInfo(_input.Text);
    }

    private void UsePreviousSuggestion()
    {
        if (_suggestions.Count == 0)
        {
            return;
        }

        _currentSuggestion = _currentSuggestion?.Previous ?? _suggestions.Last;
        _input.Text = _currentSuggestion?.Value ?? string.Empty;

        _input.MoveCaretToEnd();

        CommandInfo.UpdateCommandInfo(_input.Text);
    }

    private static LinkedList<string> GenerateCommandSuggestions(string token)
    {
        List<string> listSuggestions = RegisteredCommands.Registered
            ?.Where(x => x.Value.GetCustomAttribute<CommandAttribute>()?.Name?.StartsWith(token) == true)
            ?.Select(x => x.Value.GetCustomAttribute<CommandAttribute>()?.Name ?? string.Empty)
            ?.Where(name => !string.IsNullOrEmpty(name))
            ?.Distinct()
            ?.ToList();

        return listSuggestions is null ? new LinkedList<string>() : new LinkedList<string>(listSuggestions);
    }
}
