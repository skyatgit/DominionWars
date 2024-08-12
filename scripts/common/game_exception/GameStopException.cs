namespace DominionWars.Common.GameException;

public enum ExceptionType
{
    Mod,
    Steamworks
}

public class GameStopException : System.Exception
{
    public readonly ExceptionType ExceptionType;

    public GameStopException(ExceptionType exceptionType, string message) : base(message) =>
        ExceptionType = exceptionType;
}