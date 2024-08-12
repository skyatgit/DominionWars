#nullable enable
using Godot;
using Godot.Collections;
using YAT.Attributes;
using YAT.Enums;
using YAT.Helpers;
using YAT.Interfaces;
using YAT.Scenes;
using YAT.Types;

namespace YAT.Commands;

[Command(
    "cn",
    "Changes the selected node to the specified node path.",
    "[b]Usage[/b]: cn [i]node_path[/i]"
)]
[Argument(
    "node_path",
    "string",
    "The node path of the new selected node.\n" +
    "Use [b]&[/b] to use the RayCast method to get the node path where the camera is looking at.\n" +
    "Use [b]&[/b][i]ray_length[/i] to specify the ray length (default 256).\n" +
    "Example: [b]&[/b] or [b]&[/b][i]256[/i]" +
    "\nUse [b]?[/b] to search the node path in the whole tree.\n" +
    "Use [b]??[/b] to search the node path in the current node children.\n"
)]
public sealed class Cn : ICommand
{
    private const char RayCastPrefix = '&';
    private const float DefaultRayLength = 256;

    private const string TreeDeepSearchPrefix = "?";
    private const string TreeShallowSearchPrefix = "??";
    private BaseTerminal _terminal;

    private Yat _yat;

    public CommandResult Execute(CommandData data)
    {
        string path = (string)data.Arguments["node_path"];
        bool result;

        _yat = data.Yat;
        _terminal = data.Terminal;

        if (path.StartsWith(TreeShallowSearchPrefix, TreeDeepSearchPrefix))
        {
            result = ChangeSelectedNode(GetNodeFromSearch(path));
        }
        else if (path.StartsWith(RayCastPrefix))
        {
            result = ChangeSelectedNode(GetRayCastedNodePath(path));
        }
        else
        {
            result = ChangeSelectedNode(path);
        }

        if (!result)
        {
            return ICommand.Failure($"Invalid node path: {path}");
        }

        return ICommand.Success();
    }

    private NodePath? GetNodeFromSearch(string path)
    {
        Node? result = path.StartsWith(TreeShallowSearchPrefix)
            ? World.SearchNode(_terminal.SelectedNode.Current, path[2..])
            : World.SearchNode(_yat.GetTree().Root, path[1..]);

        if (result is not null)
        {
            return result.GetPath();
        }

        _terminal.Print("No node found.", EPrintType.Error);
        return null;
    }

    private NodePath? GetRayCastedNodePath(string path)
    {
        Dictionary result = World.RayCast(_yat.GetViewport(), GetRayLength(path));

        if (result is null)
        {
            _terminal.Print("No collider found.", EPrintType.Error);
            return null;
        }

        Node collider = result.TryGetValue("collider", out Variant value) ? value.As<Node>() : null;

        return collider?.GetPath();
    }

    private static float GetRayLength(string path) =>
        path[1..].TryConvert(out float rayLength)
            ? rayLength
            : DefaultRayLength;

    private bool ChangeSelectedNode(NodePath? path)
    {
        if (path is null)
        {
            return false;
        }

        return _terminal.SelectedNode.ChangeSelectedNode(path);
    }
}
