using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DominionWars.Common;

public static class StringUtil
{
    public static readonly StringComparer IgnoreCaseComparer = StringComparer.InvariantCultureIgnoreCase;

    public static readonly StringComparison OsDependentComparison = Environment.OSVersion.Platform == PlatformID.Unix
        ? StringComparison.Ordinal
        : StringComparison.OrdinalIgnoreCase;

    public static readonly StringComparer OsDependentComparer = Environment.OSVersion.Platform == PlatformID.Unix
        ? StringComparer.Ordinal
        : StringComparer.OrdinalIgnoreCase;

    /// <summary>
    ///     将文本拆分成行，确保连续的单词不会被拆分。
    /// </summary>
    /// <param name="input">要拆分的文本。</param>
    /// <param name="maxCharactersPerLine">每行的最大字符数。</param>
    /// <param name="indentation">每行的缩进空格数（默认为0）。</param>
    /// <returns>已拆分的文本。</returns>
    public static string WrapText(string input, int maxCharactersPerLine, int indentation = 0)
    {
        string[] words = input.Split(' ');

        StringBuilder result = new StringBuilder();
        int currentLineLength = 0;

        foreach (string word in words)
        {
            if (currentLineLength + word.Length + 1 <= maxCharactersPerLine)
            {
                if (currentLineLength > 0)
                {
                    result.Append(" ");
                    currentLineLength++;
                }

                result.Append(word);
                currentLineLength += word.Length;
            }
            else
            {
                result.Append(Environment.NewLine);
                result.Append(GetSpecifiedStr(' ', indentation));
                result.Append(word);
                currentLineLength = word.Length;
            }
        }

        return result.ToString();
    }

    /// <summary>
    ///     生成包含指定数量指定字符的字符串。
    /// </summary>
    /// <param name="character">要生成的字符。</param>
    /// <param name="count">生成的字符数量。</param>
    /// <returns>包含指定数量指定字符的字符串。</returns>
    public static string GetSpecifiedStr(char character, int count)
    {
        StringBuilder result = new StringBuilder(count);

        for (int i = 0; i < count; i++)
        {
            result.Append(character);
        }

        return result.ToString();
    }

    /// <summary>
    ///     在给定字符串的每一行的行首添加指定数量的空格缩进。
    /// </summary>
    /// <param name="source">要处理的源字符串。</param>
    /// <param name="indentationNum">每行行首要添加的空格数量。</param>
    /// <returns>添加缩进后的字符串。</returns>
    public static string AddIndentation(string source, int indentationNum)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), @"Source string cannot be null.");
        }

        if (indentationNum < 0)
        {
            throw new ArgumentException(@"Indentation number cannot be negative.", nameof(indentationNum));
        }

        StringBuilder indentedText = new StringBuilder();
        using (StringReader reader = new StringReader(source))
        {
            while (reader.ReadLine() is { } line)
            {
                indentedText.Append(new string(' ', indentationNum));
                indentedText.AppendLine(line);
            }
        }

        return indentedText.ToString();
    }

    /// <summary>
    ///     在字符串两端加上指定数量的相同字符。对于 "<"、"["、"{" 字符，会在右侧填充相对应的右半边符号。
    /// </summary>
    /// <param name="input">要处理的字符串。</param>
    /// <param name="paddingChar">用于填充的字符。</param>
    /// <param name="paddingCount">填充字符的数量（默认值为1）。</param>
    /// <returns>已在两端加上指定数量相同字符的新字符串。</returns>
    public static string AddPadding(string input, char paddingChar, int paddingCount = 1)
    {
        char leftChar = paddingChar;
        char rightChar = paddingChar;
        switch (paddingChar)
        {
            case '<':
                rightChar = '>';
                break;
            case '[':
                rightChar = ']';
                break;
            case '{':
                rightChar = '}';
                break;
            case '>':
                rightChar = '<';
                break;
            case ']':
                rightChar = '[';
                break;
            case '}':
                rightChar = '{';
                break;
        }

        StringBuilder result = new StringBuilder();
        result.Append(leftChar, paddingCount);
        result.Append(input);
        result.Append(rightChar, paddingCount);
        return result.ToString();
    }

    /// <summary>
    ///     判断两个字符串是否等效，不区分大小写。
    /// </summary>
    /// <param name="str1">要比较的第一个字符串。</param>
    /// <param name="str2">要比较的第二个字符串。</param>
    /// <returns>
    ///     如果两个字符串在不区分大小写的情况下相等，则为 true；否则为 false。
    /// </returns>
    public static bool IsEquivalentTo(string str1, string str2) =>
        // 使用 String.Equals 方法进行不区分大小写的字符串比较
        string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     将字符串转换为驼峰命名法或帕斯卡命名法
    /// </summary>
    /// <param name="str">要转换的字符串</param>
    /// <param name="firstLetterTransform">首字母转换函数</param>
    /// <returns>转换后的字符串</returns>
    private static string ToCamelOrPascalCase(string str, Func<char, char> firstLetterTransform)
    {
        string text = Regex.Replace(str, "([_\\-])(?<char>[a-z])",
            match => match.Groups["char"].Value.ToUpperInvariant(),
            RegexOptions.IgnoreCase);
        return firstLetterTransform(text[0]) + text.Substring(1);
    }

    /// <summary>
    ///     将带有下划线（this_is_a_test）或连字符（this-is-a-test）的字符串转换为驼峰命名法（thisIsATest）。
    ///     驼峰命名法与帕斯卡命名法相同，只是首字母小写。
    /// </summary>
    /// <param name="str">要转换的字符串</param>
    /// <returns>转换后的字符串</returns>
    public static string ToCamelCase(this string str) => ToCamelOrPascalCase(str, char.ToLowerInvariant);

    /// <summary>
    ///     将带有下划线（this_is_a_test）或连字符（this-is-a-test）的字符串转换为帕斯卡命名法（ThisIsATest）。
    ///     帕斯卡命名法与驼峰命名法相同，只是首字母大写。
    /// </summary>
    /// <param name="str">要转换的字符串</param>
    /// <returns>转换后的字符串</returns>
    public static string ToPascalCase(this string str) => ToCamelOrPascalCase(str, char.ToUpperInvariant);

    /// <summary>
    ///     将驼峰命名法的字符串（thisIsATest）转换为连接符号（this-is-a-test）或下划线符号（this_is_a_test）的字符串。
    /// </summary>
    /// <param name="str">要转换的字符串</param>
    /// <param name="separator">在各段之间使用的分隔符</param>
    /// <returns>转换后的字符串</returns>
    public static string FromCamelCase(this string str, string separator)
    {
        // 确保首字母始终为小写
        str = char.ToLower(str[0]) + str.Substring(1);

        str = Regex.Replace(str.ToCamelCase(), "(?<char>[A-Z])",
            match => separator + match.Groups["char"].Value.ToLowerInvariant());
        return str;
    }
}
