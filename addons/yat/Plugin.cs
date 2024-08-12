#if TOOLS
using Godot;

namespace YAT;

[Tool]
public partial class Plugin : EditorPlugin
{
    private const string PluginName = "YAT";
    private string _version = string.Empty;

    public override void _EnterTree()
    {
        _version = GetPluginVersion();

        AddAutoloadSingleton(PluginName, GetPluginPath() + "/src/YAT.tscn");
        GD.Print($"{PluginName} {_version} loaded!");
        GD.PrintRich(
            "Up to date information about YAT can be found at [url=https://github.com/MASSHUU12/godot-yat/tree/main]https://github.com/MASSHUU12/godot-yat/tree/main[/url].");
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton(PluginName);

        GD.Print($"{PluginName} {_version} unloaded!");
    }

    private string GetPluginPath() => GetScript().As<Script>().ResourcePath.GetBaseDir();
}
#endif
