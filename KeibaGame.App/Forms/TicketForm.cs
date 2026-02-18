namespace KeibaGame.App;

internal sealed class TicketForm : Form
{
    /// <summary>購入馬券表示画面を初期化します。</summary>
    /// <param name="session">現在のゲームセッションです。</param>
    /// <param name="horse">購入対象の馬です。</param>
    /// <param name="betAmount">購入金額です。</param>
    public TicketForm(GameSession session, Horse horse, int betAmount)
    {
        Text = "購入馬券表示画面";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(760, 500);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        UiTheme.ApplyForm(this);

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var card = UiTheme.CreateCard(new Rectangle(95, 55, 570, 300));

        var title = new Label
        {
            Text = "WIN TICKET",
            Font = new Font("Yu Gothic UI", 26, FontStyle.Bold),
            ForeColor = Color.FromArgb(35, 35, 35),
            AutoSize = true,
            Location = new Point(170, 22),
            BackColor = Color.Transparent
        };

        var ticket = new Label
        {
            Text = $"単勝 馬番{horse.Number} {horse.Name}\n購入金額: {betAmount:N0}円\nオッズ: {horse.Odds:F1}倍",
            Font = new Font("Yu Gothic UI", 18, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(40, 40, 40),
            Location = new Point(54, 92),
            BackColor = Color.Transparent
        };

        var money = new Label
        {
            Text = $"所持金: {session.Balance:N0} 円",
            Font = new Font("Yu Gothic UI", 12, FontStyle.Regular),
            AutoSize = true,
            ForeColor = Color.FromArgb(70, 70, 70),
            Location = new Point(56, 220),
            BackColor = Color.Transparent
        };

        var raceButton = UiTheme.PrimaryButton("いざレースへ！", new Rectangle(280, 390, 210, 66));
        raceButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        card.Controls.Add(title);
        card.Controls.Add(ticket);
        card.Controls.Add(money);

        Controls.Add(card);
        Controls.Add(raceButton);
    }
}

