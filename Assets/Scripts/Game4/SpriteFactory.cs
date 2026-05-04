using UnityEngine;

public static class SpriteFactory
{
    // ── Ground tile ──────────────────────────────────────────────────────────
    public static Sprite CreateGroundSprite()
    {
        int size = 64;
        var tex = NewTex(size);
        Color bg     = new Color(0.56f, 0.84f, 0.40f);
        Color border = new Color(0.38f, 0.65f, 0.22f);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            bool edge = x < 2 || x >= size - 2 || y < 2 || y >= size - 2;
            tex.SetPixel(x, y, edge ? border : bg);
        }
        return Finish(tex, size);
    }

    // ── Empty / dark tile ─────────────────────────────────────────────────
    public static Sprite CreateEmptyTileSprite()
    {
        int size = 64;
        var tex = NewTex(size);
        Color bg     = new Color(0.78f, 0.90f, 0.96f);
        Color border = new Color(0.60f, 0.75f, 0.85f);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            bool edge = x < 2 || x >= size - 2 || y < 2 || y >= size - 2;
            tex.SetPixel(x, y, edge ? border : bg);
        }
        return Finish(tex, size);
    }

    // ── Obstacle tile ────────────────────────────────────────────────────────
    public static Sprite CreateObstacleSprite()
    {
        int size = 64;
        var tex = NewTex(size);
        Color bg     = new Color(0.40f, 0.22f, 0.12f);
        Color xColor = new Color(0.80f, 0.15f, 0.10f);
        Color border = new Color(0.25f, 0.12f, 0.04f);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            bool edge = x < 2 || x >= size - 2 || y < 2 || y >= size - 2;
            bool isX  = Mathf.Abs(x - y) < 5 || Mathf.Abs(x - (size - 1 - y)) < 5;
            tex.SetPixel(x, y, edge ? border : (isX ? xColor : bg));
        }
        return Finish(tex, size);
    }

    // ── Character (cute smiley face) ─────────────────────────────────────────
    public static Sprite CreateCharacterSprite()
    {
        int size = 64;
        int half = size / 2;
        Color skin   = new Color(1.00f, 0.85f, 0.28f);
        Color outline= new Color(0.85f, 0.60f, 0.05f);
        Color eyeW   = Color.white;
        Color pupil  = new Color(0.10f, 0.10f, 0.30f);
        Color smile  = new Color(0.85f, 0.18f, 0.18f);
        Color clear  = new Color(0, 0, 0, 0);

        var tex = NewTex(size);
        Fill(tex, size, clear);

        float cx = half - 0.5f, cy = half - 0.5f;
        float r = half - 2f;

        // Body
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - cx, dy = y - cy;
            float d = dx * dx + dy * dy;
            if      (d <= (r - 2f) * (r - 2f))  tex.SetPixel(x, y, skin);
            else if (d <= r * r)                  tex.SetPixel(x, y, outline);
        }

        // Eyes
        DrawCircle(tex, 20, 40, 6, eyeW);
        DrawCircle(tex, 20, 40, 3, pupil);
        DrawCircle(tex, 44, 40, 6, eyeW);
        DrawCircle(tex, 44, 40, 3, pupil);

        // Smile arc
        for (float a = 200f; a <= 340f; a += 2f)
        {
            float rad = a * Mathf.Deg2Rad;
            int sx = Mathf.RoundToInt(half + Mathf.Cos(rad) * 13);
            int sy = Mathf.RoundToInt(half + Mathf.Sin(rad) * 13);
            for (int dy = -2; dy <= 2; dy++)
            for (int dx = -2; dx <= 2; dx++)
            {
                int px = sx + dx, py = sy + dy;
                if (InBounds(px, py, size)) tex.SetPixel(px, py, smile);
            }
        }

        return Finish(tex, size);
    }

    // ── Star / goal ───────────────────────────────────────────────────────────
    public static Sprite CreateStarSprite()
    {
        int size = 64;
        int half = size / 2;
        Color gold     = new Color(1.00f, 0.86f, 0.10f);
        Color goldEdge = new Color(1.00f, 0.65f, 0.00f);
        Color shine    = new Color(1.00f, 1.00f, 0.70f);
        Color clear    = new Color(0, 0, 0, 0);

        var tex = NewTex(size);
        Fill(tex, size, clear);

        float cx = half - 0.5f, cy = half - 0.5f;
        float outerR = half - 3f;
        float innerR = outerR * 0.38f;

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float px = x - cx, py = y - cy;
            if (IsInStar(px, py, outerR, innerR))
                tex.SetPixel(x, y, gold);
        }

        // Edge
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            if (tex.GetPixel(x, y).a < 0.5f) continue;
            bool edgePx = false;
            for (int dy = -1; dy <= 1 && !edgePx; dy++)
            for (int dx = -1; dx <= 1 && !edgePx; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if (!InBounds(nx, ny, size) || tex.GetPixel(nx, ny).a < 0.5f) edgePx = true;
            }
            if (edgePx) tex.SetPixel(x, y, goldEdge);
        }

        // Shine dot
        DrawCircle(tex, half - 8, half + 8, 4, shine);

        return Finish(tex, size);
    }

    // ── Direction arrow icon ─────────────────────────────────────────────────
    public static Sprite CreateArrowSprite(Direction dir)
    {
        int size = 48;
        Color[] colors = {
            new Color(0.20f, 0.72f, 0.30f), // Up   – green
            new Color(0.20f, 0.40f, 0.92f), // Down – blue
            new Color(0.95f, 0.58f, 0.10f), // Left – orange
            new Color(0.92f, 0.20f, 0.52f)  // Right– pink
        };
        Color arrowColor = colors[(int)dir];
        Color bg = new Color(1, 1, 1, 0.9f);

        var tex = NewTex(size);
        Fill(tex, size, bg);
        DrawArrow(tex, size, dir, arrowColor);
        return Finish(tex, size);
    }

    // ── Solid color rounded-rect (for buttons / panels) ──────────────────────
    public static Sprite CreateRoundRect(int w, int h, int radius, Color fill, Color border)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            float cx = Mathf.Clamp(x, radius, w - radius);
            float cy = Mathf.Clamp(y, radius, h - radius);
            float d  = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
            if      (d > radius)           tex.SetPixel(x, y, clear);
            else if (d > radius - 3)       tex.SetPixel(x, y, border);
            else                           tex.SetPixel(x, y, fill);
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), Mathf.Min(w, h) / 2f);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color c)
    {
        for (int y = cy - r; y <= cy + r; y++)
        for (int x = cx - r; x <= cx + r; x++)
        {
            if (!InBounds(x, y, tex.width)) continue;
            if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r)
                tex.SetPixel(x, y, c);
        }
    }

    static void DrawArrow(Texture2D tex, int size, Direction dir, Color c)
    {
        int cx = size / 2, cy = size / 2, arm = size / 3;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            int dx = x - cx, dy = y - cy;
            bool draw = false;
            switch (dir)
            {
                case Direction.Up:
                    draw = (dy >= 0 && Mathf.Abs(dx) <= arm - dy)
                        || (dy < 0 && Mathf.Abs(dx) < 5);
                    break;
                case Direction.Down:
                    draw = (dy <= 0 && Mathf.Abs(dx) <= arm + dy)
                        || (dy > 0 && Mathf.Abs(dx) < 5);
                    break;
                case Direction.Right:
                    draw = (dx >= 0 && Mathf.Abs(dy) <= arm - dx)
                        || (dx < 0 && Mathf.Abs(dy) < 5);
                    break;
                case Direction.Left:
                    draw = (dx <= 0 && Mathf.Abs(dy) <= arm + dx)
                        || (dx > 0 && Mathf.Abs(dy) < 5);
                    break;
            }
            if (draw) tex.SetPixel(x, y, c);
        }
    }

    static bool IsInStar(float px, float py, float outerR, float innerR)
    {
        Vector2[] verts = new Vector2[10];
        for (int i = 0; i < 10; i++)
        {
            float angle = (90f + i * 36f) * Mathf.Deg2Rad;
            float rad   = (i % 2 == 0) ? outerR : innerR;
            verts[i] = new Vector2(Mathf.Cos(angle) * rad, Mathf.Sin(angle) * rad);
        }
        return PointInPolygon(new Vector2(px, py), verts);
    }

    static bool PointInPolygon(Vector2 p, Vector2[] poly)
    {
        int n = poly.Length; bool inside = false;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            if (((poly[i].y > p.y) != (poly[j].y > p.y)) &&
                p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
                inside = !inside;
        }
        return inside;
    }

    static bool InBounds(int x, int y, int size) => x >= 0 && x < size && y >= 0 && y < size;
    static void Fill(Texture2D tex, int size, Color c)
    {
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++) tex.SetPixel(x, y, c);
    }
    static Texture2D NewTex(int size)
    {
        var t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        t.filterMode = FilterMode.Bilinear;
        return t;
    }
    static Sprite Finish(Texture2D tex, int size)
    {
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
