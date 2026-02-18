using System.Drawing.Drawing2D;

namespace KeibaGame.App;

internal static class HorseImageFactory
{
    /// <summary>固定5頭分の馬スプライトを生成します。</summary>
    /// <returns>馬スプライト配列です。</returns>
    public static Image[] CreateFixedSet()
    {
        var bodyPalettes = new[]
        {
            (Color.FromArgb(106, 67, 48), Color.FromArgb(164, 116, 83)),
            (Color.FromArgb(72, 72, 83), Color.FromArgb(126, 126, 142)),
            (Color.FromArgb(85, 66, 52), Color.FromArgb(144, 110, 84)),
            (Color.FromArgb(55, 55, 55), Color.FromArgb(116, 116, 116)),
            (Color.FromArgb(98, 58, 37), Color.FromArgb(158, 102, 68))
        };

        var jockeyPalettes = new[]
        {
            (Color.FromArgb(205, 33, 55), Color.White),
            (Color.FromArgb(20, 90, 180), Color.FromArgb(255, 224, 63)),
            (Color.FromArgb(27, 121, 63), Color.White),
            (Color.FromArgb(120, 43, 149), Color.FromArgb(245, 213, 87)),
            (Color.FromArgb(35, 35, 35), Color.FromArgb(247, 247, 247))
        };

        return bodyPalettes
            .Select((body, i) => CreateSingle(body.Item1, body.Item2, jockeyPalettes[i].Item1, jockeyPalettes[i].Item2, i + 1))
            .ToArray();
    }

    /// <summary>1頭分の馬スプライトを生成します。</summary>
    /// <param name="bodyDark">馬体の濃色です。</param>
    /// <param name="bodyLight">馬体の明色です。</param>
    /// <param name="jockeyMain">騎手服の主色です。</param>
    /// <param name="jockeyAccent">騎手服の差し色です。</param>
    /// <param name="number">ゼッケン番号です。</param>
    /// <returns>生成された馬スプライト画像です。</returns>
    private static Image CreateSingle(Color bodyDark, Color bodyLight, Color jockeyMain, Color jockeyAccent, int number)
    {
        var bmp = new Bitmap(120, 72);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.Clear(Color.Transparent);

        using var shadowBrush = new SolidBrush(Color.FromArgb(65, 0, 0, 0));
        g.FillEllipse(shadowBrush, 24, 56, 72, 10);

        using (var bodyBrush = new LinearGradientBrush(new Rectangle(18, 26, 66, 24), bodyLight, bodyDark, 120f))
        using (var maneBrush = new SolidBrush(Color.FromArgb(34, 24, 20)))
        using (var tailBrush = new LinearGradientBrush(new Rectangle(10, 30, 20, 18), bodyDark, Color.FromArgb(45, 30, 24), 45f))
        using (var legBrush = new SolidBrush(bodyDark))
        {
            g.FillEllipse(bodyBrush, 24, 24, 62, 24);
            g.FillEllipse(bodyBrush, 77, 22, 25, 17);

            var neck = new GraphicsPath();
            neck.AddPolygon(
            [
                new PointF(72, 26),
                new PointF(82, 18),
                new PointF(92, 22),
                new PointF(82, 34)
            ]);
            g.FillPath(bodyBrush, neck);

            var mane = new GraphicsPath();
            mane.AddCurve(
            [
                new PointF(78, 23),
                new PointF(74, 19),
                new PointF(66, 20),
                new PointF(63, 24),
                new PointF(70, 27)
            ]);
            g.FillPath(maneBrush, mane);

            var tail = new GraphicsPath();
            tail.AddBezier(new PointF(24, 33), new PointF(9, 30), new PointF(8, 48), new PointF(22, 45));
            tail.AddBezier(new PointF(22, 45), new PointF(10, 52), new PointF(13, 55), new PointF(26, 49));
            g.FillPath(tailBrush, tail);

            g.FillRectangle(legBrush, 38, 44, 8, 16);
            g.FillRectangle(legBrush, 53, 46, 8, 14);
            g.FillRectangle(legBrush, 66, 44, 8, 16);
            g.FillRectangle(legBrush, 80, 45, 8, 15);
        }

        using (var saddleBrush = new SolidBrush(Color.FromArgb(30, 40, 68)))
        using (var saddleAccent = new SolidBrush(Color.FromArgb(226, 190, 80)))
        {
            g.FillRectangle(saddleBrush, 52, 26, 20, 12);
            g.FillRectangle(saddleAccent, 52, 26, 20, 3);
        }

        using (var jerseyBrush = new SolidBrush(jockeyMain))
        using (var helmetBrush = new SolidBrush(jockeyMain))
        using (var accentBrush = new SolidBrush(jockeyAccent))
        {
            g.FillEllipse(helmetBrush, 56, 12, 10, 10);
            g.FillRectangle(jerseyBrush, 54, 19, 14, 10);
            g.FillRectangle(accentBrush, 54, 23, 14, 3);
            g.FillRectangle(accentBrush, 59, 19, 4, 10);
        }

        using (var eyeBrush = new SolidBrush(Color.Black))
        using (var noseBrush = new SolidBrush(Color.FromArgb(55, 40, 35)))
        {
            g.FillEllipse(eyeBrush, 94, 28, 2, 2);
            g.FillEllipse(noseBrush, 99, 33, 3, 2);
        }

        using (var numberBrush = new SolidBrush(Color.White))
        using (var numberBg = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
        using (var numberFont = new Font("Yu Gothic UI", 9, FontStyle.Bold))
        {
            g.FillEllipse(numberBg, 6, 8, 20, 20);
            g.DrawString(number.ToString(), numberFont, numberBrush, 10, 10);
        }

        return bmp;
    }
}

