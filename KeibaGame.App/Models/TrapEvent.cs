namespace KeibaGame.App;

internal sealed class TrapEvent
{
    /// <summary>トラップの種類です。</summary>
    public required TrapType Type { get; init; }
    /// <summary>発生位置(px)です。</summary>
    public required int PositionPx { get; init; }
    /// <summary>発生可能になる最小経過秒です。</summary>
    public required double MinTriggerTimeSec { get; init; }
    /// <summary>発生済みかを示します。</summary>
    public bool Triggered { get; set; }
}
