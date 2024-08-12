using Godot;

namespace YAT.Scenes;

public partial class InputInfo : PanelContainer
{
    private MarginContainer _container;
    public RichTextLabel Text;

    public override void _Ready()
    {
        Text = GetNode<RichTextLabel>("%Text");
        _container = GetNode<MarginContainer>("./MarginContainer");
        Visible = false;
    }

    public void DisplayCommandInfo(string text)
    {
        Text.Text = text;
        Visible = true;
    }
}
