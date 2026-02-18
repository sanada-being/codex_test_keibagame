using System.Drawing.Drawing2D;

namespace KeibaGame.App;

internal sealed class RaceForm : Form
{
    private readonly RaceSimulator _simulator = new();
    private readonly List<Horse> _horses;
    private readonly Label _trapLabel;
    private readonly SmoothPanel _trackPanel;
    private readonly List<Label> _rankLabels = [];
    private readonly Font _laneFont = new("Yu Gothic UI", 9, FontStyle.Bold);
    private readonly Font _goalFont = new("Yu Gothic UI", 12, FontStyle.Bold);

    /// <summary>レース終了後の結果です。</summary>
    public RaceResult? Result { get; private set; }

    /// <summary>レース画面を初期化します。</summary>
    /// <param name="session">現在のゲームセッションです。</param>
    /// <param name="selectedHorse">購入した馬です。</param>
    /// <param name="betAmount">購入金額です。</param>
    /// <param name="horses">出走馬一覧です。</param>
    public RaceForm(GameSession session, Horse selectedHorse, int betAmount, List<Horse> horses)
    {
        _horses = horses;
        _simulator.SpeedMultiplier = session.RaceSpeedMultiplier;

        Text = "レース画面";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1024, 700);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        UiTheme.ApplyForm(this);
        DoubleBuffered = true;

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var ticket = new Label
        {
            Text = $"購入馬券: 単勝 {selectedHorse.Name} / {betAmount:N0}円",
            Font = new Font("Yu Gothic UI", 13, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 18),
            BackColor = Color.Transparent
        };

        _trapLabel = new Label
        {
            Text = "実況: スタート待機中...",
            Font = new Font("Yu Gothic UI", 12, FontStyle.Bold),
            AutoSize = true,
            ForeColor = UiTheme.Gold,
            Location = new Point(24, 50),
            BackColor = Color.Transparent
        };

        var speedDisplay = new Label
        {
            Text = $"速度: {GameRules.ToSpeedLabel(session.RaceSpeedMultiplier)}",
            Font = new Font("Yu Gothic UI", 12, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.White,
            Location = new Point(820, 92),
            BackColor = Color.Transparent
        };

        _trackPanel = new SmoothPanel
        {
            Location = new Point(20, 86),
            Size = new Size(760, 590),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(36, 98, 54)
        };
        _trackPanel.Paint += (_, e) => DrawTrack(e.Graphics);

        var rankingTitle = new Label
        {
            Text = "RANKING",
            Font = new Font("Yu Gothic UI", 24, FontStyle.Bold),
            ForeColor = UiTheme.Gold,
            AutoSize = true,
            Location = new Point(820, 150),
            BackColor = Color.Transparent
        };

        for (var i = 0; i < 5; i++)
        {
            var label = new Label
            {
                Font = new Font("Yu Gothic UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(810, 210 + (i * 80)),
                BackColor = Color.Transparent
            };
            _rankLabels.Add(label);
            Controls.Add(label);
        }

        Controls.Add(ticket);
        Controls.Add(_trapLabel);
        Controls.Add(speedDisplay);
        Controls.Add(_trackPanel);
        Controls.Add(rankingTitle);

        _simulator.Updated += () =>
        {
            _trackPanel.Invalidate();
            UpdateRankingLabels();
        };
        _simulator.TrapOccurred += message => _trapLabel.Text = $"実況: {message}";
        _simulator.Completed += result =>
        {
            Result = result;
            DialogResult = DialogResult.OK;
            Close();
        };

        UpdateRankingLabels();
        Shown += (_, _) => _simulator.Start(_horses);
    }

    /// <summary>レースコースと馬の現在位置を描画します。</summary>
    /// <param name="g">描画先グラフィックスです。</param>
    private void DrawTrack(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var marginX = 70;
        var laneHeight = 108;
        var finishX = marginX + GameRules.GoalDistancePx;

        using var bg = new LinearGradientBrush(_trackPanel.ClientRectangle, Color.FromArgb(55, 130, 76), Color.FromArgb(25, 75, 43), 90f);
        g.FillRectangle(bg, _trackPanel.ClientRectangle);

        using var stripePen = new Pen(Color.FromArgb(28, 255, 255, 255), 2);
        for (var y = 0; y < _trackPanel.Height; y += 26)
        {
            g.DrawLine(stripePen, 0, y, _trackPanel.Width, y);
        }

        using var lanePen = new Pen(Color.FromArgb(230, 245, 230), 2f);
        using var nameBrush = new SolidBrush(Color.White);

        for (var i = 0; i < _horses.Count; i++)
        {
            var y = 40 + (i * laneHeight);
            g.DrawLine(lanePen, marginX, y + 58, finishX, y + 58);

            var horse = _horses[i];
            var x = marginX + (int)horse.PositionPx;

            g.DrawString($"{horse.Number}", _laneFont, nameBrush, 14, y + 30);
            g.DrawString(horse.Name, _laneFont, nameBrush, 35, y + 32);
            g.DrawImage(horse.Photo, x, y + 22, 96, 58);
        }

        g.FillRectangle(Brushes.White, finishX, 16, 6, _trackPanel.Height - 30);
        g.FillRectangle(Brushes.Red, finishX + 6, 16, 6, _trackPanel.Height - 30);
        g.DrawString("GOAL", _goalFont, Brushes.WhiteSmoke, finishX - 12, 12);
    }

    /// <summary>ゴール済み馬の着順ラベルを更新します。</summary>
    private void UpdateRankingLabels()
    {
        var finished = _horses
            .Where(h => h.Rank.HasValue)
            .OrderBy(h => h.Rank)
            .ToList();

        for (var i = 0; i < _rankLabels.Count; i++)
        {
            if (i >= finished.Count)
            {
                _rankLabels[i].Text = string.Empty;
                continue;
            }

            var horse = finished[i];
            _rankLabels[i].Text = $"{horse.Rank}着 {horse.Name}";
            _rankLabels[i].ForeColor = horse.Rank switch
            {
                1 => Color.Gold,
                2 => Color.DeepSkyBlue,
                3 => Color.HotPink,
                _ => Color.White
            };
        }
    }
}

