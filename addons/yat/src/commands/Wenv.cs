using Godot;
using YAT.Attributes;
using YAT.Interfaces;
using YAT.Types;

namespace YAT.Commands;

[Command("wenv", "Manages the world environment.", "[b]Usage[/b]: wenv [i]action[/i]")]
[Argument("action", "remove|restore", "Removes or restores the world environment.")]
public sealed class Wenv : ICommand
{
    private static Environment s_world3DEnvironment;

    public CommandResult Execute(CommandData data)
    {
        string action = (string)data.Arguments["action"];
        World3D world = data.Yat.GetTree().Root.World3D;

        if (action == "remove")
        {
            return RemoveEnvironment(world);
        }

        return RestoreEnvironment(world);
    }

    private static CommandResult RestoreEnvironment(World3D world)
    {
        if (world is null)
        {
            return ICommand.Failure("No world to restore environment to.");
        }

        if (s_world3DEnvironment is null)
        {
            return ICommand.Failure("No environment to restore.");
        }

        world.Environment = s_world3DEnvironment;
        s_world3DEnvironment = null;

        return ICommand.Success("Restored environment.");
    }

    private static CommandResult RemoveEnvironment(World3D world)
    {
        Environment env = world?.Environment;

        if (world is null)
        {
            return ICommand.Failure("No world to remove environment from.");
        }

        if (env is null)
        {
            return ICommand.Failure("No environment to remove.");
        }

        s_world3DEnvironment = env;
        world.Environment = null;

        return ICommand.Success("Removed environment.");
    }
}
