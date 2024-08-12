using Godot;
using Godot.Collections;
using YAT.Attributes;
using YAT.Helpers;
using YAT.Interfaces;

namespace YAT.Scenes;

[Title("LookingAt")]
public partial class LookingAtInfo : PanelContainer, IDebugScreenItem
{
    private const uint RayLength = 1024;

    private const string Prefix = "Looking at: ";
    private const string Nothing = Prefix + "Nothing";
    private const string NoCamera = Prefix + "No camera";
    private Label _label;
    private Yat _yat;

    public void Update()
    {
        Dictionary result = World.RayCast(_yat.GetViewport(), RayLength);

        if (result is null)
        {
            _label.Text = NoCamera;
            return;
        }

        if (result.Count == 0)
        {
            _label.Text = Nothing;
            return;
        }

        Node node = result["collider"].As<Node>();
        Vector2 position = result["position"].As<Vector2>();

        if (node is null)
        {
            _label.Text = Nothing;
            return;
        }

        _label.Text = Prefix + node.Name + " at " + position;
    }

    public override void _Ready()
    {
        _yat = GetNode<Yat>("/root/YAT");
        _label = GetNode<Label>("Label");
    }
}
