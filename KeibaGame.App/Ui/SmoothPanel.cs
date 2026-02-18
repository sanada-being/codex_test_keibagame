namespace KeibaGame.App;

internal sealed class SmoothPanel : Panel
{
    /// <summary>再描画時のちらつきを抑えるダブルバッファパネルを生成します。</summary>
    public SmoothPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        UpdateStyles();
    }
}
