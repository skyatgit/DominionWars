using System;
using System.Collections.Generic;

namespace DominionWars.Common.Logger;

public enum LiquidNodeType
{
    Pump,
    Valve,
    Pool
}

public struct LogData
{
    public DateTime Date;
    public string Msg;
    public string Scope;
    public LogPumpType Source;
    public string Value;
}

public abstract class LiquidNode
{
    protected readonly List<LiquidNode> After;
    protected readonly List<LiquidNode> Before;
    public LiquidNodeType Type;

    protected LiquidNode(LiquidNodeType type)
    {
        Type = type;
        Before = new List<LiquidNode>();
        After = new List<LiquidNode>();
    }

    public void AddBefore(LiquidNode node) => Before.Add(node);

    public void AddAfter(LiquidNode node) => After.Add(node);

    public abstract void On(LogData logData);
}
