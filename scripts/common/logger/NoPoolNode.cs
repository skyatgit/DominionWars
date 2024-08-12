using System;

namespace DominionWars.Common.Logger;

public abstract class NoPoolNode : LiquidNode
{
    protected NoPoolNode(LiquidNodeType type) : base(type)
    {
    }

    public NoPoolNode Merge(LiquidNode[] nodes)
    {
        LoggerCreator.Merge(nodes, this);
        return this;
    }

    public LiquidNode Pip(LiquidNode node)
    {
        LoggerCreator.Pip(this, node);
        return node;
    }

    public FilterValueNode Filter(Func<LogData, bool> filterFunc)
    {
        FilterValueNode node = LoggerCreator.GetValueFilter(filterFunc);
        LoggerCreator.Pip(this, node);
        return node;
    }

    public TransformValueNode Transform(Func<LogData, LogData> transformFunc)
    {
        TransformValueNode node = LoggerCreator.GetValueTransform(transformFunc);
        LoggerCreator.Pip(this, node);
        return node;
    }

    public FsPoolNode FsPool(PoolFileConf fsConf)
    {
        FsPoolNode node = LoggerCreator.GetFsPool(fsConf);
        LoggerCreator.Pip(this, node);
        return node;
    }

    public ConsolePoolNode ConsolePool(bool useSourceType)
    {
        ConsolePoolNode node = LoggerCreator.GetConsolePool(useSourceType);
        LoggerCreator.Pip(this, node);
        return node;
    }

    public NoPoolNode Colorful(bool isUnity, ColorConf color = default)
    {
        NoPoolNode node = LoggerCreator.GetColorful(isUnity, color);
        LoggerCreator.Pip(this, node);
        return node;
    }

    public NoPoolNode Format(string format = null, string dateFormat = null)
    {
        NoPoolNode node = LoggerCreator.GetFormat(format, dateFormat);
        LoggerCreator.Pip(this, node);
        return node;
    }
}
