using Godot;
using YAT.Attributes;
using YAT.Interfaces;
using YAT.Types;

namespace YAT.Commands;

[Command("version", "Displays the current game version.")]
[Usage("version")]
public sealed class Version : ICommand
{
    private static readonly string s_gameName, s_gameVersion, s_version;

    static Version()
    {
        s_gameName = ProjectSettings.GetSetting("application/config/name").ToString();
        s_gameVersion = ProjectSettings.GetSetting("application/config/version").ToString();

        if (string.IsNullOrEmpty(s_gameName))
        {
            s_gameName = "Your Awesome Game";
        }

        if (string.IsNullOrEmpty(s_gameVersion))
        {
            s_gameVersion = "v0.0.0";
        }

        s_version = $"{s_gameName} {s_gameVersion}";
    }

    public CommandResult Execute(CommandData data) => ICommand.Ok(s_version);
}
