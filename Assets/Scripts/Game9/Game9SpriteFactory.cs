using UnityEngine;

public static class Game9SpriteFactory
{
    static readonly Color[] BallColors =
    {
        new Color(0.95f, 0.35f, 0.35f), // red
        new Color(1.00f, 0.65f, 0.15f), // orange
        new Color(0.25f, 0.82f, 0.30f), // green
        new Color(0.30f, 0.55f, 1.00f), // blue
        new Color(0.85f, 0.25f, 0.90f), // purple
        new Color(0.15f, 0.88f, 0.90f), // cyan
        new Color(1.00f, 0.88f, 0.15f), // yellow
        new Color(1.00f, 0.45f, 0.75f), // pink
        new Color(0.40f, 0.88f, 0.55f), // mint
        new Color(0.80f, 0.50f, 0.20f), // brown
    };

    // ── Background ────────────────────────────────────────────────────────────
    public static Sprite CreateBackground()
    {
        int w = 4, h = 8;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color skyTop    = new Color(0.53f, 0.81f, 0.98f);
        Color skyBottom = new Color(0.78f, 0.93f, 1.00f);
        Color ground    = new Color(0.35f, 0.72f, 0.33f);

        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            if (y < 2)
                tex.SetPixel(x, y, ground);
            else
            {
                float t = (float)(y - 2) / (h - 2);
                tex.SetPixel(x, y, Color.Lerp(skyBottom, skyTop, t));
            }
        }
        tex.Apply();
        // pixelsPerUnit=1 → 4×8 world units; we scale it up in Bootstrap
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 1f);
    }

    // ── Basket ─────────────────────────────────────────────────────────────────
    public static Sprite CreateBasket()
    {
        int w = 128, h = 80;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color clear = new Color(0, 0, 0, 0);
        Fill(tex, w, h, clear);

        Color brown  = new Color(0.72f, 0.46f, 0.18f);
        Color dark   = new Color(0.50f, 0.28f, 0.06f);
        Color rim    = new Color(0.90f, 0.68f, 0.32f);

        int topY    = h - 10;
        int botY    = 4;
        int topHalf = 58;
        int botHalf = 38;
        int cx      = w / 2;

        for (int y = botY; y <= topY; y++)
        {
            float t    = (float)(y - botY) / (topY - botY);
            int   half = Mathf.RoundToInt(Mathf.Lerp(botHalf, topHalf, t));
            for (int x = cx - half; x <= cx + half; x++)
            {
                if (x < 0 || x >= w) continue;
                bool weave = ((x / 8) + (y / 5)) % 2 == 0;
                tex.SetPixel(x, y, weave ? brown : dark);
            }
        }

        // Top rim
        for (int x = cx - topHalf - 2; x <= cx + topHalf + 2; x++)
        {
            if (x < 0 || x >= w) continue;
            for (int y = topY - 2; y <= topY + 6; y++)
            {
                if (y < 0 || y >= h) continue;
                tex.SetPixel(x, y, rim);
            }
        }

        tex.Apply();
        // 128/32 = 4 world units wide, 80/32 = 2.5 world units tall
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
    }

    // ── Falling number ball ───────────────────────────────────────────────────
    public static Sprite CreateNumberBall(int value)
    {
        int   size  = 96;
        Color ball  = BallColors[value % BallColors.Length];
        Color edge  = new Color(ball.r * 0.55f, ball.g * 0.55f, ball.b * 0.55f);
        Color shine = new Color(Mathf.Min(1f, ball.r + 0.35f),
                                Mathf.Min(1f, ball.g + 0.35f),
                                Mathf.Min(1f, ball.b + 0.35f));
        Color clear = new Color(0, 0, 0, 0);

        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        float cx = size / 2f - 0.5f;
        float cy = size / 2f - 0.5f;
        float r  = size / 2f - 2f;

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx   = x - cx, dy = y - cy;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            if      (dist > r)        tex.SetPixel(x, y, clear);
            else if (dist > r - 4f)   tex.SetPixel(x, y, edge);
            else
            {
                float gradT = Mathf.Clamp01((dx + dy) / (r * 1.8f) + 0.5f);
                tex.SetPixel(x, y, Color.Lerp(shine, ball, gradT));
            }
        }

        // Shine highlight
        DrawCircle(tex, size / 2 - 14, size / 2 + 14, 9, new Color(1f, 1f, 1f, 0.65f));

        tex.Apply();
        // 96/96 = 1 world unit diameter
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), (float)size);
    }

    // ── Star-burst happy effect ───────────────────────────────────────────────
    public static Sprite CreateStarBurst()
    {
        int   size  = 96;
        Color clear = new Color(0, 0, 0, 0);
        Color gold  = new Color(1f, 0.88f, 0.08f);

        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Fill(tex, size, size, clear);

        float cx     = size / 2f;
        float cy     = size / 2f;
        float outerR = size / 2f - 2f;
        float innerR = outerR * 0.38f;

        // 8-point star
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx   = x - cx, dy = y - cy;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            if (dist > outerR) continue;
            float ang  = Mathf.Atan2(dy, dx);
            float frac = (ang / (2f * Mathf.PI) * 8f) % 1f;
            float starR = Mathf.Lerp(innerR, outerR, Mathf.Abs(frac - 0.5f) * 2f);
            if (dist <= starR)
                tex.SetPixel(x, y, gold);
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), (float)size);
    }

    // ── Rounded rectangle (buttons / panels) ─────────────────────────────────
    public static Sprite CreateRoundRect(int w, int h, int radius, Color fill, Color border)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            float closestX = Mathf.Clamp(x, radius, w - radius);
            float closestY = Mathf.Clamp(y, radius, h - radius);
            float d = Mathf.Sqrt((x - closestX) * (x - closestX) + (y - closestY) * (y - closestY));
            if      (d > radius)      tex.SetPixel(x, y, clear);
            else if (d > radius - 4)  tex.SetPixel(x, y, border);
            else                      tex.SetPixel(x, y, fill);
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), Mathf.Min(w, h) / 2f);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color c)
    {
        for (int y = cy - r; y <= cy + r; y++)
        for (int x = cx - r; x <= cx + r; x++)
        {
            if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
            if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r)
                tex.SetPixel(x, y, c);
        }
    }

    static void Fill(Texture2D tex, int w, int h, Color c)
    {
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++) tex.SetPixel(x, y, c);
    }
}
