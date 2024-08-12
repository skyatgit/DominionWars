using System;

namespace DominionWars.Common.Logger;

public class TransformValueNode : NoPoolNode
{
    private readonly Func<LogData, LogData> _transformFunc;

    public TransformValueNode(Func<LogData, LogData> transform) : base(LiquidNodeType.Valve) =>
        _transformFunc = transform;

    public override void On(LogData logData)
    {
        LogData newLog = logData;
        if (_transformFunc != null)
        {
            newLog = _transformFunc(newLog);
        }
        else
        {
            string errorMsg = "Transform function cannot be empty!";
            Logger.LogStrictCheck(errorMsg);
        }

        After.ForEach(node => node.On(newLog));
    }
}
