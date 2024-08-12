using Godot;
using YAT;
using YAT.Enums;

namespace DominionWars.Common.Logger;

public class ConsolePoolNode : PoolNode
{
    public readonly bool UseSourceType;

    public ConsolePoolNode(bool useSourceType) => UseSourceType = useSourceType;

#if TOOLS
    public override void On(LogData logData)
    {
        if (UseSourceType)
        {
            switch (logData.Source.Name)
            {
                case nameof(LogPumpType.Info):
                    GD.PrintRich(logData.Msg);
                    break;
                case nameof(LogPumpType.Warn):
                    GD.PushWarning(logData.Msg);
                    break;
                case nameof(LogPumpType.Error):
                    GD.PushError(logData.Msg);
                    break;
                case nameof(LogPumpType.Debug):
                    GD.PrintRich("[DEBUG]" + logData.Msg);
                    break;
                case nameof(LogPumpType.Trace):
                    GD.PrintRich("[TRACE]" + logData.Msg);
                    break;
                default:
                    GD.PrintRich(logData.Msg);
                    break;
            }
        }
        else
        {
            Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Error);
            GD.Print(logData.Msg);
        }
    }
#else
        public override void On(LogData logData)
        {
            if (UseSourceType)
            {
                switch (logData.Source.Name)
                {
                    case nameof(LogPumpType.Info):
                        YAT.Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Normal);
                        break;
                    case nameof(LogPumpType.Warn):
                        YAT.Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Warning);
                        break;
                    case nameof(LogPumpType.Error):
                        YAT.Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Error);
                        break;
                    case nameof(LogPumpType.Debug):
                        YAT.Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Normal);
                        break;
                    case nameof(LogPumpType.Trace):
                        YAT.Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Normal);
                        break;
                    default:
                        YAT.Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Normal);
                        break;
                }
            }
            else
            {
                YAT.Yat.Instance.CurrentTerminal.Print(logData.Msg, EPrintType.Normal);
            }
        }
#endif
}
