using UnityEngine;

// Creates and manages the visual grid of tiles
public class GridBuilder : MonoBehaviour
{
    public float CellSize   { get; private set; } = 1.0f;
    public Vector3 GridOrigin { get; private set; }

    Sprite groundSprite;
    Sprite emptySprite;
    Sprite obstacleSprite;
    Sprite starSprite;
    Sprite characterSprite;

    GameObject starGO;
    GameObject[] tilePatch = new GameObject[0];
    GameObject[] obstacleGOs = new GameObject[0];

    public void Initialize()
    {
        groundSprite    = SpriteFactory.CreateGroundSprite();
        emptySprite     = SpriteFactory.CreateEmptyTileSprite();
        obstacleSprite  = SpriteFactory.CreateObstacleSprite();
        starSprite      = SpriteFactory.CreateStarSprite();
        characterSprite = SpriteFactory.CreateCharacterSprite();
    }

    public void BuildGrid(GameManager.LevelData lvl)
    {
        // Clear previous objects
        foreach (var go in tilePatch)     if (go) Destroy(go);
        foreach (var go in obstacleGOs)   if (go) Destroy(go);
        if (starGO) Destroy(starGO);

        int cols = lvl.cols, rows = lvl.rows;

        // Grid centered at (0, 0.8) to sit in upper screen area
        GridOrigin = new Vector3(
            -(cols - 1) * CellSize * 0.5f,
             (rows - 1) * CellSize * 0.5f + 0.8f,
             0f);

        // Build tiles
        tilePatch = new GameObject[cols * rows];
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < cols; col++)
        {
            bool isStart = new Vector2Int(col, row) == lvl.playerStart;
            bool isStar  = new Vector2Int(col, row) == lvl.starPos;
            Sprite spr   = (isStart || isStar) ? groundSprite : emptySprite;

            var go = MakeTile(col, row, spr, "Tile", -0.1f);
            tilePatch[row * cols + col] = go;
        }

        // Build obstacles
        obstacleGOs = new GameObject[lvl.obstacles.Length];
        for (int i = 0; i < lvl.obstacles.Length; i++)
        {
            var pos = lvl.obstacles[i];
            obstacleGOs[i] = MakeTile(pos.x, pos.y, obstacleSprite, "Obstacle", 0f);
        }

        // Star goal
        starGO = MakeObject("Star", lvl.starPos, starSprite, 0f, new Vector3(0.7f, 0.7f, 1f));
        StarIdleAnim(starGO);
    }

    GameObject MakeTile(int col, int row, Sprite spr, string namePrefix, float z)
    {
        var go = new GameObject($"{namePrefix}_{col}_{row}");
        go.transform.SetParent(transform);
        go.transform.position = CellPos(col, row, z);
        go.transform.localScale = Vector3.one;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        return go;
    }

    GameObject MakeObject(string name, Vector2Int pos, Sprite spr, float z, Vector3 scale)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.position  = CellPos(pos.x, pos.y, z);
        go.transform.localScale = scale;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite      = spr;
        sr.sortingOrder = 2;
        return go;
    }

    void StarIdleAnim(GameObject go)
    {
        if (go == null) return;
        go.AddComponent<StarBobber>();
    }

    Vector3 CellPos(int col, int row, float z) =>
        GridOrigin + new Vector3(col * CellSize, -row * CellSize, z);

    public Vector3 CellWorldPos(Vector2Int pos, float z = 0f) =>
        GridOrigin + new Vector3(pos.x * CellSize, -pos.y * CellSize, z);
}

// Simple idle bob animation for the star
public class StarBobber : MonoBehaviour
{
    Vector3 basePos;
    void Start() => basePos = transform.position;
    void Update() =>
        transform.position = basePos + new Vector3(0, Mathf.Sin(Time.time * 2f) * 0.06f, 0);
}
