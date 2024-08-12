using Godot;
using YAT.Helpers;

namespace YAT.Scenes;

public partial class FullWindowDisplay : Control
{
    [Signal]
    public delegate void ClosedEventHandler();

    [Signal]
    public delegate void OpenedEventHandler();

    private Label _helpLabel;

    public bool IsOpen => Visible;
    public RichTextLabel MainDisplay { get; private set; }

    public override void _Ready()
    {
        MainDisplay = GetNode<RichTextLabel>("%MainDisplay");
        _helpLabel = GetNode<Label>("%HelpLabel");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(Keybindings.TerminalCloseFullWindowDisplay))
        {
            Close();
        }
    }

    public void Open(string text)
    {
        GenerateHelp();

        MainDisplay.Clear();
        MainDisplay.CallDeferred("append_text", text);
        Visible = true;

        EmitSignal(SignalName.Opened);
    }

    public void Close()
    {
        if (!Visible)
        {
            return;
        }

        MainDisplay.Clear();
        Visible = false;

        EmitSignal(SignalName.Closed);
    }

    private void GenerateHelp()
    {
        InputEvent closeKey = InputMap.ActionGetEvents(Keybindings.TerminalCloseFullWindowDisplay)[0];

        _helpLabel.Text = $"{closeKey.AsText()} - Close";
    }
}
