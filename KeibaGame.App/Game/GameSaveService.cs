using System.Text.Json;

namespace KeibaGame.App;

internal static class GameSaveService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private static string SaveDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeibaGame");
    private static string SaveFilePath => Path.Combine(SaveDirectory, "save.json");

    /// <summary>セーブファイルの存在有無を返します。</summary>
    /// <returns>セーブファイルが存在する場合は <c>true</c> です。</returns>
    public static bool Exists() => File.Exists(SaveFilePath);

    /// <summary>セーブデータを読み込み、セッションへ復元します。</summary>
    /// <param name="session">読み込み結果を格納するセッションです。</param>
    /// <returns>読み込みに成功した場合は <c>true</c> です。</returns>
    public static bool TryLoad(out GameSession session)
    {
        session = new GameSession();

        if (!Exists())
        {
            return false;
        }

        try
        {
            var json = File.ReadAllText(SaveFilePath);
            var dto = JsonSerializer.Deserialize<GameSessionDto>(json, JsonOptions);
            if (dto is null)
            {
                return false;
            }

            session.Balance = dto.Balance;
            session.TotalRaces = dto.TotalRaces;
            session.TotalBets = dto.TotalBets;
            session.TotalPayout = dto.TotalPayout;
            session.HitCount = dto.HitCount;
            session.RaceSpeedMultiplier = GameRules.NormalizeRaceSpeed(dto.RaceSpeedMultiplier);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>現在のセッション状態をセーブします。</summary>
    /// <param name="session">保存対象のセッションです。</param>
    public static void Save(GameSession session)
    {
        Directory.CreateDirectory(SaveDirectory);
        var dto = new GameSessionDto
        {
            Balance = session.Balance,
            TotalRaces = session.TotalRaces,
            TotalBets = session.TotalBets,
            TotalPayout = session.TotalPayout,
            HitCount = session.HitCount,
            RaceSpeedMultiplier = session.RaceSpeedMultiplier
        };
        var json = JsonSerializer.Serialize(dto, JsonOptions);
        File.WriteAllText(SaveFilePath, json);
    }

    /// <summary>セーブデータを削除します。</summary>
    public static void Delete()
    {
        if (Exists())
        {
            File.Delete(SaveFilePath);
        }
    }

    private sealed class GameSessionDto
    {
        /// <summary>保存時の所持金です。</summary>
        public int Balance { get; init; }
        /// <summary>保存時の累計レース数です。</summary>
        public int TotalRaces { get; init; }
        /// <summary>保存時の累計投資額です。</summary>
        public int TotalBets { get; init; }
        /// <summary>保存時の累計払戻額です。</summary>
        public int TotalPayout { get; init; }
        /// <summary>保存時の累計的中数です。</summary>
        public int HitCount { get; init; }
        /// <summary>保存時のレース速度設定です。</summary>
        public double RaceSpeedMultiplier { get; init; }
    }
}

