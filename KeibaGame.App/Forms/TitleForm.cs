namespace KeibaGame.App;

internal sealed class TitleForm : Form
{
    private readonly GameSession _session = new();
    private readonly Label _saveStatusLabel;
    private readonly Label _statsLabel;
    private readonly Label _speedLabel;
    private readonly Button _continueButton;

    /// <summary>タイトル画面を初期化します。</summary>
    public TitleForm()
    {
        Text = "競馬ゲーム";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(960, 640);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        UiTheme.ApplyForm(this);

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var board = UiTheme.CreateCard(new Rectangle(140, 84, 680, 460));

        var title = new Label
        {
            Text = "DERBY CHALLENGE",
            Font = new Font("Yu Gothic UI", 38, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(22, 40, 28),
            Location = new Point(110, 52),
            BackColor = Color.Transparent
        };

        var subtitle = new Label
        {
            Text = "単勝で資金を増やし、5万円到達を目指せ",
            Font = new Font("Yu Gothic UI", 14, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(54, 54, 54),
            Location = new Point(170, 132),
            BackColor = Color.Transparent
        };

        _saveStatusLabel = new Label
        {
            Font = new Font("Yu Gothic UI", 11, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(70, 70, 70),
            Location = new Point(56, 184),
            BackColor = Color.Transparent
        };

        _statsLabel = new Label
        {
            Font = new Font("Yu Gothic UI", 11, FontStyle.Regular),
            Size = new Size(560, 110),
            ForeColor = Color.FromArgb(80, 80, 80),
            Location = new Point(56, 214),
            BackColor = Color.Transparent
        };

        _speedLabel = new Label
        {
            Font = new Font("Yu Gothic UI", 11, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(60, 60, 60),
            Location = new Point(56, 324),
            BackColor = Color.Transparent
        };

        var settingsButton = UiTheme.SecondaryButton("設定", new Rectangle(44, 352, 86, 64));
        settingsButton.Click += (_, _) => OpenSettings();

        var newGameButton = UiTheme.PrimaryButton("新しく始める", new Rectangle(142, 352, 210, 64));
        newGameButton.Click += (_, _) => LaunchGame(startNew: true);

        _continueButton = UiTheme.SecondaryButton("続きから", new Rectangle(364, 352, 158, 64));
        _continueButton.Click += (_, _) => LaunchGame(startNew: false);

        var quitButton = UiTheme.SecondaryButton("終了", new Rectangle(534, 352, 100, 64));
        quitButton.Click += (_, _) => Close();

        board.Controls.Add(title);
        board.Controls.Add(subtitle);
        board.Controls.Add(_saveStatusLabel);
        board.Controls.Add(_statsLabel);
        board.Controls.Add(_speedLabel);
        board.Controls.Add(settingsButton);
        board.Controls.Add(newGameButton);
        board.Controls.Add(_continueButton);
        board.Controls.Add(quitButton);

        Controls.Add(board);

        RefreshSessionState();
    }

    /// <summary>設定画面を開き、変更があれば適用します。</summary>
    private void OpenSettings()
    {
        using var settingsForm = new SettingsForm(_session.RaceSpeedMultiplier);
        if (settingsForm.ShowDialog(this) == DialogResult.OK)
        {
            _session.RaceSpeedMultiplier = GameRules.NormalizeRaceSpeed(settingsForm.SelectedSpeedMultiplier);
            if (GameSaveService.Exists())
            {
                GameSaveService.Save(_session);
            }

            RefreshSessionState();
        }
    }

    /// <summary>ゲーム本編を開始します。</summary>
    /// <param name="startNew"><c>true</c> の場合は新規開始します。</param>
    private void LaunchGame(bool startNew)
    {
        var outcome = GameFlow.Run(this, _session, startNew);
        RefreshSessionState();

        if (outcome == FlowOutcome.ExitApplication)
        {
            Close();
        }
    }

    /// <summary>セーブデータ状態と表示内容を更新します。</summary>
    private void RefreshSessionState()
    {
        var currentSpeed = _session.RaceSpeedMultiplier;
        var hasSave = GameSaveService.TryLoad(out var loaded);
        if (hasSave)
        {
            _session.ApplyFrom(loaded);
            _saveStatusLabel.Text = $"セーブデータあり: 所持金 {_session.Balance:N0}円";
        }
        else
        {
            _session.Reset();
            _session.RaceSpeedMultiplier = GameRules.NormalizeRaceSpeed(currentSpeed);
            _saveStatusLabel.Text = "セーブデータなし: 新しく始めてください";
        }

        _continueButton.Enabled = hasSave;
        _statsLabel.Text =
            $"累計レース数: {_session.TotalRaces:N0}\n" +
            $"的中数: {_session.HitCount:N0}  的中率: {_session.HitRate:P1}\n" +
            $"総投資: {_session.TotalBets:N0}円  総払戻: {_session.TotalPayout:N0}円";
        _speedLabel.Text = $"現在のレース速度設定: {GameRules.ToSpeedLabel(_session.RaceSpeedMultiplier)}";
    }
}

