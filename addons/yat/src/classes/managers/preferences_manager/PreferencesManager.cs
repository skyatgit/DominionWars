using Godot;
using YAT.Resources;

namespace YAT.Classes.Managers;

public partial class PreferencesManager : Node
{
    [Signal]
    public delegate void PreferencesUpdatedEventHandler(YatPreferences preferences);

    private const string PreferencesPath = "user://yat_preferences.tres";
    private YatPreferences _defaultPreferences;

    [Export] public YatPreferences Preferences { get; set; } = new();

    public override void _Ready()
    {
        _defaultPreferences = Preferences.Duplicate() as YatPreferences;

        Load();
    }

    public void RestoreDefaults()
    {
        Preferences = (_defaultPreferences.Duplicate() as YatPreferences)!;
        EmitSignal(SignalName.PreferencesUpdated, Preferences!);
    }

    public bool Save() => ResourceSaver.Save(Preferences, PreferencesPath) == Error.Ok;

    public bool Load()
    {
        if (!ResourceLoader.Exists(PreferencesPath))
        {
            return false;
        }

        Preferences = ResourceLoader.Load<YatPreferences>(
            PreferencesPath,
            cacheMode: ResourceLoader.CacheMode.Replace
        );
        EmitSignal(SignalName.PreferencesUpdated, Preferences);

        return true;
    }
}
