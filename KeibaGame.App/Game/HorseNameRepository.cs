namespace KeibaGame.App;

internal static class HorseNameRepository
{
    private static readonly Lazy<IReadOnlyList<string>> CachedNames = new(LoadNames);

    /// <summary>馬名CSVから読み込んだ馬名一覧を返します。</summary>
    /// <returns>重複を除いた馬名一覧です。</returns>
    public static IReadOnlyList<string> GetAll() => CachedNames.Value;

    /// <summary>CSVファイルから馬名一覧を読み込みます。</summary>
    /// <returns>正規化済みの馬名一覧です。</returns>
    private static IReadOnlyList<string> LoadNames()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "horse_names.csv");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("馬名CSVが見つかりません。", filePath);
        }

        var names = File.ReadAllLines(filePath)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Split(',')[0].Trim())
            .Distinct()
            .ToList();

        if (names.Count < GameRules.HorseCount)
        {
            throw new InvalidOperationException($"馬名CSVの件数が不足しています。必要: {GameRules.HorseCount}, 実際: {names.Count}");
        }

        return names;
    }
}

