namespace DominionWars.Common.Logger;

public class LogPumpType : Enumeration
{
    private LogPumpType(int id, string name) : base(id, name)
    {
    }

    public static LogPumpType Any => new(0, "Any");
    public static LogPumpType Info => new(1, "Info");
    public static LogPumpType Warn => new(2, "Warn");
    public static LogPumpType Error => new(3, "Error");
    public static LogPumpType Debug => new(4, "Debug");
    public static LogPumpType Trace => new(5, "Trace");
    public static LogPumpType Console => new(6, "Console");
    public static LogPumpType Remote => new(7, "Remote");
}

public class PumpNode : NoPoolNode
{
    public PumpNode() : base(LiquidNodeType.Pump)
    {
    }

    public new void AddBefore(LiquidNode node)
    {
        const string errorMsg = "Unable to add previous Node to PoolNode!";
        Logger.LogStrictCheck(errorMsg);
    }

    public override void On(LogData logData) => After.ForEach(node => node.On(logData));
}
