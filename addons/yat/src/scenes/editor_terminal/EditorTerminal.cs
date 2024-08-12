using Godot;

namespace YAT.Scenes;

[Tool]
public partial class EditorTerminal : Control
{
    public BaseTerminal BaseTerminal { get; private set; }

    public override void _Ready() => BaseTerminal = GetNode<BaseTerminal>("Content/BaseTerminal");
}
