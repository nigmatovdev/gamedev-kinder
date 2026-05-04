using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // ── Level Data ────────────────────────────────────────────────────────────
    public struct LevelData
    {
        public int cols, rows;
        public Vector2Int playerStart;
        public Vector2Int starPos;
        public Vector2Int[] obstacles;
        public string hint;
    }

    static readonly LevelData[] Levels =
    {
        // Level 1 – straight path → Right x4
        new LevelData {
            cols = 5, rows = 5,
            playerStart = new Vector2Int(0, 2),
            starPos     = new Vector2Int(4, 2),
            obstacles   = new Vector2Int[0],
            hint        = "Go Right 4 times!"
        },
        // Level 2 – L-shape → Right x4, Up x4
        new LevelData {
            cols = 5, rows = 5,
            playerStart = new Vector2Int(0, 4),
            starPos     = new Vector2Int(4, 0),
            obstacles   = new Vector2Int[0],
            hint        = "Right x4, then Up x4!"
        },
        // Level 3 – obstacles → Up x2, Right x4
        new LevelData {
            cols = 5, rows = 5,
            playerStart = new Vector2Int(0, 2),
            starPos     = new Vector2Int(4, 0),
            obstacles   = new Vector2Int[] {
                new Vector2Int(2, 2),
                new Vector2Int(3, 1)
            },
            hint = "Up x2, then Right x4!"
        }
    };

    // ── References (set by Bootstrap) ─────────────────────────────────────────
    [HideInInspector] public PlayerMover  playerMover;
    [HideInInspector] public CommandQueue commandQueue;
    [HideInInspector] public UIManager    uiManager;
    [HideInInspector] public GridBuilder  gridBuilder;

    // ── State ─────────────────────────────────────────────────────────────────
    int       currentLevel = 0;
    GameState state        = GameState.Editing;

    public LevelData CurrentLevel => Levels[currentLevel];

    // ── Public API ────────────────────────────────────────────────────────────
    public void StartGame()    => LoadLevel(0);
    public void RestartLevel() => LoadLevel(currentLevel);
    public void NextLevel()    => LoadLevel(Mathf.Min(currentLevel + 1, Levels.Length - 1));

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

    // ── Level load ────────────────────────────────────────────────────────────
    void LoadLevel(int index)
    {
        StopAllCoroutines();                       // cancel any in-flight execution
        currentLevel = Mathf.Clamp(index, 0, Levels.Length - 1);
        state = GameState.Editing;

        var lvl = Levels[currentLevel];
        gridBuilder.BuildGrid(lvl);

        playerMover.Setup(
            lvl.playerStart, lvl.cols, lvl.rows,
            gridBuilder.CellSize, gridBuilder.GridOrigin,
            lvl.obstacles, lvl.starPos);

        commandQueue.Clear();
        uiManager.LoadLevel(currentLevel + 1, Levels.Length, lvl.hint);
    }

    // ── Command execution coroutine ───────────────────────────────────────────
    IEnumerator ExecuteCommands()
    {
        state = GameState.Running;
        uiManager.SetButtonsInteractable(false);

        // Snapshot the list so it can't change under us
        var cmds = new List<Direction>(commandQueue.Commands);

        foreach (var dir in cmds)
        {
            // ExecuteMove runs on PlayerMover — yield return waits for it to finish
            yield return playerMover.ExecuteMove(dir);

            yield return new WaitForSeconds(0.1f);   // brief pause between steps

            switch (playerMover.LastResult)
            {
                case MoveResult.ReachedStar:
                    state = GameState.Won;
                    uiManager.ShowWin(currentLevel + 1 < Levels.Length);
                    yield break;

                case MoveResult.HitObstacle:
                case MoveResult.OutOfBounds:
                    state = GameState.Failed;
                    uiManager.ShowRetry();
                    yield break;
            }
        }

        // Ran all commands without reaching the star
        state = GameState.Failed;
        uiManager.ShowRetry();
    }
}
