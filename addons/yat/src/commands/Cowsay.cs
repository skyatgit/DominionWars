using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAT.Attributes;
using YAT.Interfaces;
using YAT.Scenes;
using YAT.Types;

namespace YAT.Commands;

[Command("cowsay", "Make a cow say something.", "[b]Usage[/b]: cowsay [i]message[/i]")]
[Argument("message", "string", "The message to make the cow say.")]
[Option("-b", "bool", "Borg")]
[Option("-d", "bool", "Dead")]
[Option("-g", "bool", "Greedy")]
[Option("-p", "bool", "Paranoid")]
[Option("-s", "bool", "Stoned")]
[Option("-t", "bool", "Tired")]
[Option("-w", "bool", "Wired")]
[Option("-y", "bool", "Youthful")]
public sealed class Cowsay : ICommand
{
    private BaseTerminal _terminal;

    public CommandResult Execute(CommandData data)
    {
        char eye = 'o';
        char tongue = ' ';

        _terminal = data.Terminal;

        Dictionary<string, char> eyes = new Dictionary<string, char>
        {
            { "-b", '=' }, // Borg
            { "-d", 'x' }, // Dead
            { "-g", '$' }, // Greedy
            { "-p", '@' }, // Paranoid
            { "-s", '*' }, // Stoned
            { "-t", '-' }, // Tired
            { "-w", 'O' }, // Wired
            { "-y", '.' } // Youthful
        };

        Dictionary<string, char> tongues = new Dictionary<string, char> { { "-d", 'U' }, { "-s", 'U' } };

        foreach ((string key, char value) in eyes)
        {
            if (!(bool)data.Options[key])
            {
                continue;
            }

            eye = value;
            if (tongues.ContainsKey(key))
            {
                tongue = tongues[key];
            }

            break;
        }

        PrintCow(data.RawData[1], eye, tongue);

        return ICommand.Success();
    }

    private static string GenerateSpeechBubble(string text)
    {
        string[] lines = text.Split('\n');
        int maxLineLength = lines.Max(line => line.Length);
        int bubbleWidth = maxLineLength + 4;

        string topLine = "  " + new string('_', bubbleWidth + 2);
        string bottomLine = "  " + new string('-', (int)(bubbleWidth * 1.5));

        StringBuilder middleLines = new();
        foreach (string line in lines)
        {
            int padding = maxLineLength - line.Length;
            string paddedLine = "< " + line + new string(' ', padding) + " >\n";
            middleLines.Append(paddedLine);
        }

        return topLine + '\n' + middleLines + bottomLine;
    }

    private void PrintCow(string message, char eye, char tongue)
    {
        string eyes = $"{eye}{eye}";
        string bubble = GenerateSpeechBubble(message);
        string padding = string.Empty.PadRight(bubble.Length >> 2);
        string[] cow =
        {
            $"{padding} \\   ^__^", $"{padding}  \\  ({eyes})\\_______", $"{padding}     (__)\\       )\\/\\",
            $"{padding}       {tongue} ||----w |", $"{padding}         ||     ||"
        };

        StringBuilder sb = new();
        sb.AppendLine(string.Join('\n', bubble));
        sb.AppendLine(string.Join('\n', cow));
        _terminal.Print(sb.ToString());
    }
}
