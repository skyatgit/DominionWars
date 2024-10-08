using System.Linq;
using System.Text;
using YAT.Attributes;
using YAT.Helpers;
using YAT.Types;

namespace YAT.Interfaces;

public interface IExtension
{
    public CommandResult Execute(CommandData args);

    public string GenerateExtensionManual()
    {
        StringBuilder sb = new();
        ExtensionAttribute attribute = this.GetAttribute<ExtensionAttribute>();

        if (string.IsNullOrEmpty(attribute?.Manual))
        {
            return $"Extension {attribute?.Name} does not have a manual.";
        }

        sb.AppendLine($"[font_size=18]{attribute.Name}[/font_size]");
        sb.AppendLine(attribute.Description);
        sb.AppendLine('\n' + attribute.Manual);
        sb.AppendLine("\n[b]Aliases[/b]:");
        sb.AppendLine(attribute.Aliases.Length > 0
            ? string.Join("\n", attribute.Aliases.Select(alias => $"[ul]\t{alias}[/ul]"))
            : "[ul]\tNone[/ul]");

        return sb.ToString();
    }
}
