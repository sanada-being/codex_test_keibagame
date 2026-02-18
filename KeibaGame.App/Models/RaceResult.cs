namespace KeibaGame.App;

internal sealed class RaceResult
{
    /// <summary>着順確定済みの馬一覧です。</summary>
    public required IReadOnlyList<Horse> RankedHorses { get; init; }
}
