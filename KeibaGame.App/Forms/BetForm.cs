namespace KeibaGame.App;

internal sealed class BetForm : Form
{
    private readonly NumericUpDown _betAmount = new();

    /// <summary>入力された賭け金です。</summary>
    public int BetAmount => (int)_betAmount.Value;

    /// <summary>掛け金入力画面を初期化します。</summary>
    /// <param name="session">現在のゲームセッションです。</param>
    /// <param name="selectedHorse">選択中の馬です。</param>
    public BetForm(GameSession session, Horse selectedHorse)
    {
        Text = "掛け金入力画面";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(760, 500);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        UiTheme.ApplyForm(this);

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var card = UiTheme.CreateCard(new Rectangle(40, 40, 680, 360));

        var info = new Label
        {
            Text = $"馬番{selectedHorse.Number} {selectedHorse.Name}\nスピード: {selectedHorse.Speed} / 運の良さ: {selectedHorse.Luck}\nオッズ: {selectedHorse.Odds:F1}倍",
            Font = new Font("Yu Gothic UI", 14, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(30, 30, 30),
            Location = new Point(26, 26),
            BackColor = Color.Transparent
        };

        var money = new Label
        {
            Text = $"所持金: {session.Balance:N0} 円",
            Font = new Font("Yu Gothic UI", 13, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(55, 55, 55),
            Location = new Point(26, 140),
            BackColor = Color.Transparent
        };

        var inputLabel = new Label
        {
            Text = "掛け金 (100円単位)",
            Font = new Font("Yu Gothic UI", 12, FontStyle.Regular),
            AutoSize = true,
            ForeColor = Color.FromArgb(60, 60, 60),
            Location = new Point(28, 195),
            BackColor = Color.Transparent
        };

        _betAmount.Location = new Point(30, 228);
        _betAmount.Size = new Size(240, 42);
        _betAmount.Font = new Font("Yu Gothic UI", 16, FontStyle.Bold);
        _betAmount.Minimum = GameRules.MinBet;
        _betAmount.Maximum = Math.Max(GameRules.MinBet, session.Balance);
        _betAmount.Increment = GameRules.BetUnit;
        _betAmount.ThousandsSeparator = true;

        var oddsNote = new Label
        {
            Text = "払戻金 = 掛け金 × オッズ(切り捨て)",
            AutoSize = true,
            ForeColor = Color.FromArgb(90, 90, 90),
            Location = new Point(28, 288),
            BackColor = Color.Transparent
        };

        var confirm = UiTheme.PrimaryButton("確定", new Rectangle(530, 420, 140, 52));
        confirm.Click += (_, _) => Confirm(session.Balance);

        var back = UiTheme.SecondaryButton("戻る", new Rectangle(380, 420, 130, 52));
        back.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        card.Controls.Add(info);
        card.Controls.Add(money);
        card.Controls.Add(inputLabel);
        card.Controls.Add(_betAmount);
        card.Controls.Add(oddsNote);

        Controls.Add(card);
        Controls.Add(back);
        Controls.Add(confirm);
    }

    /// <summary>入力値を検証して賭け金を確定します。</summary>
    /// <param name="balance">現在の所持金です。</param>
    private void Confirm(int balance)
    {
        if (BetAmount < GameRules.MinBet || BetAmount % GameRules.BetUnit != 0)
        {
            MessageBox.Show(this, "掛け金は100円単位で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (BetAmount > balance)
        {
            MessageBox.Show(this, "所持金を超える金額は賭けられません。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}

