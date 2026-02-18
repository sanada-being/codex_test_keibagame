namespace KeibaGame.App;

internal sealed class ResultForm : Form
{
    /// <summary>結果画面で選択された次アクションです。</summary>
    public ResultAction Action { get; private set; }

    /// <summary>結果画面を初期化します。</summary>
    /// <param name="session">現在のゲームセッションです。</param>
    /// <param name="selectedHorse">購入した馬です。</param>
    /// <param name="betAmount">購入金額です。</param>
    /// <param name="payout">払戻金です。</param>
    /// <param name="result">レース結果です。</param>
    public ResultForm(GameSession session, Horse selectedHorse, int betAmount, int payout, RaceResult result)
    {
        Text = "レース結果";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(980, 680);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        UiTheme.ApplyForm(this);

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var hit = selectedHorse.Rank == 1;

        var status = new Label
        {
            Text = hit ? "的中！" : "はずれ...",
            Font = new Font("Yu Gothic UI", 34, FontStyle.Bold),
            ForeColor = hit ? UiTheme.Gold : Color.FromArgb(255, 165, 145),
            AutoSize = true,
            Location = new Point(28, 18),
            BackColor = Color.Transparent
        };

        var card = UiTheme.CreateCard(new Rectangle(25, 90, 930, 510));

        var payoutLabel = new Label
        {
            Text = $"払戻金: {payout:N0} 円",
            Font = new Font("Yu Gothic UI", 13, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(35, 35, 35),
            Location = new Point(24, 18),
            BackColor = Color.Transparent
        };

        var moneyLabel = new Label
        {
            Text = $"所持金: {session.Balance:N0} 円",
            Font = new Font("Yu Gothic UI", 13, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(35, 35, 35),
            Location = new Point(260, 18),
            BackColor = Color.Transparent
        };

        var ticketLabel = new Label
        {
            Text = $"購入: 馬番{selectedHorse.Number} {selectedHorse.Name} / {betAmount:N0}円 / オッズ {selectedHorse.Odds:F1}倍",
            Font = new Font("Yu Gothic UI", 11, FontStyle.Regular),
            AutoSize = true,
            ForeColor = Color.FromArgb(60, 60, 60),
            Location = new Point(24, 54),
            BackColor = Color.Transparent
        };

        var statsLabel = new Label
        {
            Text = $"累計: {_sessionStats(session)}",
            Font = new Font("Yu Gothic UI", 10, FontStyle.Regular),
            AutoSize = true,
            ForeColor = Color.FromArgb(80, 80, 80),
            Location = new Point(24, 76),
            BackColor = Color.Transparent
        };

        var rankGrid = new DataGridView
        {
            Location = new Point(24, 110),
            Size = new Size(880, 305),
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        UiTheme.StyleGrid(rankGrid);
        rankGrid.Columns.Add("Rank", "順位");
        rankGrid.Columns.Add("No", "馬番");
        rankGrid.Columns.Add("Name", "馬名");
        rankGrid.Columns.Add("Speed", "スピード");
        rankGrid.Columns.Add("Luck", "運の良さ");

        foreach (var horse in result.RankedHorses)
        {
            rankGrid.Rows.Add($"{horse.Rank}着", horse.Number, horse.Name, horse.Speed, horse.Luck);
        }

        var nextButton = UiTheme.PrimaryButton("次のレースへ", new Rectangle(600, 435, 180, 54));
        nextButton.Click += (_, _) =>
        {
            Action = ResultAction.NextRace;
            DialogResult = DialogResult.OK;
            Close();
        };

        var endButton = UiTheme.SecondaryButton("終了", new Rectangle(792, 435, 112, 54));
        endButton.Click += (_, _) =>
        {
            Action = ResultAction.EndGame;
            DialogResult = DialogResult.OK;
            Close();
        };

        card.Controls.Add(payoutLabel);
        card.Controls.Add(moneyLabel);
        card.Controls.Add(ticketLabel);
        card.Controls.Add(statsLabel);
        card.Controls.Add(rankGrid);
        card.Controls.Add(nextButton);
        card.Controls.Add(endButton);

        Controls.Add(status);
        Controls.Add(card);
    }

    /// <summary>セッション戦績の表示文字列を作成します。</summary>
    /// <param name="session">集計対象のゲームセッションです。</param>
    /// <returns>画面表示用の戦績文字列です。</returns>
    private static string _sessionStats(GameSession session)
    {
        return $"レース{session.TotalRaces:N0} / 的中{session.HitCount:N0} ({session.HitRate:P1}) / 総投資{session.TotalBets:N0}円 / 総払戻{session.TotalPayout:N0}円";
    }
}

