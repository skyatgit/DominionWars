using System;

namespace DominionWars.Common.Logger;

public static class Logger
{
    public static readonly string Version = "0.1.0";
    public static readonly PumpNode InfoPump = new();
    public static readonly PumpNode WarnPump = new();
    public static readonly PumpNode ErrorPump = new();
    public static readonly PumpNode DebugPump = new();
    public static readonly PumpNode TracePump = new();

    /// <summary>
    ///     获取或设置是否使用严格模式。
    /// </summary>
    public static bool UseStrict = true;

    /// <summary>
    ///     记录信息级别的日志。
    /// </summary>
    /// <param name="msg">要记录的消息。</param>
    /// <param name="scope">消息的作用域（可选）。</param>
    public static void Info(string msg, string scope = null) => _log(msg, scope ?? "", InfoPump, LogPumpType.Info);

    /// <summary>
    ///     记录警告级别的日志。
    /// </summary>
    /// <param name="msg">要记录的消息。</param>
    /// <param name="scope">消息的作用域（可选）。</param>
    public static void Warn(string msg, string scope = null) => _log(msg, scope ?? "", WarnPump, LogPumpType.Warn);

    /// <summary>
    ///     记录包含异常信息的警告级别的日志。会同时记录错误栈。
    /// </summary>
    /// <param name="e">要记录的异常。</param>
    /// <param name="scope">消息的作用域（可选）。</param>
    public static void Warn(Exception e, string scope = null)
    {
        string msg = $"{e.Message}\n{StringUtil.AddIndentation(e.StackTrace, 2)}";
        _log(msg, scope ?? "", InfoPump, LogPumpType.Warn);
    }

    /// <summary>
    ///     记录错误级别的日志。
    /// </summary>
    /// <param name="msg">要记录的消息。</param>
    /// <param name="scope">消息的作用域（可选）。</param>
    public static void Error(string msg, string scope = null) => _log(msg, scope ?? "", ErrorPump, LogPumpType.Error);

    /// <summary>
    ///     记录包含异常信息的错误级别的日志。会同时记录错误栈。
    /// </summary>
    /// <param name="e">要记录的异常。</param>
    /// <param name="scope">消息的作用域（可选）。</param>
    public static void Error(Exception e, string scope = null)
    {
        string msg = $"{e.Message}\n{StringUtil.AddIndentation(e.StackTrace, 2)}";
        _log(msg, scope ?? "", ErrorPump, LogPumpType.Error);
    }

    /// <summary>
    ///     记录调试级别的日志。
    /// </summary>
    /// <param name="msg">要记录的消息。</param>
    /// <param name="scope">消息的作用域（可选）。</param>
    public static void Debug(string msg, string scope = null) => _log(msg, scope ?? "", DebugPump, LogPumpType.Debug);

    /// <summary>
    ///     记录追踪级别的日志。
    /// </summary>
    /// <param name="msg">要记录的消息。</param>
    /// <param name="scope">消息的作用域（可选）。</param>
    public static void Trace(string msg, string scope = null) => _log(msg, scope ?? "", TracePump, LogPumpType.Trace);

    /// <summary>
    ///     私有方法，用于实际记录日志。
    /// </summary>
    /// <param name="msg">要记录的消息。</param>
    /// <param name="scope">消息的作用域。</param>
    /// <param name="pump">要使用的 PumpNode 实例。</param>
    /// <param name="type">日志的类型。</param>
    private static void _log(string msg, string scope, PumpNode pump, LogPumpType type) =>
        pump.On(new LogData
        {
            Date = DateTime.Now,
            Msg = msg,
            Scope = scope,
            Source = type,
            Value = msg
        });

    /// <summary>
    ///     在严格模式下，抛出日志系统的内部异常，否则只以 Warn 形式记录。
    /// </summary>
    /// <param name="errorMsg">日志系统内部错误的信息。</param>
    public static void LogStrictCheck(string errorMsg)
    {
        if (UseStrict)
        {
            throw new Exception(errorMsg);
        }

        Warn(errorMsg, "Logger");
    }
}
