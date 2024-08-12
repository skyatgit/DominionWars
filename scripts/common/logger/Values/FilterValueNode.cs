using System;

namespace DominionWars.Common.Logger;

public class FilterValueNode : NoPoolNode
{
    private readonly Func<LogData, bool> _filterFunc;

    public FilterValueNode(Func<LogData, bool> filter) : base(LiquidNodeType.Valve) => _filterFunc = filter;

    public override void On(LogData logData)
    {
        if (_filterFunc != null && !_filterFunc(logData))
        {
            return;
        }

        After.ForEach(node => node.On(logData));
    }
}
