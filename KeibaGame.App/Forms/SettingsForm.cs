namespace KeibaGame.App;

internal sealed class SettingsForm : Form
{
    private readonly ComboBox _speedBox = new();

    /// <summary>選択されたレース速度倍率です。</summary>
    public double SelectedSpeedMultiplier { get; private set; }

    /// <summary>設定画面を初期化します。</summary>
    /// <param name="currentSpeedMultiplier">現在設定されているレース速度倍率です。</param>
    public SettingsForm(double currentSpeedMultiplier)
    {
        Text = "設定";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(480, 280);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        UiTheme.ApplyForm(this);

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var card = UiTheme.CreateCard(new Rectangle(30, 24, 420, 180));

        var title = new Label
        {
            Text = "レース速度設定",
            Font = new Font("Yu Gothic UI", 16, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(35, 35, 35),
            Location = new Point(26, 24),
            BackColor = Color.Transparent
        };

        var desc = new Label
        {
            Text = "この設定は次のレースから反映されます。",
            Font = new Font("Yu Gothic UI", 10, FontStyle.Regular),
            AutoSize = true,
            ForeColor = Color.FromArgb(80, 80, 80),
            Location = new Point(28, 58),
            BackColor = Color.Transparent
        };

        _speedBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _speedBox.Font = new Font("Yu Gothic UI", 14, FontStyle.Bold);
        _speedBox.Location = new Point(30, 95);
        _speedBox.Width = 140;
        _speedBox.Items.AddRange(GameRules.RaceSpeedOptions.Select(GameRules.ToSpeedLabel).Cast<object>().ToArray());
        _speedBox.SelectedItem = GameRules.ToSpeedLabel(currentSpeedMultiplier);

        var cancelButton = UiTheme.SecondaryButton("キャンセル", new Rectangle(240, 220, 100, 42));
        cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        var okButton = UiTheme.PrimaryButton("保存", new Rectangle(350, 220, 100, 42));
        okButton.Click += (_, _) =>
        {
            SelectedSpeedMultiplier = GameRules.ParseSpeedLabel(_speedBox.SelectedItem?.ToString());
            DialogResult = DialogResult.OK;
            Close();
        };

        card.Controls.Add(title);
        card.Controls.Add(desc);
        card.Controls.Add(_speedBox);

        Controls.Add(card);
        Controls.Add(cancelButton);
        Controls.Add(okButton);
    }
}

