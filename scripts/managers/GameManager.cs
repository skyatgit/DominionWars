using System;
using DominionWars.Common.GameException;
using Godot;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DominionWars.Common.Logger;
using YAT;

namespace DominionWars.Manager;

public static class GameManager
{
    public delegate void GameOverHandler();
    private static readonly Dictionary<ExceptionType, List<Action<GameStopException>>> s_gameExceptionCallback = new();

    public static bool CanUseConsole
    {
        get => Yat.CanConsoleUsed;
        set => Yat.CanConsoleUsed = value;
    }
    public static string UserDataDir { get; set; }
    public static bool IsStop { get; set; }
    public static event GameOverHandler GameOverEvent;

    private static void Init()
    {
        UserDataDir = OS.GetUserDataDir();
        IsStop = false;
        CanUseConsole = false;
        InitConsole();
        InitManager();
    }

    private static void InitManager()
    {

    }

    private static void InitConsole()
    {
        Yat.Title = "DominionWars";
        Yat.ScreenPrompt += "\nUsed [code]list[/code] show all commands.";
        Yat.HasContextMenu = false;
        Yat.IsShowPath = false;
        Yat.SupportMultipleTerminal = false;
        Yat.CanConsoleUsed = false;
    }

    public static void Start()
    {
        try
        {
            Init();
            CanUseConsole = true;
        }
        catch (GameStopException e)
        {
            ErrorGameOver(e);
        }
        catch (Exception e)
        {
            Logger.Error(e, "MainInit");
            throw;
        }
    }

    public static void MainLoop()
    {
        try
        {
        }
        catch (GameStopException e)
        {
            ErrorGameOver(e);
        }
        catch (Exception e)
        {
            Logger.Error(e, "MainLoop");
            throw;
        }
    }

    public static void GameOver()
    {
        if (GameOverEvent != null)
        {
            GameOverEvent();
        }

        IsStop = true;
    }

    private static void DefineLogger()
    {
#if TOOLS
        LoggerCreator.GetFormat("[{date}][{source}][{scope}]: {msg}", "yyyy-MM-dd HH:mm:ss")
            .Merge(new LiquidNode[]
            {
                Logger.InfoPump, Logger.WarnPump, Logger.ErrorPump, Logger.DebugPump, Logger.TracePump
            })
            .ConsolePool(true);
        LiquidNode[] main = { Logger.InfoPump, Logger.WarnPump, Logger.DebugPump, Logger.TracePump };
#else
            LiquidNode[] main = { Logger.InfoPump, Logger.WarnPump };
#endif
        // main log file
        LoggerCreator.GetFormat("[{date}][{source}][{scope}]: {msg}", "yyyy-MM-dd HH:mm:ss")
            .Merge(main)
            .FsPool(new PoolFileConf
            {
                Dir = Path.Join(UserDataDir, "log"),
                Encoding = Encoding.UTF8,
                Filename = "main-{0}.log",
                PathIsFormatString = true
            });
        // error log file
        LoggerCreator.GetErrorPump()
            .Format("[{date}][{source}][{scope}]: {msg}", "yyyy-MM-dd HH:mm:ss")
            .FsPool(new PoolFileConf
            {
                Dir = Path.Join(UserDataDir, "log"),
                Encoding = Encoding.UTF8,
                Filename = "error_{0}.log",
                PathIsFormatString = true
            });
    }

    public static void ErrorGameOver(GameStopException e)
    {
        if (s_gameExceptionCallback.ContainsKey(e.ExceptionType))
        {
            s_gameExceptionCallback[e.ExceptionType].ForEach(action => action(e));
        }
        IsStop = true;
    }

    public static void AddGameStopCallback(ExceptionType type, Action<GameStopException> action) =>
        s_gameExceptionCallback[type].Add(action);
}