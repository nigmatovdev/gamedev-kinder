using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // ── Level Data ───────────────────────────────────────────────────────────
    public struct LevelData
    {
        public int cols, rows;
        public Vector2Int playerStart;
        public Vector2Int starPos;
        public Vector2Int[] obstacles;
        public string hint;
    }

    static readonly LevelData[] Levels = new LevelData[]
    {
        // Level 1 – straight path
        new LevelData {
            cols = 5, rows = 5,
            playerStart = new Vector2Int(0, 2),
            starPos     = new Vector2Int(4, 2),
            obstacles   = new Vector2Int[0],
            hint        = "Right × 4!"
        },
        // Level 2 – L-shaped path
        new LevelData {
            cols = 5, rows = 5,
            playerStart = new Vector2Int(0, 4),
            starPos     = new Vector2Int(4, 0),
            obstacles   = new Vector2Int[0],
            hint        = "Right × 4, Up × 4!"
        },
        // Level 3 – obstacles
        new LevelData {
            cols = 5, rows = 5,
            playerStart = new Vector2Int(0, 2),
            starPos     = new Vector2Int(4, 0),
            obstacles   = new Vector2Int[] {
                new Vector2Int(2, 2),
                new Vector2Int(3, 1)
            },
            hint = "Up × 2, Right × 4!"
        }
    };

    // ── References set by Bootstrap ─────────────────────────────────────────
    [HideInInspector] public PlayerMover  playerMover;
    [HideInInspector] public CommandQueue commandQueue;
    [HideInInspector] public UIManager    uiManager;
    [HideInInspector] public GridBuilder  gridBuilder;

    // ── State ────────────────────────────────────────────────────────────────
    int currentLevel = 0;
    GameState state  = GameState.Editing;

    public LevelData CurrentLevel => Levels[currentLevel];
    public int LevelNumber         => currentLevel + 1;
    public int TotalLevels         => Levels.Length;

    // ── Public API ────────────────────────────────────────────────────────────
    public void StartGame()    => LoadLevel(0);
    public void RestartLevel() => LoadLevel(currentLevel);
    public void NextLevel()    => LoadLevel(currentLevel + 1);

    public void AddCommand(Direction dir)
    {
        if (state != GameState.Editing) return;
        commandQueue.Add(dir);
        uiManager.RefreshCommandQueue();
    }

    public void ClearCommands()
    {
        if (state != GameState.Editing) return;
        commandQueue.Clear();
        uiManager.RefreshCommandQueue();
    }

    public void RunCommands()
    {
        if (state != GameState.Editing) return;
        if (commandQueue.Count == 0) return;
        StartCoroutine(ExecuteCommands());
    }

    // ── Internal ──────────────────────────────────────────────────────────────
    void LoadLevel(int index)
    {
        currentLevel = Mathf.Clamp(index, 0, Levels.Length - 1);
        state = GameState.Editing;

        var lvl = Levels[currentLevel];
        gridBuilder.BuildGrid(lvl);
        playerMover.Setup(lvl.playerStart, lvl.cols, lvl.rows,
                          gridBuilder.CellSize, gridBuilder.GridOrigin,
                          lvl.obstacles, lvl.starPos);
        commandQueue.Clear();
        uiManager.LoadLevel(currentLevel + 1, Levels.Length);
    }

    IEnumerator ExecuteCommands()
    {
        state = GameState.Running;
        uiManager.SetButtonsInteractable(false);

        foreach (var dir in commandQueue.Commands)
        {
            MoveResult result = MoveResult.Success;
            bool done = false;
            StartCoroutine(playerMover.MoveStep(dir, r => { result = r; done = true; }));

            yield return new WaitUntil(() => done);
            yield return new WaitForSeconds(0.12f);

            if (result == MoveResult.ReachedStar)
            {
                state = GameState.Won;
                bool hasNext = currentLevel + 1 < Levels.Length;
                uiManager.ShowWin(hasNext);
                yield break;
            }

            if (result == MoveResult.HitObstacle || result == MoveResult.OutOfBounds)
            {
                state = GameState.Failed;
                uiManager.ShowRetry();
                yield break;
            }
        }

        // Commands exhausted without reaching star
        state = GameState.Failed;
        uiManager.ShowRetry();
    }
}
