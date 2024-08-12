namespace YAT.Types;

public record NetworkingOptions
{
    public ushort Timeout { get; init; } = 10000;
    public ushort Ttl { get; init; } = 30;
    public ushort BufferSize { get; init; } = 32;
    public bool DontFragment { get; init; } = true;
    public ushort Delay { get; init; } = 1000;
}
