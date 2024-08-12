using Godot;
using YAT.Attributes;
using YAT.Interfaces;
using YAT.Types;

namespace YAT.Commands;

[Command("preferences", aliases: "prefs")]
[Usage("prefs")]
[Description("Creates a window with the available preferences.")]
public sealed class Preferences : ICommand
{
    private static Scenes.Preferences s_windowInstance;

    private static readonly PackedScene s_prefsWindow = GD.Load<PackedScene>(
        "uid://ca2i4r24ny7y3"
    );

    public CommandResult Execute(CommandData data)
    {
        bool instanceValid = GodotObject.IsInstanceValid(s_windowInstance);

        if (instanceValid)
        {
            CloseWindow();
            return ICommand.Success();
        }

        s_windowInstance = s_prefsWindow.Instantiate<Scenes.Preferences>();
        data.Yat.Windows.AddChild(s_windowInstance);

        return ICommand.Success();
    }

    private static void CloseWindow()
    {
        s_windowInstance.QueueFree();
        s_windowInstance = null;
    }
}
