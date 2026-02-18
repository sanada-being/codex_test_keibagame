namespace KeibaGame.App;

internal sealed class RaceSimulator
{
    private sealed class ActiveEffect
    {
        /// <summary>効果中の速度倍率です。</summary>
        public required double Multiplier { get; init; }
        /// <summary>効果終了時刻(シミュレーション秒)です。</summary>
        public required double EndAtSec { get; init; }
    }

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 100 };
    private readonly Random _random = new();
    private readonly Dictionary<Horse, List<ActiveEffect>> _effects = new();

    private List<Horse> _horses = [];
    private int _finishOrder;
    private double _simulationSec;
    private double _speedMultiplier = GameRules.MinRaceSpeedMultiplier;

    /// <summary>レース状態更新時に発生します。</summary>
    public event Action? Updated;
    /// <summary>トラップ発生メッセージ確定時に発生します。</summary>
    public event Action<string>? TrapOccurred;
    /// <summary>全馬ゴール後に最終結果を通知します。</summary>
    public event Action<RaceResult>? Completed;

    /// <summary>レースの進行速度倍率です。</summary>
    public double SpeedMultiplier
    {
        get => _speedMultiplier;
        set => _speedMultiplier = GameRules.NormalizeRaceSpeed(value);
    }

    /// <summary>レースシミュレータを初期化します。</summary>
    public RaceSimulator()
    {
        _timer.Tick += (_, _) => Tick();
    }

    /// <summary>指定した出走馬でレースを開始します。</summary>
    /// <param name="horses">出走馬一覧です。</param>
    public void Start(List<Horse> horses)
    {
        _horses = horses;
        _effects.Clear();
        _finishOrder = 0;
        _simulationSec = 0;

        foreach (var horse in _horses)
        {
            horse.PositionPx = 0;
            horse.Finished = false;
            horse.Rank = null;
            foreach (var trap in horse.TrapEvents)
            {
                trap.Triggered = false;
            }

            _effects[horse] = [];
        }

        _timer.Start();
    }

    /// <summary>1ティック分レース状態を更新します。</summary>
    private void Tick()
    {
        var deltaSec = 0.1 * _speedMultiplier;
        _simulationSec += deltaSec;

        foreach (var horse in _horses.Where(h => !h.Finished))
        {
            TriggerTraps(horse, _simulationSec);
            var active = _effects[horse];
            active.RemoveAll(effect => _simulationSec >= effect.EndAtSec);

            var multiplier = active.Aggregate(1.0, (current, effect) => current * effect.Multiplier);
            var movePerSec = horse.Speed * 0.4 * multiplier;
            horse.PositionPx = Math.Min(GameRules.GoalDistancePx, horse.PositionPx + (movePerSec * deltaSec));

            if (horse.PositionPx >= GameRules.GoalDistancePx && !horse.Finished)
            {
                horse.Finished = true;
                _finishOrder++;
                horse.Rank = _finishOrder;
            }
        }

        Updated?.Invoke();

        if (_horses.All(h => h.Finished))
        {
            _timer.Stop();
            Completed?.Invoke(new RaceResult
            {
                RankedHorses = _horses.OrderBy(h => h.Rank).ToList()
            });
        }
    }

    /// <summary>発生条件を満たしたトラップを判定・適用します。</summary>
    /// <param name="horse">判定対象の馬です。</param>
    /// <param name="now">現在のシミュレーション秒です。</param>
    private void TriggerTraps(Horse horse, double now)
    {
        foreach (var trap in horse.TrapEvents.Where(t => !t.Triggered))
        {
            if (horse.PositionPx < trap.PositionPx || now < trap.MinTriggerTimeSec)
            {
                continue;
            }

            trap.Triggered = true;
            var dodgeRate = Math.Clamp(0.25 + (horse.Luck * 0.005), 0.25, 0.75);
            if (_random.NextDouble() <= dodgeRate)
            {
                TrapOccurred?.Invoke($"{horse.Name} は {TrapLabel(trap.Type)} を回避した！");
                continue;
            }

            _effects[horse].Add(new ActiveEffect
            {
                Multiplier = TrapMultiplier(trap.Type),
                EndAtSec = now + 2
            });
            TrapOccurred?.Invoke($"{horse.Name} に {TrapLabel(trap.Type)} が発生！");
        }
    }

    private static double TrapMultiplier(TrapType type) => type switch
    {
        TrapType.Headwind => 0.9,
        TrapType.BadTrack => 0.8,
        TrapType.Temper => 0.7,
        _ => 1.0
    };

    private static string TrapLabel(TrapType type) => type switch
    {
        TrapType.Headwind => "逆風",
        TrapType.BadTrack => "馬場が悪い",
        TrapType.Temper => "気性が荒くなる",
        _ => "トラップ"
    };
}

