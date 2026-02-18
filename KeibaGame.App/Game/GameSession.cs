namespace KeibaGame.App;

internal sealed class GameSession
{
    /// <summary>現在の所持金です。</summary>
    public int Balance { get; set; } = GameRules.InitialBalance;
    /// <summary>累計レース数です。</summary>
    public int TotalRaces { get; set; }
    /// <summary>累計投資額です。</summary>
    public int TotalBets { get; set; }
    /// <summary>累計払戻額です。</summary>
    public int TotalPayout { get; set; }
    /// <summary>累計的中数です。</summary>
    public int HitCount { get; set; }
    /// <summary>レース速度倍率設定です。</summary>
    public double RaceSpeedMultiplier { get; set; } = GameRules.MinRaceSpeedMultiplier;

    /// <summary>クリア条件を満たしているかを返します。</summary>
    public bool IsClear => Balance >= GameRules.ClearBalance;
    /// <summary>破産条件を満たしているかを返します。</summary>
    public bool IsBankrupt => Balance < GameRules.BankruptThreshold;
    /// <summary>累計的中率を返します。</summary>
    public double HitRate => TotalRaces == 0 ? 0 : (double)HitCount / TotalRaces;

    /// <summary>セッションを新規開始状態にリセットします。</summary>
    public void Reset()
    {
        Balance = GameRules.InitialBalance;
        TotalRaces = 0;
        TotalBets = 0;
        TotalPayout = 0;
        HitCount = 0;
    }

    /// <summary>別セッションの値を現在セッションへ反映します。</summary>
    /// <param name="source">反映元のセッションです。</param>
    public void ApplyFrom(GameSession source)
    {
        Balance = source.Balance;
        TotalRaces = source.TotalRaces;
        TotalBets = source.TotalBets;
        TotalPayout = source.TotalPayout;
        HitCount = source.HitCount;
        RaceSpeedMultiplier = GameRules.NormalizeRaceSpeed(source.RaceSpeedMultiplier);
    }

    /// <summary>1レース分の戦績を加算します。</summary>
    /// <param name="betAmount">賭け金です。</param>
    /// <param name="payout">払戻金です。</param>
    /// <param name="hit">的中した場合は <c>true</c> です。</param>
    public void RecordRace(int betAmount, int payout, bool hit)
    {
        TotalRaces++;
        TotalBets += betAmount;
        TotalPayout += payout;
        if (hit)
        {
            HitCount++;
        }
    }
}

