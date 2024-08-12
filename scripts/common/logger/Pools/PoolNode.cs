namespace DominionWars.Common.Logger;

public class PoolNode : LiquidNode
{
    protected PoolNode() : base(LiquidNodeType.Pool)
    {
    }

    public override void On(LogData logData) => After.ForEach(node => node.On(logData));

    public new void AddAfter(LiquidNode node)
    {
        string errMsg = "Unable to add subsequent nodes to PoolNode!";
        Logger.LogStrictCheck(errMsg);
    }

    public PoolNode Merge(LiquidNode[] nodes)
    {
        LoggerCreator.Merge(nodes, this);
        return this;
    }
}
