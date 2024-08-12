using System;
using System.Collections.Generic;

namespace DominionWars.Common.Logger;

public struct ColorConf
{
    public Func<string, bool, string> Info;
    public Func<string, bool, string> Warn;
    public Func<string, bool, string> Error;
    public Func<string, bool, string> Debug;
    public Func<string, bool, string> Trace;

    public static readonly ColorConf Default = new()
    {
        Info = (msg, isGodot) =>
        {
            if (isGodot)
            {
                return $"[color=green]{msg}</color>";
            }

            return $"`e[{msg}`e[0m";
        },
        Warn = (msg, isGodot) =>
        {
            if (isGodot)
            {
                return $"<color=yellow>{msg}</color>";
            }

            return $"`e[{msg}`e[0m";
        },
        Error = (msg, isGodot) =>
        {
            if (isGodot)
            {
                return $"<color=red>{msg}</color>";
            }

            return $"`e[{msg}`e[0m";
        },
        Debug = (msg, isGodot) =>
        {
            if (isGodot)
            {
                return $"<color=gray>{msg}</color>";
            }

            return $"`e[{msg}`e[0m";
        },
        Trace = (msg, isGodot) =>
        {
            if (isGodot)
            {
                return $"<fgcolor=#FFD700>{msg}</color>";
            }

            return $"`e[{msg}`e[0m";
        }
    };
}

public static class LoggerCreator
{
    public static void Pip(LiquidNode first, LiquidNode after)
    {
        first.AddAfter(after);
        after.AddBefore(first);
    }

    public static void Merge(LiquidNode[] inNodes, LiquidNode outNode)
    {
        foreach (LiquidNode inNode in inNodes)
        {
            inNode.AddAfter(outNode);
            outNode.AddBefore(inNode);
        }
    }

    public static PumpNode GetInfoPump() => Logger.InfoPump;

    public static PumpNode GetWarnPump() => Logger.WarnPump;

    public static PumpNode GetErrorPump() => Logger.ErrorPump;

    public static PumpNode GetDebugPump() => Logger.DebugPump;

    public static PumpNode GetTracePump() => Logger.TracePump;

    public static RemotePumpNode GetRemotePumpNode(GetData getData) => new(getData);

    public static FilterValueNode GetValueFilter(Func<LogData, bool> action) => new(action);

    public static TransformValueNode GetValueTransform(Func<LogData, LogData> action) => new(action);

    public static FsPoolNode GetFsPool(PoolFileConf fsConf) => new(fsConf);

    public static ConsolePoolNode GetConsolePool(bool useSourceType) => new(useSourceType);

    public static NoPoolNode GetColorful(bool isGodot, ColorConf color = default)
    {
        ColorConf colorConf = EqualityComparer<ColorConf>.Default.Equals(color, default)
            ? ColorConf.Default
            : color;
        return GetValueTransform(log =>
        {
            switch (log.Source.Name)
            {
                case nameof(LogPumpType.Info):
                    log.Msg = colorConf.Info(log.Msg, isGodot);
                    break;
                case nameof(LogPumpType.Warn):
                    log.Msg = colorConf.Warn(log.Msg, isGodot);
                    break;
                case nameof(LogPumpType.Error):
                    log.Msg = colorConf.Error(log.Msg, isGodot);
                    break;
                case nameof(LogPumpType.Debug):
                    log.Msg = colorConf.Debug(log.Msg, isGodot);
                    break;
                case nameof(LogPumpType.Trace):
                    log.Msg = colorConf.Trace(log.Msg, isGodot);
                    break;
            }

            return log;
        });
    }

    public static NoPoolNode GetFormat(string format = null, string dateFormat = null) =>
        GetValueTransform(log =>
        {
            format ??= "[{date}][{source}][{scope}]: {msg}";
            dateFormat ??= "yyyy-M-d-HH-mm-ss-fff";
            log.Msg = format
                .Replace("{date}", log.Date.ToString(dateFormat))
                .Replace("{source}", log.Source.Name)
                .Replace("{scope}", log.Scope ?? "Main")
                .Replace("{msg}", log.Msg);
            return log;
        });
}
