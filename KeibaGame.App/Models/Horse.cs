namespace KeibaGame.App;

internal sealed class Horse
{
    /// <summary>馬番です。</summary>
    public required int Number { get; init; }
    /// <summary>馬名です。</summary>
    public required string Name { get; init; }
    /// <summary>スピード値です。</summary>
    public required int Speed { get; init; }
    /// <summary>運の良さです。</summary>
    public required int Luck { get; init; }
    /// <summary>オッズ倍率です。</summary>
    public required double Odds { get; set; }
    /// <summary>馬体画像です。</summary>
    public required Image Photo { get; init; }
    /// <summary>この馬に設定されたトラップ一覧です。</summary>
    public List<TrapEvent> TrapEvents { get; } = [];

    /// <summary>現在の走行位置(px)です。</summary>
    public double PositionPx { get; set; }
    /// <summary>ゴール済みかを示します。</summary>
    public bool Finished { get; set; }
    /// <summary>着順です。未確定時は <c>null</c> です。</summary>
    public int? Rank { get; set; }
}
