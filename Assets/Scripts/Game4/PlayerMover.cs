using UnityEngine;
using System.Collections;
using System;

public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 4f;

    // ── Shared result written by MoveStep, read by GameManager ───────────────
    [NonSerialized] public MoveResult LastResult = MoveResult.Success;
    [NonSerialized] public bool       Moving     = false;

    Vector2Int gridPos;
    int cols, rows;
    float cellSize;
    Vector3 gridOrigin;
    Vector2Int[] obstacles;
    Vector2Int starPos;
    Vector2Int startPos;

    // ── Setup ─────────────────────────────────────────────────────────────────
    public void Setup(Vector2Int start, int cols, int rows, float cellSize,
                      Vector3 gridOrigin, Vector2Int[] obstacles, Vector2Int starPos)
    {
        this.startPos   = start;
        this.gridPos    = start;
        this.cols       = cols;
        this.rows       = rows;
        this.cellSize   = cellSize;
        this.gridOrigin = gridOrigin;
        this.obstacles  = obstacles;
        this.starPos    = starPos;
        transform.position = GridToWorld(start);
    }

    public void ResetToStart()
    {
        StopAllCoroutines();
        Moving  = false;
        gridPos = startPos;
        transform.position = GridToWorld(startPos);
    }

    // ── Public entry: GameManager yields on the returned Coroutine ────────────
    //   LastResult is set before the Coroutine finishes.
    public Coroutine ExecuteMove(Direction dir)
        => StartCoroutine(MoveStep(dir));

    // ── Core move coroutine (runs on PlayerMover's scheduler) ─────────────────
    IEnumerator MoveStep(Direction dir)
    {
        Moving = true;

        Vector2Int next = gridPos;
        switch (dir)
        {
            case Direction.Up:    next.y--; break;
            case Direction.Down:  next.y++; break;
            case Direction.Left:  next.x--; break;
            case Direction.Right: next.x++; break;
        }

        // Out of bounds
        if (next.x < 0 || next.x >= cols || next.y < 0 || next.y >= rows)
        {
            yield return StartCoroutine(BounceAnim(next));
            LastResult = MoveResult.OutOfBounds;
            Moving = false;
            yield break;
        }

        // Obstacle
        foreach (var obs in obstacles)
        {
            if (obs == next)
            {
                yield return StartCoroutine(BounceAnim(next));
                LastResult = MoveResult.HitObstacle;
                Moving = false;
                yield break;
            }
        }

        // Valid move
        yield return StartCoroutine(SmoothMove(GridToWorld(next)));
        gridPos = next;

        LastResult = (gridPos == starPos) ? MoveResult.ReachedStar : MoveResult.Success;
        Moving = false;
    }

    // ── Animation helpers ─────────────────────────────────────────────────────
    IEnumerator SmoothMove(Vector3 target)
    {
        Vector3 from     = transform.position;
        float   duration = 1f / moveSpeed;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float pct = Mathf.SmoothStep(0f, 1f, t / duration);
            transform.position = Vector3.Lerp(from, target, pct);
            yield return null;
        }
        transform.position = target;
    }

    IEnumerator BounceAnim(Vector2Int toward)
    {
        Vector3 from   = transform.position;
        Vector3 midway = Vector3.Lerp(from, GridToWorld(toward), 0.28f);
        float   dur    = 0.12f;

        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from, midway, t / dur);
            yield return null;
        }
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(midway, from, t / dur);
            yield return null;
        }
        transform.position = from;
    }

    // ── Utility ───────────────────────────────────────────────────────────────
    public Vector3 GridToWorld(Vector2Int pos)
        => gridOrigin + new Vector3(pos.x * cellSize, -pos.y * cellSize, 0f);
}
