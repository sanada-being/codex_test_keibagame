using System.Drawing.Drawing2D;

namespace KeibaGame.App;

internal static class UiTheme
{
    /// <summary>芝を表現する基本色です。</summary>
    public static readonly Color Turf = Color.FromArgb(34, 88, 50);
    /// <summary>芝背景の濃色です。</summary>
    public static readonly Color TurfDark = Color.FromArgb(20, 56, 32);
    /// <summary>強調に使う金色です。</summary>
    public static readonly Color Gold = Color.FromArgb(236, 198, 94);
    /// <summary>ボードヘッダー色です。</summary>
    public static readonly Color Board = Color.FromArgb(21, 34, 24);
    /// <summary>カード背景色です。</summary>
    public static readonly Color Card = Color.FromArgb(248, 245, 235);

    /// <summary>フォームへ共通テーマを適用します。</summary>
    /// <param name="form">適用対象のフォームです。</param>
    public static void ApplyForm(Form form)
    {
        form.BackColor = TurfDark;
        form.ForeColor = Color.White;
        form.Font = new Font("Yu Gothic UI", 10F, FontStyle.Regular);
    }

    /// <summary>主操作用ボタンを生成します。</summary>
    /// <param name="text">ボタン表示テキストです。</param>
    /// <param name="bounds">ボタンの配置領域です。</param>
    /// <returns>テーマ適用済みのボタンです。</returns>
    public static Button PrimaryButton(string text, Rectangle bounds)
    {
        var button = new Button
        {
            Text = text,
            Bounds = bounds,
            BackColor = Gold,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Yu Gothic UI", 12, FontStyle.Bold)
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(120, 95, 40);
        button.FlatAppearance.BorderSize = 1;
        return button;
    }

    /// <summary>副操作用ボタンを生成します。</summary>
    /// <param name="text">ボタン表示テキストです。</param>
    /// <param name="bounds">ボタンの配置領域です。</param>
    /// <returns>テーマ適用済みのボタンです。</returns>
    public static Button SecondaryButton(string text, Rectangle bounds)
    {
        var button = new Button
        {
            Text = text,
            Bounds = bounds,
            BackColor = Color.FromArgb(231, 229, 220),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Yu Gothic UI", 11, FontStyle.Regular)
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(110, 110, 110);
        button.FlatAppearance.BorderSize = 1;
        return button;
    }

    /// <summary>カード表示用パネルを生成します。</summary>
    /// <param name="bounds">パネルの配置領域です。</param>
    /// <returns>カード表示用パネルです。</returns>
    public static Panel CreateCard(Rectangle bounds)
    {
        return new Panel
        {
            Bounds = bounds,
            BackColor = Card,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    /// <summary>グリッド表示の共通スタイルを適用します。</summary>
    /// <param name="grid">スタイル適用対象のグリッドです。</param>
    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.GridColor = Color.FromArgb(210, 210, 210);
        grid.DefaultCellStyle.BackColor = Color.White;
        grid.DefaultCellStyle.ForeColor = Color.Black;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 242, 196);
        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Board;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Yu Gothic UI", 10, FontStyle.Bold);
        grid.EnableHeadersVisualStyles = false;
        grid.RowTemplate.Height = 34;
    }

    /// <summary>芝風の背景を描画します。</summary>
    /// <param name="g">描画先のグラフィックスです。</param>
    /// <param name="rect">描画領域です。</param>
    public static void PaintTurfBackground(Graphics g, Rectangle rect)
    {
        using var brush = new LinearGradientBrush(rect, Turf, TurfDark, 90f);
        g.FillRectangle(brush, rect);

        using var stripePen = new Pen(Color.FromArgb(25, 255, 255, 255), 2f);
        for (var y = rect.Top; y < rect.Bottom; y += 28)
        {
            g.DrawLine(stripePen, rect.Left, y, rect.Right, y);
        }
    }
}
