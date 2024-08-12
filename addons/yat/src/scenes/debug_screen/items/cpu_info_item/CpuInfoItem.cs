using Godot;
using YAT.Attributes;
using YAT.Interfaces;

namespace YAT.Scenes;

[Title("CPU")]
public partial class CpuInfoItem : PanelContainer, IDebugScreenItem
{
    private readonly string _arch = Engine.GetArchitectureName();
    private readonly int _cpuCount = OS.GetProcessorCount();
    private readonly string _cpuName = OS.GetProcessorName();
    private string _cpuInfo;
    private Label _label;

    public void Update()
    {
    }

    public override void _Ready()
    {
        _cpuInfo = $"{_cpuName} {_arch} ({_cpuCount} cores)";

        _label = GetNode<Label>("Label");
        _label.Text = _cpuInfo;
    }
}
