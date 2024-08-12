using Godot;
using YAT.Attributes;
using YAT.Interfaces;

namespace YAT.Scenes;

[Title("Engine")]
public partial class EngineInfoItem : PanelContainer, IDebugScreenItem
{
    private readonly string _engineVersion = Engine.GetVersionInfo()["string"].AsString();

    private readonly bool _isDebug = OS.IsDebugBuild();
    private string _engineInfo;
    private Label _label;

    public void Update()
    {
    }

    public override void _Ready()
    {
        _engineInfo = $"Godot {_engineVersion} ({(_isDebug ? "Debug" : "Release")} template)";

        _label = GetNode<Label>("Label");
        _label.Text = _engineInfo;
    }
}
