namespace KeibaGame.App;

internal static class RaceFactory
{
    private static readonly Image[] Photos = HorseImageFactory.CreateFixedSet();

    /// <summary>1レース分の出走馬情報を生成します。</summary>
    /// <param name="random">乱数生成器です。</param>
    /// <returns>生成された出走馬一覧です。</returns>
    public static List<Horse> CreateRace(Random random)
    {
        var names = HorseNameRepository.GetAll().OrderBy(_ => random.Next()).Take(GameRules.HorseCount).ToArray();
        var horses = new List<Horse>(GameRules.HorseCount);

        for (var i = 0; i < GameRules.HorseCount; i++)
        {
            horses.Add(new Horse
            {
                Number = i + 1,
                Name = names[i],
                Speed = random.Next(50, 101),
                Luck = random.Next(0, 101),
                Odds = 0,
                Photo = Photos[i]
            });
        }

        SetOdds(horses);
        foreach (var horse in horses)
        {
            ConfigureTraps(horse, random);
        }

        return horses;
    }

    /// <summary>各馬のオッズを計算して設定します。</summary>
    /// <param name="horses">オッズ設定対象の馬一覧です。</param>
    private static void SetOdds(List<Horse> horses)
    {
        var effective = horses.Select(h => h.Speed * (1 + (h.Luck * 0.002))).ToArray();
        var average = effective.Average();

        for (var i = 0; i < horses.Count; i++)
        {
            var raw = (average / effective[i]) * 2.1;
            raw = Math.Max(raw, 1.1);
            horses[i].Odds = Math.Floor(raw * 10) / 10.0;
        }
    }

    /// <summary>指定した馬にトラップ情報を割り当てます。</summary>
    /// <param name="horse">トラップ設定対象の馬です。</param>
    /// <param name="random">乱数生成器です。</param>
    private static void ConfigureTraps(Horse horse, Random random)
    {
        var types = new[] { TrapType.Headwind, TrapType.BadTrack, TrapType.Temper }
            .OrderBy(_ => random.Next())
            .ToArray();

        var positions = new List<int>();
        while (positions.Count < 3)
        {
            var candidate = random.Next(0, 501);
            if (positions.All(existing => Math.Abs(existing - candidate) >= 100))
            {
                positions.Add(candidate);
            }
        }

        positions.Sort();
        horse.TrapEvents.Clear();
        for (var i = 0; i < 3; i++)
        {
            horse.TrapEvents.Add(new TrapEvent
            {
                Type = types[i],
                PositionPx = positions[i],
                MinTriggerTimeSec = 2 * (i + 1),
                Triggered = false
            });
        }
    }
}

