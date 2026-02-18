namespace KeibaGame.App;

internal sealed class HorseSelectForm : Form
{
    private readonly DataGridView _grid = new();
    private readonly List<Horse> _horses;

    /// <summary>選択された馬です。</summary>
    public Horse? SelectedHorse { get; private set; }

    /// <summary>馬選択画面を初期化します。</summary>
    /// <param name="session">現在のゲームセッションです。</param>
    /// <param name="horses">表示対象の出走馬一覧です。</param>
    public HorseSelectForm(GameSession session, List<Horse> horses)
    {
        _horses = horses;

        Text = "馬選択画面";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(960, 640);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        UiTheme.ApplyForm(this);

        Paint += (_, e) => UiTheme.PaintTurfBackground(e.Graphics, ClientRectangle);

        var header = new Label
        {
            Text = "出走馬を選択",
            Font = new Font("Yu Gothic UI", 24, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 18),
            BackColor = Color.Transparent
        };

        var money = new Label
        {
            Text = $"所持金: {session.Balance:N0} 円",
            Font = new Font("Yu Gothic UI", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(730, 24),
            BackColor = Color.Transparent
        };

        var card = UiTheme.CreateCard(new Rectangle(24, 70, 910, 500));

        _grid.Location = new Point(20, 20);
        _grid.Size = new Size(870, 400);
        _grid.ReadOnly = true;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        UiTheme.StyleGrid(_grid);

        _grid.Columns.Add("No", "馬番");
        _grid.Columns.Add("Name", "馬名");
        _grid.Columns.Add("Speed", "スピード");
        _grid.Columns.Add("Luck", "運の良さ");
        _grid.Columns.Add("Odds", "オッズ");

        foreach (var horse in _horses)
        {
            _grid.Rows.Add(horse.Number, horse.Name, horse.Speed, horse.Luck, $"{horse.Odds:F1}倍");
        }

        var hint = new Label
        {
            Text = "行を選択して[決定]を押してください",
            AutoSize = true,
            ForeColor = Color.FromArgb(70, 70, 70),
            Location = new Point(24, 435),
            BackColor = Color.Transparent
        };

        var decideButton = UiTheme.PrimaryButton("決定", new Rectangle(720, 430, 160, 50));
        decideButton.Click += (_, _) => Decide();

        card.Controls.Add(_grid);
        card.Controls.Add(hint);
        card.Controls.Add(decideButton);

        Controls.Add(header);
        Controls.Add(money);
        Controls.Add(card);
    }

    /// <summary>選択中の馬を確定して画面を閉じます。</summary>
    private void Decide()
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show(this, "馬を選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var no = Convert.ToInt32(_grid.SelectedRows[0].Cells[0].Value);
        SelectedHorse = _horses.First(h => h.Number == no);
        DialogResult = DialogResult.OK;
        Close();
    }
}

