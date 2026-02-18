namespace KeibaGame.App;

internal static class GameRules
{
    /// <summary>ゲーム開始時の所持金です。</summary>
    public const int InitialBalance = 10000;
    /// <summary>ゲームクリア判定に使う所持金です。</summary>
    public const int ClearBalance = 50000;
    /// <summary>破産判定に使う最低所持金です。</summary>
    public const int BankruptThreshold = 100;

    /// <summary>賭け金の最小値です。</summary>
    public const int MinBet = 100;
    /// <summary>賭け金の入力単位です。</summary>
    public const int BetUnit = 100;

    /// <summary>1レースの出走頭数です。</summary>
    public const int HorseCount = 5;
    /// <summary>レースのゴール距離(px)です。</summary>
    public const int GoalDistancePx = 600;

    /// <summary>レース速度倍率の最小値です。</summary>
    public const double MinRaceSpeedMultiplier = 1.0;
    /// <summary>レース速度倍率の最大値です。</summary>
    public const double MaxRaceSpeedMultiplier = 4.0;

    /// <summary>選択可能なレース速度倍率一覧です。</summary>
    public static readonly double[] RaceSpeedOptions = [1.0, 2.0, 4.0];

    /// <summary>速度倍率を有効値へ正規化します。</summary>
    /// <param name="speed">正規化前の速度倍率です。</param>
    /// <returns>有効な速度倍率です。</returns>
    public static double NormalizeRaceSpeed(double speed)
    {
        if (RaceSpeedOptions.Contains(speed))
        {
            return speed;
        }

        return MinRaceSpeedMultiplier;
    }

    /// <summary>速度倍率を画面表示用ラベルへ変換します。</summary>
    /// <param name="speed">変換対象の速度倍率です。</param>
    /// <returns>画面表示用の速度ラベルです。</returns>
    public static string ToSpeedLabel(double speed) => $"{NormalizeRaceSpeed(speed):0}x";

    /// <summary>画面表示ラベルを速度倍率へ変換します。</summary>
    /// <param name="label">画面表示用の速度ラベルです。</param>
    /// <returns>対応する速度倍率です。</returns>
    public static double ParseSpeedLabel(string? label)
    {
        return label switch
        {
            "2x" => 2.0,
            "4x" => 4.0,
            _ => 1.0
        };
    }
}

