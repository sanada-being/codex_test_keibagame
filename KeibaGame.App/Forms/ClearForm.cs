namespace KeibaGame.App;

internal sealed class ClearForm : Form
{
    /// <summary>クリア画面を初期化します。</summary>
    public ClearForm()
    {
        Text = "クリア";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(760, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        UiTheme.ApplyForm(this);

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var card = UiTheme.CreateCard(new Rectangle(85, 60, 590, 240));

        var label = new Label
        {
            Text = "あなたはもう競馬マスターです！！",
            Font = new Font("Yu Gothic UI", 26, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(26, 47, 33),
            Location = new Point(34, 72),
            BackColor = Color.Transparent
        };

        var button = UiTheme.PrimaryButton("タイトルへ", new Rectangle(285, 330, 180, 54));
        button.Click += (_, _) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        card.Controls.Add(label);

        Controls.Add(card);
        Controls.Add(button);
    }
}

